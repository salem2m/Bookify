using Bokify.Web.Core.Models;
using Bokify.Web.Core.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Security.Claims;

namespace Bokify.Web.Controllers
{
    public class SubscribersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IImageService _imageService;

        public SubscribersController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IImageService imageService)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            var subscriber = _context.Subscribers
                .Include(g=>g.Governorate)
                .Include(a=>a.Area)
                .SingleOrDefault(s=>s.Id == id);

            if (subscriber is null)
                return NotFound();

            var viewModel = _mapper.Map<SubscriberDetailsViewModel>(subscriber);

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View("Form", PopulateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriberFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            var Subscriber = _mapper.Map<Subscriber>(model);

            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image!.FileName)}";
            var imagePath = "/images/Subscribers";

            var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, imagePath, hasThumbnail: true);

            if (!isUploaded)
            {
                ModelState.AddModelError("Image", errorMessage!);
                return View("Form", PopulateViewModel(model));
            }

            Subscriber.ImageUrl = $"{imagePath}/{imageName}";
            Subscriber.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";
            Subscriber.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            _context.Add(Subscriber);
            _context.SaveChanges();

            //TODO: Send welcome email

            return RedirectToAction(nameof(Details), new { id = Subscriber.Id });
        }

        public IActionResult Edit(int id)
        {
            var subscriber = _context.Subscribers.SingleOrDefault(x => x.Id == id);
            if(subscriber is null)
                return NotFound();

            return View("Form", PopulateViewModel(_mapper.Map<SubscriberFormViewModel>(subscriber)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubscriberFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", PopulateViewModel(model));

            var subscriber = _context.Subscribers.SingleOrDefault(x => x.Id == model.Id);
            if (subscriber is null)
                return NotFound();

            if (model.Image is not null)
            {
                if (!string.IsNullOrEmpty(subscriber.ImageUrl))
                    _imageService.Delete(subscriber.ImageUrl, subscriber.ImageThumbnailUrl);

                var imageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
                var imagePath = "/images/subscribers";

                var (isUploaded, errorMessage) = await _imageService.UploadAsync(model.Image, imageName, imagePath, hasThumbnail: true);

                if (!isUploaded)
                {
                    ModelState.AddModelError("Image", errorMessage!);
                    return View("Form", PopulateViewModel(model));
                }

                model.ImageUrl = $"{imagePath}/{imageName}";
                model.ImageThumbnailUrl = $"{imagePath}/thumb/{imageName}";
            }

            else if (!string.IsNullOrEmpty(subscriber.ImageUrl))
            {
                model.ImageUrl = subscriber.ImageUrl;
                model.ImageThumbnailUrl = subscriber.ImageThumbnailUrl;
            }

            subscriber = _mapper.Map(model, subscriber);
            subscriber.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            subscriber.LastUpdatedOn = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index), new {id = subscriber.Id});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(SearchFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var subscriber = _context.Subscribers.SingleOrDefault(e => e.Email == model.Value || e.NationalId == model.Value || e.MobileNumber == model.Value);

            var viewModel = _mapper.Map<SubscriberSearchFormViewModel>(subscriber);

            return PartialView("_Result", viewModel);
        }

        public IActionResult AllowEmail(SubscriberFormViewModel model)
        {
            var subscriber =  _context.Subscribers.SingleOrDefault(e=> e.Email == model.Email);
            var isAllowed = subscriber is null || subscriber.Id.Equals(model.Id);

            return Json(isAllowed);
        }
        public IActionResult AllowNationalId(SubscriberFormViewModel model)
        {
            var subscriber =  _context.Subscribers.SingleOrDefault(e=> e.NationalId == model.NationalId);
            var isAllowed = subscriber is null || subscriber.Id.Equals(model.Id);

            return Json(isAllowed);
        }
        public IActionResult AllowMobileNumber(SubscriberFormViewModel model)
        {
            var subscriber =  _context.Subscribers.SingleOrDefault(e=> e.MobileNumber == model.MobileNumber);
            var isAllowed = subscriber is null || subscriber.Id.Equals(model.Id);

            return Json(isAllowed);
        }

        [Filters.AjaxOnly]
        public IActionResult GetAreas(int governorateId)
        {
            var areas = _context.Areas
                    .Where(a => a.GovernorateId == governorateId && !a.IsDeleted)
                    .OrderBy(g => g.Name)
                    .ToList();

            return Ok(_mapper.Map<IEnumerable<SelectListItem>>(areas));
        }

        private SubscriberFormViewModel PopulateViewModel(SubscriberFormViewModel? model = null)
        {
            SubscriberFormViewModel viewModel = model is null ? new SubscriberFormViewModel() : model;

            var governorates = _context.Governorates.Where(e => !e.IsDeleted).OrderBy(e => e.Name).ToList();
            viewModel.Governorate = _mapper.Map<IEnumerable<SelectListItem>>(governorates);

            if (model?.GovernorateId > 0)
            {
                var areas = _context.Areas.Where(a => a.GovernorateId == model.GovernorateId && !a.IsDeleted)
                    .OrderBy(a => a.Name)
                    .ToList();
                viewModel.Areas = _mapper.Map<IEnumerable<SelectListItem>>(areas);
            }

            return viewModel;
        }
    }
}
