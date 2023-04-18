using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockboxControl.Core.Models
{
    /// <summary>
    /// Represents a paged result, with page number, results, and total items matching the query.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Page items
        /// </summary>
        public ICollection<T> Results { get; set; } = new List<T>();
        /// <summary>
        /// Total number of matching items including the items in the page.
        /// </summary>
        public int TotalMatches { get; set; }
        /// <summary>
        /// The requested page number.
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// The page size.
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Total number of pages with this given page size.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(((float)TotalMatches) / PageSize);
    }
}
