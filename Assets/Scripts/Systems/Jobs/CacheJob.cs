using Match3.ECS.Components;
using Match3.ECS.Helpers;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ECS.Systems.Jobs
{
    public struct CacheJob : IJobParallelFor
    {
        [WriteOnly]
        [NativeDisableParallelForRestriction]
        public NativeArray<Entity> CachedEntities;
        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<CellPositionComponent> Positions;
        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<Entity> Entities;

        public ArrayToCoordinatesConverter Helper;
            
        public void Execute(int index)
        {
            var position = Positions[index];
            CachedEntities[Helper.GetI(position.x, position.y)] = Entities[index];
        }
    }
}