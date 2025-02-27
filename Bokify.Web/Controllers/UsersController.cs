using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bokify.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var usersViewModel = _mapper.Map<IEnumerable<UsersViewModel>>(users);
            return View(usersViewModel);
        }
    }
}
