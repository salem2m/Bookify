using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;

namespace Bokify.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailBodyBuilder _emailBodyBuilder;

		public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment, IEmailBodyBuilder emailBodyBuilder)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_mapper = mapper;
			_emailSender = emailSender;
			_webHostEnvironment = webHostEnvironment;
			_emailBodyBuilder = emailBodyBuilder;
		}

		public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersViewModel = _mapper.Map<IEnumerable<UsersViewModel>>(users);
            return View(usersViewModel);
        }

        [HttpGet]
        [Filters.AjaxOnly]
        public async Task<IActionResult> Create()
        {
            var viewModel = new UserFormViewModel
            {
                Roles = await _roleManager.Roles
                .Select(r => new SelectListItem
                {
                    Text = r.Name, Value = r.Name 
                })
                .ToListAsync()
            };
            return PartialView("_Form", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            ApplicationUser user = new()
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            };

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code },
                    protocol: Request.Scheme);

                var placeholders = new Dictionary<string, string>()
                {
                    {"imageUrl", "https://res.cloudinary.com/salemgomaa/image/upload/v1744808552/envelope-with-approved-letter-opened-envelope-document-with-blue-tick-icon-confirmation-email-vector-illustration_735449-472_o43qfy.avif"},
                    {"header", $"Hey {user.FullName}  thanks for joining us!"},
                    {"body", "please confirm your email"},
                    {"url", $"{HtmlEncoder.Default.Encode(callbackUrl!)}"},
                    {"linkTitle", "Active Account!"}
                };

                var body = _emailBodyBuilder.GetEmailBody(
					  EmailTemplates.Email, placeholders
                    );

                await _emailSender.SendEmailAsync("salemgomaa01@gmail.com", "thisis me", body);
                var users = await _userManager.Users.ToListAsync();
                var usersViewModel = _mapper.Map<IEnumerable<UsersViewModel>>(users);
                

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email", body);

                var viewModel = _mapper.Map<UsersViewModel>(user);
                return PartialView("_UserRow", viewModel);
            }

            return BadRequest(string.Join(',', result.Errors.Select(e=>e.Description)));
        }
        [HttpGet]
        [Filters.AjaxOnly]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            var viewModel = new UserResetPasswordFormViewModel { Id = user.Id };

            return PartialView("_ResetPasswordForm", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(UserResetPasswordFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(model.Id!);
            ;
            if (user is null)
                return NotFound();

            var currentPassword = user.PasswordHash;
            
            await _userManager.RemovePasswordAsync(user);
            
            var result = await _userManager.AddPasswordAsync(user, model.Password);
            if (result.Succeeded)
            {
                user.LastUpdatedOn = DateTime.Now;
                user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                await _userManager.UpdateAsync(user);

                var viewModel = _mapper.Map<UsersViewModel>(user);
                return PartialView("_UserRow", viewModel);
            }
            user.PasswordHash = currentPassword;
            await _userManager.UpdateAsync(user);

            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }

        [HttpGet]
        [Filters.AjaxOnly]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return NotFound();

            var viewModel = _mapper.Map<UserFormViewModel>(user);
            viewModel.SelectedRoles = await _userManager.GetRolesAsync(user);

            viewModel.Roles = await _roleManager.Roles
                .Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Name
                })
                .ToListAsync();

            return PartialView("_Form", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(model.Id!);

            if (user is null)
                return NotFound();

            user = _mapper.Map(model, user);

            user.LastUpdatedOn = DateTime.Now;
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var rolesUpdated = !roles.SequenceEqual(model.SelectedRoles);
                if (rolesUpdated)
                {
                    await _userManager.RemoveFromRolesAsync(user, roles);
                    await _userManager.AddToRolesAsync(user, model.SelectedRoles);
                }

				await _userManager.UpdateSecurityStampAsync(user);

				var viewModel = _mapper.Map<UsersViewModel>(user);
                return PartialView("_UserRow", viewModel);
            }
            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }

        public async Task<IActionResult> AllowUserName(UserFormViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            var isAllowed = user is null || user.Id.Equals(model.Id);

            return Json(isAllowed);
        }

        public async Task<IActionResult> AllowEmail(UserFormViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var isAllowed = user is null || user.Id.Equals(model.Id);

            return Json(isAllowed);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);

            if (user is null)
                return BadRequest();

            user.IsDeleted = !user.IsDeleted;  
            user.LastUpdatedOn = DateTime.Now;
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            await _userManager.UpdateAsync(user);

            if (user.IsDeleted)
                await _userManager.UpdateSecurityStampAsync(user);

            return Ok(user.LastUpdatedOn.ToString());
        }

		[HttpPost]
		public async Task<IActionResult> Unlock(string id)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			var user = await _userManager.FindByIdAsync(id);

			if (user is null)
				return BadRequest();

			var isLockedOut = await _userManager.IsLockedOutAsync(user);
            if (isLockedOut)
                await _userManager.SetLockoutEndDateAsync(user, null);

			await _userManager.UpdateAsync(user);

			return Ok();
		}
	}
}
