using Microsoft.AspNetCore.Mvc;

namespace Bokify.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        readonly ApplicationDbContext _context;
        readonly IMapper _mapper;

        public DashboardController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var books = _context.BookCopies.Where(b=>!b.IsDeleted).Count();
            var numberOfSubscribers = _context.Subscribers.Where(b=>!b.IsDeleted).Count();
            var numberOfBooks = books <= 10 ? books : books /10 * 10;

            var lastAddedBooks = _context.Books
                .Include(a=>a.Author)
                .Where(b=>!b.IsDeleted)
                .OrderByDescending(b=>b.Id)
                .Take(8)
                .ToList();

            var topBooks = _context.RentalCopies
                .Include(c => c.BookCopy)
                .ThenInclude(c => c!.Book)
                .ThenInclude(b => b!.Author)
                .GroupBy(c => new
                {
                    c.BookCopy!.BookId,
                    c.BookCopy!.Book!.Title,
                    c.BookCopy!.Book!.ImageThumbnailUrl,
                    AuthorName = c.BookCopy!.Book!.Author!.Name
                })
                .Select(b => new
                {
                    b.Key.BookId,
                    b.Key.Title,
                    b.Key.ImageThumbnailUrl,
                    b.Key.AuthorName,
                    Count = b.Count()
                })
                .OrderByDescending(b => b.Count)
                .Take(6)
                .Select(b => new BookViewModel
                {
                    Id = b.BookId,
                    Title = b.Title,
                    ImageThumbnailUrl = b.ImageThumbnailUrl,
                    Author = b.AuthorName
                })
                .ToList();

            var viewModel = new DashboardViewModel
            {
                NumberOfCopies = numberOfBooks,
                NumberOfSubscribers = numberOfSubscribers,
                LastAddedBooks = _mapper.Map<IEnumerable<BookViewModel>>(lastAddedBooks),
                TopBooks = topBooks
            };

            return View(viewModel);
        }

        [Filters.AjaxOnly]
        public IActionResult GetRentalsPerDay(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.Today.AddDays(-29);
            endDate ??= DateTime.Today;

            var data = _context.RentalCopies
                .Where(r => r.RentalDate >= startDate && r.RentalDate <= endDate)
                .GroupBy(c => new { Date = c.RentalDate })
                .Select(g => new ChartItemViewModel { Label=g.Key.Date.ToString("d MMM"), Value = g.Count().ToString()})
                .ToList();

            return Ok(data);
        }

        [Filters.AjaxOnly]
        public IActionResult GetSubscribersPerGovernorate()
        {
            var data = _context.Subscribers
                .Include(g=>g.Governorate)
                .Where(s=>!s.IsDeleted)
                .GroupBy(s => new { Governorate=s.Governorate!.Name})
                .Select(i=> new ChartItemViewModel { Label = i.Key.Governorate, Value = i.Count().ToString()})
                .ToList();

            return Ok(data);
        }
    }
}
