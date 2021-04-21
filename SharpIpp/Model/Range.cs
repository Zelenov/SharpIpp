namespace SharpIpp.Model
{
    public struct Range
    {
        public int Lower { get; set; }
        public int Upper { get; set; }

        public Range(int lower, int upper)
        {
            Lower = lower;
            Upper = upper;
        }

        public override string ToString() => $"{Lower} - {Upper}";
    }
}