namespace Bokify.Web.Core.ViewModels
{
    public class UserResetPasswordFormViewModel
    {
        public string Id { get; set; } = null!;
        [StringLength(100, ErrorMessage = Errors.MaxMinLinth, MinimumLength = 8),
            DataType(DataType.Password),
            RegularExpression(RegexPaterns.Password, ErrorMessage = Errors.InvalidPassword)]
        public string Password { get; set; } = null!;
        [Compare("Password", ErrorMessage = Errors.PasseordNotMatch),
            Display(Name = "Confirm password"),
            DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
