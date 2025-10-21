using Bokify.Web.Core.Mapping;
using Bokify.Web.Helpers;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;
using ViewToHTML.Extensions;
using WhatsAppCloudApi.Extensions;

namespace Bokify.Web
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.Configure<SecurityStampValidatorOptions>(options =>
                options.ValidationInterval = TimeSpan.Zero);

            services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                //// Default Password settings.
                //options.Password.RequireDigit = true;
                //options.Password.RequireLowercase = true;
                //options.Password.RequireNonAlphanumeric = true;
                //options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                //options.Password.RequiredUniqueChars = 1;
                options.User.RequireUniqueEmail = true;
            });

            services.AddDataProtection().SetApplicationName(nameof(Bokify));

            services.AddSingleton<IHashids>(_ => new Hashids(minHashLength: 11));

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IEmailBodyBuilder, EmailBodyBuilder>();

            services.AddControllersWithViews();

            services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
            services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));
            services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));

            services.AddWhatsAppApiClient(builder.Configuration);

            services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            services.AddHangfireServer();

            services.AddExpressiveAnnotations();

            services.Configure<AuthorizationOptions>(options =>
            options.AddPolicy("AdminsOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Admin);
            }));

            services.AddViewToHTML();

            //Add antiforgerytoken
            services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            return services;
        }
    }
}
