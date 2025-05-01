namespace Bokify.Web.Core.ViewModels
{
    public class SubscriberFormViewModel
    {
        public string? Key { get; set; }

        [Display(Name = "First Name"),
            MaxLength(100, ErrorMessage = Errors.MaxLinth),
            RegularExpression(RegexPaterns.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Last Name"), 
            MaxLength(100, ErrorMessage = Errors.MaxLinth),
            RegularExpression(RegexPaterns.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)]
        public string LastName { get; set; } = null!;

        [Display(Name = "Birth Date")]
        [AssertThat("DateOfBirth <= Today()", ErrorMessage = Errors.FutureDate)]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;

        [Display(Name = "National ID")]
        [MaxLength(14, ErrorMessage = Errors.MaxLinth),
           RegularExpression(RegexPaterns.NationalId, ErrorMessage = Errors.InvalidNationalId)]
        [Remote("AllowNationalId", null!, AdditionalFields = "Key", ErrorMessage = Errors.DuplicateValue)]
        public string NationalId { get; set; } = null!;

        [Phone]
        [Display(Name = "Phone number"), MaxLength(11, ErrorMessage = Errors.MaxLinth),
                RegularExpression(RegexPaterns.MobileNumber, ErrorMessage = Errors.InvalidPhoneNumber)]
        [Remote("AllowMobileNumber", null!, AdditionalFields = "Key", ErrorMessage = Errors.DuplicateValue)]
        public string MobileNumber { get; set; } = null!;

        [Display(Name = "Do you have WhatsApp?")]
        public bool HasWhattsApp { get; set; }

        [System.ComponentModel.DataAnnotations.EmailAddress,
            MaxLength(100, ErrorMessage = Errors.MaxLinth)]
        [Remote("AllowEmail", null!, AdditionalFields = "Key", ErrorMessage = Errors.DuplicateValue)]
        public string Email { get; set; } = null!;

        [RequiredIf("Key == ''", ErrorMessage = Errors.ReqImage)]
        public IFormFile? Image { get; set; } =null!;
        public string? ImageUrl { get; set; } 
        public string? ImageThumbnailUrl { get; set; } 

        [Display(Name = "Area")]
        public int AreaId { get; set; }
        public IEnumerable<SelectListItem>? Areas { get; set; } = new List<SelectListItem>();

        [Display(Name = "Governorate")]
        public int GovernorateId { get; set; }
        public IEnumerable<SelectListItem>? Governorate { get; set; }

        [MaxLength(500)]
        public string Address { get; set; } = null!;

        [Display(Name = "Black Liste")]
        public bool IsBlackListed { get; set; }
    }
}
