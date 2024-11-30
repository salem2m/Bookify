namespace Bokify.Web.Core.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = "Max Lenth cannot be mor than 100 chr.")]
        public string Name { get; set; } = null!;
    }
}
