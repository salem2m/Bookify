namespace Bokify.Web.Core.ViewModels
{
    public class RentalsReportViewModel
    {
        [Required(ErrorMessage = "the Duration field is required")]
        public string Date { get; set; } = null!;
        public PaginatedList<RentalCopy>? Rentals { get; set; }
    }
}
