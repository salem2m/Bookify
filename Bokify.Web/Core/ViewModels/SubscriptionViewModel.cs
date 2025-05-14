namespace Bokify.Web.Core.ViewModels
{
    public class SubscriptionViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status
        {
            get { return DateTime.Today > EndDate ? "Expired" : DateTime.Today < StartDate ? string.Empty : "Active"; }
        }
            
    }
}
