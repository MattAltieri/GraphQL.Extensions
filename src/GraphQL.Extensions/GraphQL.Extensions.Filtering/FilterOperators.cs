using System;

namespace GraphQL.Extensions.Filtering {

    [Flags]
    public enum FilterOperators {
        Equal           = 0x00000001,
        Not             = 0x00000002,
        In              = 0x00000004,
        NotIn           = 0x00000008,
        lt              = 0x00000010,
        lte             = 0x00000020,
        gt              = 0x00000040,
        gte             = 0x00000080,
        Contains        = 0x00000100,
        NotContains     = 0x00000200,
        StartsWith      = 0x00000400,
        NotStartsWith   = 0x00000800,
        EndsWith        = 0x00001000,
        NotEndsWith     = 0x00002000,
        Null            = 0x00004000,
        NotNull         = 0x00008000,
        Empty           = 0x00010000,
        NotEmpty        = 0x00020000,
    }
}