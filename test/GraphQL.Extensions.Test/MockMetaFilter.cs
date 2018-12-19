using System;
using System.Collections.Generic;
using GraphQL.Extensions.Filtering;

namespace GraphQL.Extensions.Test {
    public class MockMetaFilter : IMetaFilter<MockMetaFilter> {
        
        public List<MockMetaFilter> And { get; set; }
        public List<MockMetaFilter> Or { get; set; }
        
        // char
        public char? Char { get; set; }
        public char? Char_not { get; set; }
        public char[] Char_in { get; set; }
        public char[] Char_not_in { get; set; }

        // char?
        public char? CharN { get; set; }
        public char? CharN_not { get; set; }
        public char[] CharN_in { get; set; }
        public char[] CharN_not_in { get; set; }
        public bool? CharN_null { get; set; }
        public bool? CharN_not_null { get; set; }

        // short
        public short? Short { get; set; }
        public short? Short_not { get; set; }
        public short[] Short_in { get; set; }
        public short[] Short_not_in { get; set; }
        public short? Short_lt { get; set; }
        public short? Short_lte { get; set; }
        public short? Short_gt { get; set; }
        public short? Short_gte { get; set; }

        // short?
        public short? ShortN { get; set; }
        public short? ShortN_not { get; set; }
        public short[] ShortN_in { get; set; }
        public short[] ShortN_not_in { get; set; }
        public short? ShortN_lt { get; set; }
        public short? ShortN_lte { get; set; }
        public short? ShortN_gt { get; set; }
        public short? ShortN_gte { get; set; }
        public bool? ShortN_null { get; set; }
        public bool? ShortN_not_null { get; set; }

        // int
        public int? Int { get; set; }
        public int? Int_not { get; set; }
        public int[] Int_in { get; set; }
        public int[] Int_not_in { get; set; }
        public int? Int_lt { get; set; }
        public int? Int_lte { get; set; }
        public int? Int_gt { get; set; }
        public int? Int_gte { get; set; }

        // int?
        public int? IntN { get; set; }
        public int? IntN_not { get; set; }
        public int[] IntN_in { get; set; }
        public int[] IntN_not_in { get; set; }
        public int? IntN_lt { get; set; }
        public int? IntN_lte { get; set; }
        public int? IntN_gt { get; set; }
        public int? IntN_gte { get; set; }
        public bool? IntN_null { get; set; }
        public bool? IntN_not_null { get; set; }

        // long
        public long? Long { get; set; }
        public long? Long_not { get; set; }
        public long[] Long_in { get; set; }
        public long[] Long_not_in { get; set; }
        public long? Long_lt { get; set; }
        public long? Long_lte { get; set; }
        public long? Long_gt { get; set; }
        public long? Long_gte { get; set; }

        // long?
        public long? LongN { get; set; }
        public long? LongN_not { get; set; }
        public long[] LongN_in { get; set; }
        public long[] LongN_not_in { get; set; }
        public long? LongN_lt { get; set; }
        public long? LongN_lte { get; set; }
        public long? LongN_gt { get; set; }
        public long? LongN_gte { get; set; }
        public bool? LongN_null { get; set; }
        public bool? LongN_not_null { get; set; }

        // decimal
        public decimal? Decimal { get; set; }
        public decimal? Decimal_not { get; set; }
        public decimal[] Decimal_in { get; set; }
        public decimal[] Decimal_not_in { get; set; }
        public decimal? Decimal_lt { get; set; }
        public decimal? Decimal_lte { get; set; }
        public decimal? Decimal_gt { get; set; }
        public decimal? Decimal_gte { get; set; }

        // decimal?
        public decimal? DecimalN { get; set; }
        public decimal? DecimalN_not { get; set; }
        public decimal[] DecimalN_in { get; set; }
        public decimal[] DecimalN_not_in { get; set; }
        public decimal? DecimalN_lt { get; set; }
        public decimal? DecimalN_lte { get; set; }
        public decimal? DecimalN_gt { get; set; }
        public decimal? DecimalN_gte { get; set; }
        public bool? DecimalN_null { get; set; }
        public bool? DecimalN_not_null { get; set; }

        // float
        public float? Float { get; set; }
        public float? Float_not { get; set; }
        public float[] Float_in { get; set; }
        public float[] Float_not_in { get; set; }
        public float? Float_lt { get; set; }
        public float? Float_lte { get; set; }
        public float? Float_gt { get; set; }
        public float? Float_gte { get; set; }
        

        // float?
        public float? FloatN { get; set; }
        public float? FloatN_not { get; set; }
        public float[] FloatN_in { get; set; }
        public float[] FloatN_not_in { get; set; }
        public float? FloatN_lt { get; set; }
        public float? FloatN_lte { get; set; }
        public float? FloatN_gt { get; set; }
        public float? FloatN_gte { get; set; }
        public bool? FloatN_null { get; set; }
        public bool? FloatN_not_null { get; set; }

        // double
        public double? Double { get; set; }
        public double? Double_not { get; set; }
        public double[] Double_in { get; set; }
        public double[] Double_not_in { get; set; }
        public double? Double_lt { get; set; }
        public double? Double_lte { get; set; }
        public double? Double_gt { get; set; }
        public double? Double_gte { get; set; }

        // double?
        public double? DoubleN { get; set; }
        public double? DoubleN_not { get; set; }
        public double[] DoubleN_in { get; set; }
        public double[] DoubleN_not_in { get; set; }
        public double? DoubleN_lt { get; set; }
        public double? DoubleN_lte { get; set; }
        public double? DoubleN_gt { get; set; }
        public double? DoubleN_gte { get; set; }
        public bool? DoubleN_null { get; set; }
        public bool? DoubleN_not_null { get; set; }

        // string
        public string String { get; set; }
        public string String_not { get; set; }
        public string[] String_in { get; set; }
        public string[] String_not_in { get; set; }
        public string String_contains { get; set; }
        public string String_not_contains { get; set; }
        public string String_starts_with { get; set; }
        public string String_not_starts_with { get; set; }
        public string String_ends_with { get; set; }
        public string String_not_ends_with { get; set; }
        public bool String_null { get; set; }
        public bool String_not_null { get; set; }
        public bool String_empty { get; set; }
        public bool String_not_empty { get; set; }

        // DateTime
        public DateTime? DateTime { get; set; }
        public DateTime? DateTime_not { get; set; }
        public DateTime[] DateTime_in { get; set; }
        public DateTime[] DateTime_not_in { get; set; }
        public DateTime? DateTime_lt { get; set; }
        public DateTime? DateTime_lte { get; set; }
        public DateTime? DateTime_gt { get; set; }
        public DateTime? DateTime_gte { get; set; }

        // DateTime?
        public DateTime? DateTimeN { get; set; }
        public DateTime? DateTimeN_not { get; set; }
        public DateTime[] DateTimeN_in { get; set; }
        public DateTime[] DateTimeN_not_in { get; set; }
        public DateTime? DateTimeN_lt { get; set; }
        public DateTime? DateTimeN_lte { get; set; }
        public DateTime? DateTimeN_gt { get; set; }
        public DateTime? DateTimeN_gte { get; set; }
        public bool? DateTimeN_null { get; set; }
        public bool? DateTimeN_not_null { get; set; }
    }
}