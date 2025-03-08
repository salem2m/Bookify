namespace Bokify.Web.Core.ViewModels
{
    public class AuthorFormViewModel
    {
        public int Id { get; set; }
        [Remote("AllowItem", null!, AdditionalFields = "Id", ErrorMessage = Errors.DuplicateValue)]
        [MaxLength(100, ErrorMessage = Errors.MaxLinth), Display(Name = "Author"),
            RegularExpression(RegexPaterns.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)]
        public string Name { get; set; } = null!;
    }
}
