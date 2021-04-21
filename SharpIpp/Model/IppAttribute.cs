namespace SharpIpp.Model
{
    public class IppAttribute
    {
        public IppAttribute(Tag tag, string name, object value)
        {
            Tag = tag;
            Name = name;
            Value = value;
        }

        public IppAttribute()
        {
            Name = null!;
            Value = null!;
        }

        public Tag Tag { get; set; }
        public string Name { get; set; }

        /// <summary>
        ///     Possible values:
        ///     <see cref="System.Int32" />
        ///     <see cref="System.Boolean" />
        ///     <see cref="System.String" />
        ///     <see cref="System.DateTimeOffset" />
        ///     <see cref="NoValue" />
        ///     <see cref="Range" />
        ///     <see cref="Resolution" />
        ///     <see cref="StringWithLanguage" />
        /// </summary>
        public object Value { get; set; }

        public override string ToString() => $"({Tag}) {Name}: {Value}";
    }
}