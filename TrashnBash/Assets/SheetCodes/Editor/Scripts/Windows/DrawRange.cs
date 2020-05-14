namespace SheetCodesEditor
{
    public class DrawRange
    {
        public readonly int xMin;
        public readonly int xMax;
        public readonly int yMin;
        public readonly int yMax;

        public DrawRange(int xMin, int xMax, int yMin, int yMax)
        {
            this.xMax = xMax;
            this.xMin = xMin;

            this.yMax = yMax;
            this.yMin = yMin;
        }
    }
}