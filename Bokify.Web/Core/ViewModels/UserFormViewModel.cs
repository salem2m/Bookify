namespace Bokify.Web.Core.ViewModels
{
    public class UserFormViewModel
    {
        public string? Id { get; set; }
        [MaxLength(100, ErrorMessage = Errors.MaxLinth),
            RegularExpression(RegexPaterns.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)]
        public string FullName { get; set; } = null!;
        [MaxLength(25, ErrorMessage = Errors.MaxLinth),
            Display(Name ="Username"),
            RegularExpression(RegexPaterns.Username, ErrorMessage = Errors.InvalidUsername)]
        [Remote("AllowUserName", null!, AdditionalFields = "Id", ErrorMessage = Errors.DuplicateValue)]
        public string UserName { get; set; } = null!;
        [System.ComponentModel.DataAnnotations.EmailAddress,
            MaxLength(100, ErrorMessage =Errors.MaxLinth)]
        [Remote("AllowEmail", null!, AdditionalFields = "Id", ErrorMessage = Errors.DuplicateValue)]
        public string Email { get; set; } = null!;
        [StringLength(100, ErrorMessage = Errors.MaxMinLinth, MinimumLength = 8),
            DataType(DataType.Password),
            RegularExpression(RegexPaterns.Password, ErrorMessage =Errors.InvalidPassword)]
        [RequiredIf("Id == null", ErrorMessage = Errors.RequiredPassword)]
        public string? Password { get; set; } = null!;
        [Compare("Password", ErrorMessage = Errors.PasseordNotMatch),
            Display(Name = "Confirm Password"),
            DataType(DataType.Password)]
        [RequiredIf("Id == null", ErrorMessage = Errors.RequiredConfirmPassword)]
        public string? ConfirmPassword { get; set; } = null!;
        [Display(Name = "Roles")]
        public IList<string> SelectedRoles { get; set; } = new List<string>();
        public IEnumerable<SelectListItem>? Roles { get; set; }

    }
}
