using Microsoft.AspNetCore.WebUtilities;

namespace Bokify.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHashids _hashids;

        public HomeController(ILogger<HomeController> logger, IMapper mapper, ApplicationDbContext context, IHashids hashids)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
            _hashids = hashids;
        }

        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction(nameof(Index), "Dashboard");

            var lastAddedBooks = _context.Books
                .Include(a => a.Author)
                .Where(b => !b.IsDeleted)
                .OrderByDescending(b => b.Id)
                .Take(10)
                .ToList();

            var viewModel = _mapper.Map<IEnumerable<BookViewModel>>(lastAddedBooks);

            foreach (var bookViewModel in viewModel)
                bookViewModel.b = _hashids.EncodeHex(bookViewModel.Id.ToString());
             

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode = 500)
        {
            return View(new ErrorViewModel { ErrorCode = statusCode, ErrorDescription=ReasonPhrases.GetReasonPhrase(statusCode) });
        }
    }
}
