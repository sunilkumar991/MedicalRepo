namespace Nop.Web.Framework.Kendoui
{
    /// <summary>
    /// DataSource request
    /// </summary>
    public class DataSourceRequest
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public DataSourceRequest()
        {
            this.Page = 1;
            //Edited by Sunil Kumar at 17-1-19 from PageSize 10 to 25
            this.PageSize = 25;
        }

        /// <summary>
        /// Page number
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; }
    }
}
