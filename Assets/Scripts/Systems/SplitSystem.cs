using Match3.ECS.Components;
using Match3.ECS.Helpers;
using ECS.Systems.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ECS.Systems
{
    [UpdateAfter(typeof(SpawnerSystem))]
    public class SplitSystem : JobComponentSystem
    {
        private EntityQuery _positionsQuery;
        
        private struct SplitJob : IJob
        {
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<Entity> CachedEntities;
            
            [ReadOnly]
            public ComponentDataFromEntity<CellTypeComponent> CellType;

            public ArrayToCoordinatesConverter Helper;
            
            public void Execute()
            {
                var groupId = 1;
                var count = CachedEntities.Length;
                for (var i = 0; i < count; ++i)
                {
                    var checkedEntity = CachedEntities[i];
                    Analyse(i, CellType[checkedEntity].CellTypeID, groupId);
                    ++groupId;
                }
            }
            
            private void Analyse(int i, int typeId, int groupId)
            {
                if (i == -1) return;
                
                var entity = CachedEntities[i];

                // Check entity
                if (entity == Entity.Null) return;
                
                // Check type
                if (typeId != CellType[entity].CellTypeID) return;
                
                Analyse(Helper.GetUp(i), typeId, groupId);
                Analyse(Helper.GetDown(i), typeId, groupId);
                Analyse(Helper.GetRight(i), typeId, groupId);
                Analyse(Helper.GetLeft(i), typeId, groupId);
            }
        }

        protected override void OnCreate()
        {
            _positionsQuery = GetEntityQuery(ComponentType.ReadOnly<CellPositionComponent>());
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var settings = GetSingleton<GameBoardSettingsComponent>();

            if (_positionsQuery.CalculateEntityCount() != settings.Width * settings.Height)
                return inputDeps;
            
            var cachedEntities = new NativeArray<Entity>(settings.Width * settings.Height, Allocator.TempJob);

            var helper = new ArrayToCoordinatesConverter(settings.Width, settings.Height);

            var cacheJob = new CacheJob
            {
                CachedEntities = cachedEntities,
                Entities = _positionsQuery.ToEntityArray(Allocator.TempJob),
                Positions = _positionsQuery.ToComponentDataArray<CellPositionComponent>(Allocator.TempJob),
                Helper = helper
            };

            var splitJob = new SplitJob
            {
                CachedEntities = cachedEntities,
                CellType = GetComponentDataFromEntity<CellTypeComponent>(true),
                Helper = helper
            };
            
            var jobHandle = cacheJob.Schedule(_positionsQuery.CalculateEntityCount(), 32, inputDeps); 

            jobHandle = splitJob.Schedule(jobHandle);
            
            return jobHandle;
        }
    }
}
