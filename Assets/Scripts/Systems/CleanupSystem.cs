using Match3.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ECS.Systems
{
    [UpdateAfter(typeof(DestroySystem))]
    public class CleanupSystem : JobComponentSystem
    {
        private struct CleanClicksJob : IJob
        {
            [DeallocateOnJobCompletion]
            [ReadOnly]
            public NativeArray<Entity> Entities;
            public EntityCommandBuffer CommandBuffer;

            public void Execute()
            {
                var length = Entities.Length;
                for (var i = 0; i < length; ++i)
                {
                    CommandBuffer.DestroyEntity(Entities[i]);
                }
            }
        }
        
        private BeginInitializationEntityCommandBufferSystem _commandBuffer;
        private EntityQuery _clickedQuery;

        protected override void OnCreate()
        {
            _clickedQuery = GetEntityQuery(ComponentType.ReadOnly<UserClickComponent>());
            _commandBuffer = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            RequireForUpdate(_clickedQuery);
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var cleanClicksJob = new CleanClicksJob
            {
                CommandBuffer = _commandBuffer.CreateCommandBuffer(),
                Entities = _clickedQuery.ToEntityArray(Allocator.TempJob)
            };
            
            var clicksJobHandle = cleanClicksJob.Schedule(inputDeps);
            return clicksJobHandle;
        }
    }
}