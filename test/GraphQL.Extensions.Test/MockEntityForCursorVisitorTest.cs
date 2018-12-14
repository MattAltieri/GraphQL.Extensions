using System;

namespace GraphQL.Extensions.Test
{
    public class MockEntityForCursorVisitorTest
    {

        public MockEntityForCursorVisitorTest() { }

        public MockEntityForCursorVisitorTest(MockEntityForCursorVisitorTest entity) {
            Char = entity.Char;
            CharNull = entity.CharNull;
            Short = entity.Short;
            ShortNull = entity.ShortNull;
            Int = entity.Int;
            IntNull = entity.IntNull;
            Long = entity.Long;
            LongNull = entity.LongNull;
            Decimal = entity.Decimal;
            DecimalNull = entity.DecimalNull;
            Float = entity.Float;
            FloatNull = entity.FloatNull;
            Double = entity.Double;
            DoubleNull = entity.DoubleNull;
            DateTime = entity.DateTime;
            DateTimeNull = entity.DateTimeNull;
            String = entity.String;
            Cursor = entity.Cursor;
        }

        public MockEntityForCursorVisitorTest(MockEntityForCursorVisitorTest entity, string newCursor)
            : this(entity)
            => Cursor = newCursor;

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



        public string Cursor { get; set; }
    }
}