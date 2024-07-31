using AutoMapper;
using Google.Cloud.RecaptchaEnterprise.V1;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Spider_EMT.Configuration;
using Spider_EMT.Configuration.Authorization.Models;
using Spider_EMT.Configuration.IService;
using Spider_EMT.Configuration.Service;
using Spider_EMT.DAL;
using Spider_EMT.Data;
using Spider_EMT.Data.Account;
using Spider_EMT.Repository.Domain;
using Spider_EMT.Repository.Skeleton;

var builder = WebApplication.CreateBuilder(args);

// Configure the connection string for SQL Database Helper
SqlDBHelper.CONNECTION_STRING = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
Configure(app);

app.Run();

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Add DbContext with SQL Server
    services.AddDbContext<ApplicationDbContext>(options => {
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        options.EnableSensitiveDataLogging();
    });

    // Add Identity services with default token providers
    services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    // Configure application cookies
    services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

    // Add cookie-based authentication
    services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "MyCookieAuth";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    }).AddFacebook(options =>
    {
        options.AppId = configuration["FacebookAppId"];
        options.AppSecret = configuration["FacebookAppSecret"];
    }).AddGoogle(options =>
    {
        options.ClientId = configuration["GoogleClientID"];
        options.ClientSecret = configuration["GoogleClientSecret"];
    });

    // Configure SMTP settings
    services.Configure<SmtpSetting>(configuration.GetSection("SMTP"));

    // Add authorization policies
    services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
    });

    // Add Razor Pages with custom page routes
    services.AddRazorPages()
        .AddRazorPagesOptions(options =>
        {
            options.Conventions.AddPageRoute("/Account/Login","");
        });

    // Register GoogleCloudSettings
    services.Configure<GoogleReCaptchaSettings>(options =>
    {
        options.SiteKey = configuration["GoogleCloud_GoogleReCaptcha_SiteKey"];
        options.SecretKey = configuration["GoogleCloud_GoogleReCaptcha_SecretKey"];
    });

    // Register RecaptchaEnterpriseServiceClient
    services.AddSingleton<RecaptchaEnterpriseServiceClient>(provider =>
    {
        var settings = provider.GetRequiredService<IOptions<GoogleReCaptchaSettings>>().Value;
        // Create client with the configuration or use default settings
        return RecaptchaEnterpriseServiceClient.Create();
    });

    services.AddControllers();

    // Add HTTP client services
    services.AddHttpClient("WebAPI",client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
    });

    // Add memory cache services
    services.AddMemoryCache();

    // Add lazy cache services
    services.AddLazyCache();

    // Add AutoMapper with configuration
    services.AddAutoMapper(typeof(AutoMapperConfig));

    //
    services.AddHttpContextAccessor();
    services.AddScoped<ICurrentUserService, CurrentUserService>();

    // Add repositories and services
    services.AddTransient<ISiteSelectionRepository>(provider =>
    {
        IConfiguration config = provider.GetRequiredService<IConfiguration>();
        string ssDataFilePath = config["ss_data_path"];
        IMapper mapper = provider.GetRequiredService<IMapper>();
        return new SiteSelectionRepository(ssDataFilePath, mapper);
    });
    // Register the RoleManager and UserManager services
    // services.AddScoped<RoleManager<IdentityRole<int>>>();
    // services.AddScoped<UserManager<ApplicationUser>>();

    services.AddSingleton<IEmailService, EmailService>();
    services.AddTransient<INavigationRepository, NavigationRepository>();
    services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
    services.AddScoped<IUniquenessCheckService, UniquenessCheckService>();

    // Add response caching
    services.AddResponseCaching();
}

void Configure(WebApplication app)
{
    // Use exception handler and HSTS in production environment
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    // Configure static file options with caching
    app.UseStaticFiles(new StaticFileOptions
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=600");
        }
    });

    // Use response caching middleware
    app.UseResponseCaching();

    // Use HTTPS redirection and static files middleware
    app.UseHttpsRedirection();
    app.UseStaticFiles();

    // Configure routing
    app.UseRouting();

    // Use authentication and authorization
    app.UseAuthentication();
    app.UseAuthorization();

    // Use custom exception handler middleware
    // app.UseCustomExceptionHandlerMiddleware();

    // Map Razor Pages and controllers
    app.MapRazorPages();
    app.MapControllers();
}