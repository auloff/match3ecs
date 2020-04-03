using Unity.Entities;

namespace Match3.ECS.Components
{
    public struct UserClickComponent : IComponentData
    {
        public int x;
        public int y;
    }
}
