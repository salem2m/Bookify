using Bokify.Web.Extensions;
using ClosedXML.Excel;
using OpenHtmlToPdf;
using ViewToHTML.Services;

namespace Bokify.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IViewRendererService _viewRendererService;
        public ReportsController(ApplicationDbContext context, IMapper mapper, IViewRendererService viewRendererService)
        {
            _context = context;
            _mapper = mapper;
            _viewRendererService = viewRendererService;
        }


        public IActionResult Index()
        {
            return View();
        }

        #region Books
        public IActionResult Books(List<int> selectedAuthors, List<int> selectedCategories, int pageNumber)
        {
            var authors = _context.Authors.OrderBy(a => a.Name).ToList();
            var categories = _context.Categories.OrderBy(a => a.Name).ToList();

            IQueryable<Book> books = _context.Books
                .Include(a => a.Author)
                .Include(a => a.Categories)
                .ThenInclude(c => c.Category)
                .Where(b => (!selectedAuthors.Any() || selectedAuthors.Contains(b.AuthorId))
                && (!selectedCategories.Any() || b.Categories.Any(c => selectedCategories.Contains(c.CategoryId))));

            var viewModel = new BooksReportViewModel
            {
                Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors),
                Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories),
            };

            if (pageNumber != 0)
                viewModel.Books = PaginatedList<Book>.Create(books, pageNumber, (int)ReportsConfigurations.PageSize);

            return View("Books", viewModel);
        }

        public async Task<IActionResult> ExportBooksToExcel(string authors, string categories)
        {
            var selectedAuthors = authors?.Split(',');
            var selectedCategories = categories?.Split(',');

            var books = _context.Books
                .Include(a => a.Author)
                .Include(a => a.Categories)
                .ThenInclude(c => c.Category)
                .Where(b => (string.IsNullOrEmpty(authors) || selectedAuthors!.Contains(b.AuthorId.ToString()))
                && (string.IsNullOrEmpty(categories) || b.Categories.Any(c => selectedCategories!.Contains(c.CategoryId.ToString())))).ToList();

            using var workBook = new XLWorkbook();

            var sheet = workBook.AddWorksheet("Books");

            var headerCells = new string[] { "Title", "Author", "Categories", "Publisher",
                "Publishing Date", "Hall", "Available for rental", "Status" };

            
            sheet.AddHeader(headerCells);

            for (int i = 0; i < books.Count(); i++)
            {
                sheet.Cell(i + 2, 1).SetValue(books[i].Title);
                sheet.Cell(i + 2, 2).SetValue(books[i].Author!.Name);
                sheet.Cell(i + 2, 3).SetValue(books[i].Publisher);
                sheet.Cell(i + 2, 4).SetValue(books[i].PublishingDate.ToString("d MMM yyyy", new CultureInfo("en-US")));
                sheet.Cell(i + 2, 5).SetValue(books[i].Hall);
                sheet.Cell(i + 2, 6).SetValue(string.Join(", ", books[i].Categories.Select(c => c.Category!.Name)));
                sheet.Cell(i + 2, 7).SetValue(books[i].IsAvailableForRental ? "Yes" : "No");
                sheet.Cell(i + 2, 8).SetValue(books[i].IsDeleted ? "Deleted" : "Avilable");
            }

            sheet.Format();

            await using var stream = new MemoryStream();
            workBook.SaveAs(stream);

            return File(stream.ToArray(), "application/octet-stream", "Books.xlsx");
        }

        public async Task<IActionResult> ExportBooksToPDF(string authors, string categories)
        {
            var selectedAuthors = authors?.Split(',');
            var selectedCategories = categories?.Split(',');

            var books = _context.Books
                .Include(a => a.Author)
                .Include(a => a.Categories)
                .ThenInclude(c => c.Category)
                .Where(b => (string.IsNullOrEmpty(authors) || selectedAuthors!.Contains(b.AuthorId.ToString()))
                && (string.IsNullOrEmpty(categories) || b.Categories.Any(c => selectedCategories!.Contains(c.CategoryId.ToString()))));

            var viewModel = _mapper.Map<IEnumerable<BookViewModel>>(books);

            var templatePath = "~/Views/Reports/BookTemplate.cshtml";
            var html = await _viewRendererService.RenderViewToStringAsync(ControllerContext, templatePath, viewModel);


            var pdf = Pdf.From(html).Content();

            return File(pdf.ToArray(), "application/octet-stream", "Books.pdf");
        }
        #endregion

        #region Rentals
        public IActionResult Rentals(string date, int pageNumber) 
        {
            var viewModel = new RentalsReportViewModel{Date=date};

            if (!string.IsNullOrEmpty(date))
            {
                if (!DateTime.TryParse(date.Split(" - ")[0], out DateTime start))
                {
                    ModelState.AddModelError("Date", Errors.InvalidStartDate);
                    return View(viewModel);
                }

                if (!DateTime.TryParse(date.Split(" - ")[1], out DateTime end))
                {
                    ModelState.AddModelError("Date", Errors.InvalidEndDate);
                    return View(viewModel);
                }
                
                var rentals = _context.RentalCopies
                .Include(s=>s.BookCopy).ThenInclude(s=>s!.Book).ThenInclude(a => a!.Author).Include(r=>r.Rental).ThenInclude(b=>b!.Subscriber)
                .Where(r=>r.RentalDate.Date>=start && r.RentalDate.Date <= end);
                
                if (pageNumber != 0)
                viewModel.Rentals = PaginatedList<RentalCopy>.Create(rentals, pageNumber, (int)ReportsConfigurations.PageSize);
            }

            
            ModelState.Clear();

            return View(viewModel); 
        }

        public async Task<IActionResult> ExportRentalsToExcel(string date)
        {
            var start = DateTime.Parse(date.Split(" - ")[0]);
            var end = DateTime.Parse(date.Split(" - ")[1]);

            var rentals = _context.RentalCopies
                 .Include(s => s.BookCopy).ThenInclude(s => s!.Book).ThenInclude(a => a!.Author).Include(r => r.Rental).ThenInclude(b => b!.Subscriber)
                 .Where(r => r.RentalDate.Date >= start && r.RentalDate.Date <= end).ToList();

            using var workBook = new XLWorkbook();

            var sheet = workBook.AddWorksheet("Rentals");

            var headerCells = new string[] { "Subscriber Id", "Subscriber Name", "Subscriber Phone", "Book Title",
                "Book Author", "Rental Date", "End Date", "Return Date", "Extended On" };


            sheet.AddHeader(headerCells);

            for (int i = 0; i < rentals.Count(); i++)
            {
                sheet.Cell(i + 2, 1).SetValue(rentals[i].Rental!.SubscriberId);
                sheet.Cell(i + 2, 2).SetValue(rentals[i].Rental!.Subscriber!.FirstName + " " + rentals[i].Rental!.Subscriber!.LastName);
                sheet.Cell(i + 2, 3).SetValue(rentals[i].Rental!.Subscriber!.MobileNumber);
                sheet.Cell(i + 2, 4).SetValue(rentals[i].BookCopy!.Book!.Title);
                sheet.Cell(i + 2, 5).SetValue(rentals[i].BookCopy!.Book!.Author!.Name);
                sheet.Cell(i + 2, 6).SetValue(rentals[i].RentalDate.ToString("d MMM yyyy", new CultureInfo("en-US")));
                sheet.Cell(i + 2, 7).SetValue(rentals[i].EndDate.ToString("d MMM yyyy", new CultureInfo("en-US")));
                sheet.Cell(i + 2, 8).SetValue(rentals[i].ReturnDate?.ToString("d MMM yyyy", new CultureInfo("en-US")));
                sheet.Cell(i + 2, 9).SetValue(rentals[i].ExtendedeOn?.ToString("d MMM yyyy", new CultureInfo("en-US")));
            }

            sheet.Format();

            await using var stream = new MemoryStream();
            workBook.SaveAs(stream);

            return File(stream.ToArray(), "application/octet-stream", "Rentals.xlsx");
        }

        public async Task<IActionResult> ExportRentalsToPDF(string date)
        {
            var start = DateTime.Parse(date.Split(" - ")[0]);
            var end = DateTime.Parse(date.Split(" - ")[1]);

            var rentals = _context.RentalCopies
                 .Include(s => s.BookCopy).ThenInclude(s => s!.Book).ThenInclude(a => a!.Author).Include(r => r.Rental).ThenInclude(b => b!.Subscriber)
                 .Where(r => r.RentalDate.Date >= start && r.RentalDate.Date <= end);

            var templatePath = "~/Views/Reports/RentalTemplate.cshtml";
            var html = await _viewRendererService.RenderViewToStringAsync(ControllerContext, templatePath, rentals);


            var pdf = Pdf.From(html).Content();

            return File(pdf.ToArray(), "application/octet-stream", "Rentals.pdf");
        }
        #endregion

        #region DelyedRentals
        public IActionResult DelyedRentals()
        {

            var rentals = _context.RentalCopies
                .Include(s => s.BookCopy).ThenInclude(s => s!.Book).Include(r => r.Rental).ThenInclude(b => b!.Subscriber)
                .Where(r => r.EndDate.Date < DateTime.Now && !r.ReturnDate.HasValue);

            var viewModel = _mapper.Map<IEnumerable<RentalCopyViewModel>>(rentals);

            return View(viewModel);
        }

        public async Task<IActionResult> ExportDelayedRentalsToExcel()
        {
            var rentals = _context.RentalCopies
                .Include(s => s.BookCopy).ThenInclude(s => s!.Book).Include(r => r.Rental).ThenInclude(b => b!.Subscriber)
                .Where(r => r.EndDate.Date < DateTime.Now && !r.ReturnDate.HasValue).ToList();

            var viewModel = _mapper.Map<IList<RentalCopyViewModel>>(rentals);

            using var workBook = new XLWorkbook();

            var sheet = workBook.AddWorksheet("Delayed_Rentals");

            var headerCells = new string[] { "Subscriber Id", "Subscriber Name", "Subscriber Phone", "Book Title",
                "Book Serial", "Rental Date", "End Date", "Extended On", "Delay in Days" };


            sheet.AddHeader(headerCells);

            for (int i = 0; i < viewModel.Count(); i++)
            {
                sheet.Cell(i + 2, 1).SetValue(viewModel[i].Rental!.Subscriber!.Id);
                sheet.Cell(i + 2, 2).SetValue(viewModel[i].Rental!.Subscriber!.FullName);
                sheet.Cell(i + 2, 3).SetValue(viewModel[i].Rental!.Subscriber!.MobileNumber);
                sheet.Cell(i + 2, 4).SetValue(viewModel[i].BookCopy!.BookTitle);
                sheet.Cell(i + 2, 5).SetValue(viewModel[i].BookCopy!.SerialNumber);
                sheet.Cell(i + 2, 6).SetValue(viewModel[i].RentalDate.ToString("d MMM yyyy", new CultureInfo("en-US")));
                sheet.Cell(i + 2, 7).SetValue(viewModel[i].EndDate.ToString("d MMM yyyy", new CultureInfo("en-US")));
                sheet.Cell(i + 2, 8).SetValue(viewModel[i].ExtendedOn is null ? "-" : viewModel[i].ExtendedOn?.ToString("d MMM yyyy", new CultureInfo("en-US")));
                sheet.Cell(i + 2, 9).SetValue(viewModel[i].DelayInDays);
            }

            sheet.Format();
            sheet.AddTable(viewModel.Count, 9);

            await using var stream = new MemoryStream();
            workBook.SaveAs(stream);

            return File(stream.ToArray(), "application/octet-stream", "Delayed_Rentals.xlsx");
        }

        public async Task<IActionResult> ExportDelayedRentalsToPDF()
        {
            var rentals = _context.RentalCopies
                .Include(s => s.BookCopy).ThenInclude(s => s!.Book).Include(r => r.Rental).ThenInclude(b => b!.Subscriber)
                .Where(r => r.EndDate.Date < DateTime.Now && !r.ReturnDate.HasValue);

            var viewModel = _mapper.Map<IEnumerable<RentalCopyViewModel>>(rentals);

            var templatePath = "~/Views/Reports/DelayedRentalTemplate.cshtml";
            var html = await _viewRendererService.RenderViewToStringAsync(ControllerContext, templatePath, viewModel);


            var pdf = Pdf.From(html).Content();

            return File(pdf.ToArray(), "application/octet-stream", "Delayed_Rentals.pdf");
        }
        #endregion
    }
}
