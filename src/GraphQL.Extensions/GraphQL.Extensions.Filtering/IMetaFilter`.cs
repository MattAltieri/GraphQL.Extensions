using System.Collections.Generic;

namespace GraphQL.Extensions.Filtering {
    public interface IMetaFilter<TMetaFilter> : IMetaFilter
        where TMetaFilter : IMetaFilter<TMetaFilter> {

        List<TMetaFilter> And { get; set; }
        List<TMetaFilter> Or { get; set; }
         
    }
}