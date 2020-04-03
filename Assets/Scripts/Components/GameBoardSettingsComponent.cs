using Unity.Entities;

namespace Match3.ECS.Components
{
    public struct GameBoardSettingsComponent : IComponentData
    {
        public int Width;
        public int Height;
        public int SetSize;
        public int CellSpeed;
        public int MinGroupSize;
    }
}