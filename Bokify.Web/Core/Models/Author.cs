namespace Bokify.Web.Core.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Author : Base
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
    }
}
