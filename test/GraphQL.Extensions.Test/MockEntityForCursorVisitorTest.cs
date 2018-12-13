using System;

namespace GraphQL.Extensions.Test
{
    public class MockEntityForCursorVisitorTest
    {
        public char Char { get; set; }
        public char? CharNull { get; set; }
        public short Short { get; set; }
        public short? ShortNull { get; set; }
        public int Int { get; set; }
        public int? IntNull { get; set; }
        public long Long { get; set; }
        public long? LongNull { get; set; }
        public decimal Decimal { get; set; }
        public decimal? DecimalNull { get; set; }
        public float Float { get; set; }
        public float? FloatNull { get; set; }
        public double Double { get; set; }
        public double? DoubleNull { get; set; }
        public DateTime DateTime { get; set; }
        public DateTime? DateTimeNull { get; set; }
        public string String { get; set; }
    }
}