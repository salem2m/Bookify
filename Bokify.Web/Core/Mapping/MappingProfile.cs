namespace Bokify.Web.Core.Mapping
{
    public class MappingProfile : Profile 
    {
        public MappingProfile() {
            //category mapper
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryFormViewModel, Category>().ReverseMap();
            CreateMap<Category, SelectListItem>()
                .ForMember(dest=>dest.Value,opt=>opt.MapFrom(src=>src.Id))
                .ForMember(dest=>dest.Text,opt=>opt.MapFrom(dest=>dest.Name));

            //author mapper
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorFormViewModel, Author>().ReverseMap();
            CreateMap<Author, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(dest => dest.Name));

            //book mapper
            CreateMap<BookFormViewModel, Book>()
                .ReverseMap()
                .ForMember(dest => dest.Categories, opt => opt.Ignore());
            CreateMap<Book, BookViewModel>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author!.Name))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c=>c.Category!.Name).ToList()));
            CreateMap<BookCopy, BookCopyViewModel>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book!.Title));

            CreateMap<BookCopy, BookCopyFormViewModel>();

            //Users
            CreateMap<ApplicationUser, UsersViewModel>();
            CreateMap<UserFormViewModel, ApplicationUser>()
                .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(e => e.Email.ToUpper()))
                .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(e => e.UserName.ToUpper()))
                .ReverseMap();

            //Governorate
            CreateMap<Governorate, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(dest => dest.Name));

            //Areas
            CreateMap<Area, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(dest => dest.Name));

            //Subscribers
            CreateMap<SubscriberFormViewModel, Subscriber>().ReverseMap();
            CreateMap<Subscriber, SubscriberSearchFormViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
            CreateMap<Subscriber, SubscriberDetailsViewModel>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area!.Name))
                .ForMember(dest => dest.Governorate, opt => opt.MapFrom(src => src.Governorate!.Name));

        }
    }
}
