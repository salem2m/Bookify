namespace Bokify.Web.Controllers
{

    public class CategoriesController : Controller
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(IApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            var viewmodel = _mapper.Map<IEnumerable<CategoryViewModel>>(categories);

            return View(viewmodel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View("Form");
        }

        [HttpPost]
        public IActionResult Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var category = _mapper.Map<Category>(model);

            category.CreatedById = User.GetUserid();

            _context.Categories.Add(category);
            _context.SaveChanges();

            var viewmodel = _mapper.Map<CategoryViewModel>(category);

            TempData["Message"] = "Saved successflly";

            return RedirectToAction(nameof(Index), viewmodel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category is null)
                return NotFound();
            var ViewModel = _mapper.Map<CategoryFormViewModel>(category);


            return View("Form", ViewModel);
        }

        [HttpPost]
        public IActionResult Edit(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var category = _context.Categories.Find(model.Id);
            if (category is null)
                return NotFound();

            category = _mapper.Map(model, category);
            category.LastUpdatedOn = DateTime.Now;
            category.LastUpdatedById = User.GetUserid();

            _context.SaveChanges();
            var viewmodel = _mapper.Map<CategoryViewModel>(category);
            TempData["Message"] = "Saved successflly";

            return RedirectToAction(nameof(Index), viewmodel);
        }

        [HttpPost]
        public IActionResult ChangeStatus(int id)
        {

            var category = _context.Categories.Find(id);
            if (category is null)
                return NotFound();

            category.IsDeleted = !category.IsDeleted;
            category.LastUpdatedOn = DateTime.Now;
            category.LastUpdatedById = User.GetUserid();

            _context.SaveChanges();

            return Ok(category.LastUpdatedOn.ToString());
        }

        public IActionResult AllowItem(CategoryFormViewModel model)
        {

            var category = _context.Categories.SingleOrDefault(c => c.Name == model.Name);
            var IsAllowed = category is null || category.Id.Equals(model.Id);
            return Json(IsAllowed);
        }

    }
}
