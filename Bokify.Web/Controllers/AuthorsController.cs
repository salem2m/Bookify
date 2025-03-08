using Microsoft.AspNetCore.Mvc;
using Bokify.Web.Filters;
using Bokify.Web.Core.Models;
using Bokify.Web.Core.ViewModels;
using System.Security.Claims;
namespace Bokify.Web.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthorsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var authors = _context.Authors.AsNoTracking().ToList();
            var viewmodel = _mapper.Map<IEnumerable<AuthorViewModel>>(authors);

            return View(viewmodel);
        }
        [HttpGet]
        [Filters.AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var author = _mapper.Map<Author>(model);

            author.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.Add(author);
            _context.SaveChanges();

            var viewmodel = _mapper.Map<AuthorViewModel>(author);

            return PartialView("_AuthorRow", viewmodel);
        }
        [HttpGet]
        [Filters.AjaxOnly]
        public IActionResult Edit(int id)
        {

            var author = _context.Authors.Find(id);
            if (author is null)
                return NotFound();
            var ViewModel = _mapper.Map<AuthorFormViewModel>(author);


            return PartialView("_Form", ViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var author = _context.Authors.Find(model.Id);
            if (author is null)
                return NotFound();

            author = _mapper.Map(model, author);
            author.LastUpdatedOn = DateTime.Now;
            author.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();
            var viewmodel = _mapper.Map<AuthorViewModel>(author);

            return PartialView("_AuthorRow", viewmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(int id)
        {

            var author = _context.Authors.Find(id);
            if (author is null)
                return NotFound();

            author.IsDeleted = !author.IsDeleted;
            author.LastUpdatedOn = DateTime.Now;
            author.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();

            return Ok(author.LastUpdatedOn.ToString());
        }

        public IActionResult AllowItem(AuthorFormViewModel model)
        {
            var author = _context.Authors.SingleOrDefault(c => c.Name == model.Name);
            var IsAllowed = author is null || author.Id.Equals(model.Id);
            return Json(IsAllowed);
        }
    }
}
