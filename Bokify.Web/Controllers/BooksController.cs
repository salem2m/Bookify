namespace Bokify.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private List<string> _allowedExtentions = new() {".jpg", ".png", ".jpeg" };
        private int _maxAllowedSize = 2097152;

        public BooksController(ApplicationDbContext context,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create ()
        {
            
            return View("Form", PopulateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookFormViewModel model)
        {
            if(!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            var book = _mapper.Map<Book>(model);

            if (model.Image != null) {
                var extension = model.Image.FileName;

                if (!_allowedExtentions.Contains(Path.GetExtension(extension)))
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtensions);
                    return View("Form", PopulateViewModel(model));
                }
                if (model.Image.Length>_maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.MaxSize);
                    return View("Form", PopulateViewModel(model));
                }
                var imgName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/imges/books", imgName);
                var stream = System.IO.File.Create(path);
                model.Image.CopyTo(stream);
                book.ImageUrl = imgName;
            }

            foreach (var category in model.SelectedCategories)
                book.Categories.Add(new BookCategory {CategoryId = category});

            _context.Add(book);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Include(b=>b.Categories).SingleOrDefault(c=>c.Id ==id);
            if (book is null)
                return NotFound();
            var model = _mapper.Map<BookFormViewModel>(book);
            var viewmodel = PopulateViewModel(model);

            viewmodel.SelectedCategories = book.Categories.Select(b => b.CategoryId).ToList();

            return View("Form", viewmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));
            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(c => c.Id == model.Id);
            if (book is null)
                return NotFound();

            if (model.Image != null)
            {
                if (!string.IsNullOrEmpty(model.ImageUrl)) 
                { 
                    var OldPath = Path.Combine($"{_webHostEnvironment.WebRootPath}/imges/books", book.ImageUrl);
                    if(System.IO.File.Exists(OldPath))
                        System.IO.File.Delete(OldPath);
                }
                var extension = model.Image.FileName;

                if (!_allowedExtentions.Contains(Path.GetExtension(extension)))
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.NotAllowedExtensions);
                    return View("Form", PopulateViewModel(model));
                }
                if (model.Image.Length > _maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Errors.MaxSize);
                    return View("Form", PopulateViewModel(model));
                }
                var imgName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/images/books", imgName);
                var stream = System.IO.File.Create(path);
                model.Image.CopyTo(stream);
                model.ImageUrl = imgName;
            }
            else if(model.Image is null && !string.IsNullOrEmpty(book.ImageUrl))
                model.ImageUrl = book.ImageUrl;

            book=_mapper.Map(model, book);
            book.LastUpdetedOn = DateTime.Now;

            foreach (var category in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = category });

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AllowItem(BookFormViewModel model)
        {
            var book = _context.Books.SingleOrDefault(c => c.Title == model.Title && c.AuthorId==model.AuthorId);
            var IsAllowed = book is null || book.Id.Equals(model.Id);
            return Json(IsAllowed);
        }

        private BookFormViewModel PopulateViewModel(BookFormViewModel? model = null)
        {
            BookFormViewModel viewModel = model is null ? new BookFormViewModel() : model;

            var authors = _context.Authors.Where(e => !e.IsDeleted).OrderBy(e => e.Name).ToList();
            var categories = _context.Categories.Where(e => !e.IsDeleted).OrderBy(e => e.Name).ToList();
            
            viewModel.Author = _mapper.Map<IEnumerable<SelectListItem>>(authors);
            viewModel.Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories);

            return viewModel;
        }
    }
}
