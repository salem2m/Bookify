using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bokify.Web.Core.ViewModels
{
    public class BookFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(300, ErrorMessage =Errors.MaxLinth)]
        [Remote("AllowItem", null, AdditionalFields = "Id, AuthorId", ErrorMessage = Errors.DuplicateBook)]

        public string Title { get; set; } = null!;
        [Display(Name ="Author")]
        [Remote("AllowItem", null, AdditionalFields = "Id, Title", ErrorMessage = Errors.DuplicateBook)]
        public int AuthorId { get; set; }
        public IEnumerable<SelectListItem>? Author { get; set; }
        [MaxLength(150, ErrorMessage = Errors.MaxLinth)]
        public string publisher { get; set; } = null!;
        [Display(Name = "publishing Date")]
        [AssertThat("publishingDate <= Today()", ErrorMessage =Errors.FutureDate)]
        public DateTime publishingDate { get; set; } = DateTime.Now;
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageThumbnailUrl { get; set; }
        [MaxLength(50, ErrorMessage = Errors.MaxLinth)]
        public string Hall { get; set; } = null!;
        [Display(Name = "Is available for rental")]
        public bool IsAvailableForRental { get; set; }
        public string Description { get; set; } = null!;
        [Display(Name = "Selected Categories")]
        public IList<int> SelectedCategories { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? Categories { get; set; }

    }
}
