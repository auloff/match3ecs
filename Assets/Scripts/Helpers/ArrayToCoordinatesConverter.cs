namespace Match3.ECS.Helpers
{
    public struct ArrayToCoordinatesConverter
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public ArrayToCoordinatesConverter(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int GetRight(int i) => (i % Width < Width - 1) ? i + 1 : -1;
        public int GetLeft(int i) => (i % Width > 0) ? i - 1 : -1;
        public int GetUp(int i) => ((i += Width) < Width * Height) ? i : -1;
        public int GetDown(int i) => ((i -= Width) >= 0) ? i : -1;
        public int GetX(int i) => i % Width;
        public int GetY(int i) => i / Width;
        public int GetI(int x, int y) => y * Width + x;
    }
}