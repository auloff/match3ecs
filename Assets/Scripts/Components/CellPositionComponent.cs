using Unity.Entities;

namespace Match3.ECS.Components
{
    public struct CellPositionComponent : IComponentData
    {
        public int x;
        public int y;
    }
}