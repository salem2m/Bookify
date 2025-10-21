namespace Bokify.Web.Controllers
{

    public class SearchController : Controller
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHashids _hashids;

        public SearchController(IHashids hashids, IMapper mapper, IApplicationDbContext context)
        {
            _hashids = hashids;
            _mapper = mapper;
            _context = context;
        }

        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Details(string b)
        {
            var id = int.Parse(_hashids.DecodeHex(b));

            var book = _context.Books
                .Include(a => a.Author)
                .Include(a => a.Copies)
                .Include(a => a.Categories)
                .ThenInclude(a => a.Category)
                .SingleOrDefault(b => b.Id == id);

            if (book is null)
                return NotFound();

            var viewModel = _mapper.Map<BookViewModel>(book);

            return View(viewModel);
        }

        public ActionResult Find(string query)
        {
            var books = _context.Books
                .Include(a => a.Author)
                .Where(b => !b.IsDeleted && (b.Title.Contains(query) || b.Author!.Name.Contains(query)))
                .Select(b => new { b.Title, Author = b.Author!.Name, b = _hashids.EncodeHex(b.Id.ToString()) })
                .ToList();

            return Ok(books);
        }
    }
}
