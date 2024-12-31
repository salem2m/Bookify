namespace Bokify.Web.Core.Mapping
{
    public class MappingProfile : Profile 
    {
        public MappingProfile() {
            //category mapper
            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryFormViewModel, Category>().ReverseMap();

            //author mapper
            CreateMap<Author, AuthorViewModel>();
            CreateMap<AuthorFormViewModel, Author>().ReverseMap();



        }
    }
}
