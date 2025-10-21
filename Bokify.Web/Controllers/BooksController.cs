using Bokify.Web.Settings;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;

namespace Bokify.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImageService _imageService;
        private readonly Cloudinary _cloudinary;

        private List<string> _allowedExtentions = new() { ".jpg", ".png", ".jpeg" };
        private int _maxAllowedSize = 2097152;

        public BooksController(ApplicationDbContext context,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment,
            IOptions<CloudinarySettings> cloudinary,
            IImageService imageService)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;

            Account account = new()
            {
                Cloud = cloudinary.Value.Cloud,
                ApiKey = cloudinary.Value.ApiKey,
                ApiSecret = cloudinary.Value.ApiSecret
            };

            _cloudinary = new Cloudinary(account);
            _imageService = imageService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, IgnoreAntiforgeryToken]
        public IActionResult GetBooks()
        {
            var skip = int.Parse(Request.Form["start"]!);
            var pageSize = int.Parse(Request.Form["length"]!);

            var searchValue = Request.Form["search[value]"];

            var sortColumnIndex = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            IQueryable<Book> books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Categories)
                .ThenInclude(c => c.Category);

            if (!string.IsNullOrEmpty(searchValue))
                books = books.Where(b => b.Title.Contains(searchValue!) || b.Author!.Name.Contains(searchValue!));

            books = books.OrderBy($"{sortColumn} {sortColumnDirection}");

            var data = books.Skip(skip).Take(pageSize).ToList();

            var mappedData = _mapper.Map<IEnumerable<BookViewModel>>(data);

            var recordsTotal = books.Count();

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }

        public IActionResult Details(int id)
        {
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

        public IActionResult Create()
        {

            return View("Form", PopulateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            var book = _mapper.Map<Book>(model);

            if (model.Image is not null)
            {

                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, "/images/books", hasThumbnail: true);

                if (!isUploaded)
                {
                    ModelState.AddModelError(nameof(Image), errorMessage!);
                    return View("Form", PopulateViewModel(model));
                }
                book.ImageUrl = $"/images/books/{imageName}";
                book.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";
                //Save Image in Cloudinary

                //using var straem = model.Image.OpenReadStream();

                //var imageParams = new ImageUploadParams
                //{
                //    File = new FileDescription(imageName, straem),
                //    UseFilename = true
                //};

                //var result = await _cloudinary.UploadAsync(imageParams);

                //book.ImageUrl = result.SecureUrl.ToString();
                //book.ImageThumbnailUrl = GetThumbnailUrl(book.ImageUrl);
                //book.ImagePublicId = result.PublicId;

            }

            foreach (var category in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = category });

            book.CreatedById = User.GetUserid();

            _context.Add(book);
            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = book.Id });
        }

        public IActionResult Edit(int id)
        {
            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(c => c.Id == id);
            if (book is null)
                return NotFound();
            var model = _mapper.Map<BookFormViewModel>(book);
            var viewmodel = PopulateViewModel(model);

            viewmodel.SelectedCategories = book.Categories.Select(b => b.CategoryId).ToList();

            return View("Form", viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            var book = _context.Books
                .Include(b => b.Categories)
                .Include(b => b.Copies)
                .SingleOrDefault(c => c.Id == model.Id);

            if (book is null)
                return NotFound();

            //string imgPubId = null;
            if (model.Image is not null)
            {
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    //Delete Image from sever 
                    _imageService.Delete(book.ImageUrl, book.ImageThumbnailUrl);

                    //Delete Image from Cloudinary
                    //await _cloudinary.DeleteResourcesAsync(book.ImagePublicId); 
                }
                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";

                var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, "/images/books", hasThumbnail: true);

                if (!isUploaded)
                {
                    ModelState.AddModelError(nameof(Image), errorMessage!);
                    return View("Form", PopulateViewModel(model));
                }
                model.ImageUrl = $"/images/books/{imageName}";
                model.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";


                //Save Image in Cloudinary 
                //using var straem = model.Image.OpenReadStream();

                //var imageParams = new ImageUploadParams
                //{
                //    File = new FileDescription(imageName, straem),
                //    UseFilename = true
                //};

                //var result = await _cloudinary.UploadAsync(imageParams);

                //model.ImageUrl = result.SecureUrl.ToString();
                //imgPubId=result.PublicId;
            }
            else if (model.Image is null && !string.IsNullOrEmpty(book.ImageUrl))
                model.ImageUrl = book.ImageUrl;
            model.ImageThumbnailUrl = book.ImageThumbnailUrl;

            book = _mapper.Map(model, book);
            book.LastUpdatedOn = DateTime.Now;
            book.LastUpdatedById = User.GetUserid();
            //book.ImageThumbnailUrl = GetThumbnailUrl(book.ImageUrl!);
            //book.ImagePublicId = imgPubId;
            if (!model.IsAvailableForRental)
                _context.BookCopies.Where(b=>b.BookId == book.Id)
                    .ExecuteUpdate(c=>c.SetProperty(c=>c.IsAvailableForRental, false));

            foreach (var category in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = category });

            _context.SaveChanges();

            return RedirectToAction(nameof(Details), new { id = book.Id });
        }

        [HttpPost]
        public IActionResult ChangeStatus(int id)
        {

            var book = _context.Books.Find(id);
            if (book is null)
                return NotFound();

            book.IsDeleted = !book.IsDeleted;
            book.LastUpdatedOn = DateTime.Now;
            book.LastUpdatedById = User.GetUserid();

            _context.SaveChanges();

            return Ok();
        }

        public IActionResult AllowItem(BookFormViewModel model)
        {
            var book = _context.Books.SingleOrDefault(c => c.Title == model.Title && c.AuthorId == model.AuthorId);
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


        private string GetThumbnailUrl(string url)
        {
            var separator = "image/upload/";
            var urlParts = url.Split(separator);

            var thumbnailUrl = $"{urlParts[0]}{separator}c_thumb,w_200,g_face/{urlParts[1]}";

            return thumbnailUrl;
        }

    }
}
