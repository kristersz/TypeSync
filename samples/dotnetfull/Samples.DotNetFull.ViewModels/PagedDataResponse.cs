using System.Collections.Generic;

namespace Samples.DotNetFull.ViewModels
{
    public class PagedDataResponse<T>
    {
        public List<T> Rows { get; set; }
        public long TotalCount { get; set; }
    }
}
