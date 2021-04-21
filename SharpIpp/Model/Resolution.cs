namespace SharpIpp.Model
{
    public struct Resolution
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public ResolutionUnit Units { get; set; }

        public Resolution(int width, int height, ResolutionUnit units)
        {
            Width = width;
            Height = height;
            Units = units;
        }

        public override string ToString() =>
            $"{Width}x{Height} ({(Units == ResolutionUnit.DotsPerInch ? "dpi" : Units == ResolutionUnit.DotsPerCm ? "dpcm" : "unknown")})";
    }
}