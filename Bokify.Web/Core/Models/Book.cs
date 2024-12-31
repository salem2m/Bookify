namespace Bokify.Web.Core.Models
{
    [Index(nameof(Title), nameof(AuthorId), IsUnique = true)]
    public class Book : Base
    {
        public int Id { get; set; }
        [MaxLength(300)]
        public string Title { get; set; }=null!;
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
        [MaxLength (150)]
        public string publisher { get; set; } = null!;
        public DateTime publishingDate { get; set; }
        public string? ImageUrl { get; set; }
        [MaxLength(50)]
        public string Hall { get; set; } = null!;
        public bool IsAvailableForRental { get; set; }
        public string Description { get; set; } = null!;
        public ICollection<BookCategory> Categories {  get; set; } = new List<BookCategory> (); 
    }
}
