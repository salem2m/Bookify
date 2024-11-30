using Bokify.Web.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bokify.Web.Controllers
{
	public class CategoriesController : Controller
	{
		private ApplicationDbContext _context;

		public CategoriesController(ApplicationDbContext context)
		{
			_context = context;
		}
		[HttpGet]
		public IActionResult Index()
		{
			//TODO: use view model
			return View(_context.Categories.ToList());
		}
		[HttpGet]
		public IActionResult Create()
		{
			return View("Form");
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(CategoryFormViewModel model)
		{
			if (!ModelState.IsValid)
				return View("Form", model);
			_context.Add(new Category { Name = model.Name });
			_context.SaveChanges();

			return RedirectToAction(nameof(Index));
		}
		[HttpGet]
		public IActionResult Edit(int id)
		{
			var category = _context.Categories.Find(id);
			if (category is null)
				return NotFound();
			var ViewModel = new CategoryFormViewModel { Id = id, Name = category.Name };


            return View("Form", ViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var category = _context.Categories.Find(model.Id);
            if (category is null)
                return NotFound();

			category.Name = model.Name;
			category.LastUpdetedOn = DateTime.Now;

			_context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
