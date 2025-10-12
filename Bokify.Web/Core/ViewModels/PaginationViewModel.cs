namespace Bokify.Web.Core.ViewModels
{
    public class PaginationViewModel
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int Start
        {
            get
            {
                var start = 1;

                if (TotalPages > (int)ReportsConfigurations.NumberOfPages)
                    start = PageNumber - 9 < 1 ? 1 : PageNumber - 9;

                return start;
            }
        }
        public int End
        {
            get
            {
                var end = TotalPages;
                var maxPages = (int)ReportsConfigurations.NumberOfPages;

                if (TotalPages > maxPages)
                    end = Start + maxPages > TotalPages ? TotalPages : Start + maxPages;

                return end;
            }
        }
    }
}
