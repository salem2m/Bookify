namespace Bokify.Web.Core.ViewModels
{
    public class SubscriberViewModel
    {
        public int Id { get; set; }
        public string? Key { get; set; }
        public string FullName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public string NationalId { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ImageUrl { get; set; } 
        public string? ImageThumbnailUrl { get; set; }
        public string? Area { get; set; } = null!;
        public string? Governorate { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public bool HasWhattsApp { get; set; }
        public bool IsBlackListed { get; set; }
        public IEnumerable<SubscriptionViewModel> Subscriptions { get; set; } = new List<SubscriptionViewModel>();
        public IEnumerable<RentalViewModel> Rentals { get; set; } = new List<RentalViewModel>();
    }
}
