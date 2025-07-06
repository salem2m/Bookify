namespace Bokify.Web.Core.Const
{
    public static class Errors
    {
        public const string MaxLinth = "Max Lenth cannot be mor than {1} chr!";
        public const string MaxMinLinth = "The {0} must be at least {2} and at max {1} characters long.";
        public const string DuplicateValue = "{0} with the same Name is Already Exists!";
        public const string DuplicateBook = "Book with the same Name is Already Exists wiht the same Author!";
        public const string NotAllowedExtensions = "only jpg, jpeg or png are allowed";
        public const string MaxSize = "File cannot be mor than 2mb!";
        public const string FutureDate = "can not chosse a future date!";
        public const string InvalidRange = "{0} should be between {1} and {2}!";
        public const string PasseordNotMatch = "The password and confirmation password do not match.";
        public const string InvalidUsername = "Username can only contain letters or digits..";
        public const string RequiredPassword = "The Password field is required.";
        public const string RequiredConfirmPassword = "The Confirm password field is required.";
        public const string InvalidPassword = "password must be contain an uppercase character, lowercase character, a digit, and a non-alphanumeric character. Passwords must be at least 8 characters long.";
        public const string OnlyEnglishLetters = "Only English letters are allowed.";
        public const string OnlyArabicLetters = "Only Arabic letters are allowed.";
        public const string OnlyNumbersAndLetters = "Only Arabic/English letters or digits are allowed.";
        public const string DenySpecialCharacters = "Special characters are not allowed.";
        public const string InvalidPhoneNumber = "Invalid Phone Number.";
        public const string InvalidNationalId = "Invalid National ID.";
        public const string InvalidSerialNumber = "Invalid serial number.";
        public const string NotAvilableForRental = "This book/copy is not avilable for  rental.";
        public const string ReqImage = "Please select an Image.";
        public const string BlackListedSubscriber = "This subscriber is blacklisted.";
        public const string InactiveSubscriber = "This subscriber is inactive.";
        public const string MaxCopiesReached = "This subscriber has reached the max number for rentals.";
        public const string CopyIsInRental = "This copy is already rentaled.";
        public const string RentalNotAllowedForBlacklisted = "Rental cannot be extended for blacklisted subscribers.";
        public const string RentalNotAllowedForInactive = "Rental cannot be extended for this subscriber before renwal.";
        public const string ExtendNotAllowed = "Rental cannot be extended.";
        public const string PenaltyShouldBePaid = "Penalty should be paid.";
    }
}