using Match3.ECS.Components;
using Match3.ECS.Helpers;
using ECS.Systems.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ECS.Systems
{
    [UpdateAfter(typeof(SplitSystem))]
    public class DestroySystem : JobComponentSystem
    {
        private struct DestroyJob : IJob
        {
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<Entity> CachedEntities;
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<UserClickComponent> ClickedComponents;
            public EntityCommandBuffer CommandBuffer;

            public ArrayToCoordinatesConverter Helper;
            public int MinGroupSize;
            
            public void Execute()
            {
                var count = ClickedComponents.Length;
                for (var i = 0; i < count; ++i)
                {
                    var destroyPos = ClickedComponents[i];
                    var clickedEntity = CachedEntities[Helper.GetI(destroyPos.x, destroyPos.y)];
                    if(clickedEntity == Entity.Null) continue;

                    var groupSize = 0;
                    
                    if(groupSize < MinGroupSize)
                        continue;
                }
            }
        }

        private BeginInitializationEntityCommandBufferSystem _commandBuffer;
        private EntityQuery _clickedQuery;
        private EntityQuery _positionsQuery;

        protected override void OnCreate()
        {
            _clickedQuery = GetEntityQuery(ComponentType.ReadOnly<UserClickComponent>());
            _positionsQuery = GetEntityQuery(ComponentType.ReadOnly<CellPositionComponent>());
            _commandBuffer = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            RequireForUpdate(_clickedQuery);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var settings = GetSingleton<GameBoardSettingsComponent>();
            
            var cachedEntities = new NativeArray<Entity>(settings.Width * settings.Height, Allocator.TempJob);

            var helper = new ArrayToCoordinatesConverter(settings.Width, settings.Height);

            var cacheJob = new CacheJob
            {
                CachedEntities = cachedEntities,
                Entities = _positionsQuery.ToEntityArray(Allocator.TempJob),
                Positions = _positionsQuery.ToComponentDataArray<CellPositionComponent>(Allocator.TempJob),
                Helper = helper
            };
            
            var destroyJob = new DestroyJob
            {
                CachedEntities = cachedEntities,
                CommandBuffer = _commandBuffer.CreateCommandBuffer(),
                ClickedComponents = _clickedQuery.ToComponentDataArray<UserClickComponent>(Allocator.TempJob),
                Helper = helper,
                MinGroupSize = settings.MinGroupSize
            };

            var jobHandle = cacheJob.Schedule(_positionsQuery.CalculateEntityCount(), 32, inputDeps); 

            jobHandle = destroyJob.Schedule(jobHandle);
            
            _commandBuffer.AddJobHandleForProducer(jobHandle);

            return jobHandle;
        }
    }
}
