namespace Bokify.Web.Core.Models
{
    public class Base
    {
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastUpdatedOn { get; set; }
        
    }
}
