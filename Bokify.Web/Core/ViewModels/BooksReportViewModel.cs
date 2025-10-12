namespace Bokify.Web.Core.ViewModels
{
    public class BooksReportViewModel
    {
        [Display(Name = "Author")]
        public List<int>? SelectedAuthors { get; set; } = new();

        public IEnumerable<SelectListItem> Authors { get; set; } = new List<SelectListItem>();

        [Display(Name = "Category")]
        public List<int>? SelectedCategories { get; set; } = new();
        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        public PaginatedList<Book>? Books { get; set; }
    }
}
