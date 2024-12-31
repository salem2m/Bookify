namespace Bokify.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }
        [Remote("AllowItem", null, AdditionalFields ="Id", ErrorMessage =Errors.DuplicateValue)]
        [MaxLength(100, ErrorMessage = Errors.MaxLinth), Display(Name ="Category")]
        public string Name { get; set; } = null!;
    }
}
