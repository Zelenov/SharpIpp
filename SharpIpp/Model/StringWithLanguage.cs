namespace SharpIpp.Model
{
    public struct StringWithLanguage
    {
        public string Language { get; set; }
        public string Value { get; set; }

        public StringWithLanguage(string language, string value)
        {
            Language = language;
            Value = value;
        }

        public override string ToString() => $"{Value} ({Language})";
    }
}