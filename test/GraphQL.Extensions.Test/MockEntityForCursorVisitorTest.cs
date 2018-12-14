using System;

namespace GraphQL.Extensions.Test {
    public class MockEntityForCursorVisitorTest : IEquatable<MockEntityForCursorVisitorTest> {

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

        public bool Equals(MockEntityForCursorVisitorTest other) {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other)) return false;
            return this.Char.Equals(other.Char) &&
                   this.CharNull.Equals(other.CharNull) &&
                   this.Short.Equals(other.Short) &&
                   this.ShortNull.Equals(other.ShortNull) &&
                   this.Int.Equals(other.Int) &&
                   this.IntNull.Equals(other.IntNull) &&
                   this.Long.Equals(other.Long) &&
                   this.LongNull.Equals(other.LongNull) &&
                   this.Decimal.Equals(other.Decimal) &&
                   this.DecimalNull.Equals(other.DecimalNull) &&
                   this.Float.Equals(other.Float) &&
                   this.FloatNull.Equals(other.FloatNull) &&
                   this.Double.Equals(other.Double) &&
                   this.DoubleNull.Equals(other.DoubleNull) &&
                   this.DateTime.Equals(other.DateTime) &&
                   this.DateTimeNull.Equals(other.DateTimeNull) &&
                   this.String.Equals(other.String) &&
                   this.Cursor.Equals(other.Cursor);
        }

        public override bool Equals(object obj) => Equals(obj as MockEntityForCursorVisitorTest);

        public override int GetHashCode()
            => unchecked((229 *139) +
                this.Char.GetHashCode() +
                this.CharNull.GetHashCode() +
                this.Short.GetHashCode() +
                this.ShortNull.GetHashCode() +
                this.Int.GetHashCode() +
                this.IntNull.GetHashCode() +
                this.Long.GetHashCode() +
                this.LongNull.GetHashCode() +
                this.Decimal.GetHashCode() +
                this.DecimalNull.GetHashCode() +
                this.Float.GetHashCode() +
                this.FloatNull.GetHashCode() +
                this.Double.GetHashCode() +
                this.DoubleNull.GetHashCode() +
                this.DateTime.GetHashCode() +
                this.DateTimeNull.GetHashCode() +
                this.String.GetHashCode() +
                this.Cursor.GetHashCode()
            );

        

        public static bool Equals(MockEntityForCursorVisitorTest operand1, MockEntityForCursorVisitorTest operand2) {
            if (ReferenceEquals(operand1, operand2)) return true;
            if (ReferenceEquals(null, operand2)) return false;
            if (ReferenceEquals(null, operand1)) return false;
            return operand1.Char.Equals(operand2.Char) &&
                   operand1.CharNull.Equals(operand2.CharNull) &&
                   operand1.Short.Equals(operand2.Short) &&
                   operand1.ShortNull.Equals(operand2.ShortNull) &&
                   operand1.Int.Equals(operand2.Int) &&
                   operand1.IntNull.Equals(operand2.IntNull) &&
                   operand1.Long.Equals(operand2.Long) &&
                   operand1.LongNull.Equals(operand2.LongNull) &&
                   operand1.Decimal.Equals(operand2.Decimal) &&
                   operand1.DecimalNull.Equals(operand2.DecimalNull) &&
                   operand1.Float.Equals(operand2.Float) &&
                   operand1.FloatNull.Equals(operand2.FloatNull) &&
                   operand1.Double.Equals(operand2.Double) &&
                   operand1.DoubleNull.Equals(operand2.DoubleNull) &&
                   operand1.DateTime.Equals(operand2.DateTime) &&
                   operand1.DateTimeNull.Equals(operand2.DateTimeNull) &&
                   operand1.String.Equals(operand2.String) &&
                   operand1.Cursor.Equals(operand2.Cursor);
        }

        public static bool operator ==(MockEntityForCursorVisitorTest operand1, MockEntityForCursorVisitorTest operand2)
            => Equals(operand1, operand2);

        public static bool operator !=(MockEntityForCursorVisitorTest operand1, MockEntityForCursorVisitorTest operand2)
            => !(operand1 == operand2);
    }
}