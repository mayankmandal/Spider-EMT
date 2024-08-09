using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Spider_EMT.Middlewares;
using Microsoft.AspNetCore.Authorization;

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
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

    // Add cookie-based authentication
    services.AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JwtSettings_SecretKey"])),
        };
    })
        .AddFacebook(options =>
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
        options.AddPolicy("PageAccess", policy => policy.Requirements.Add(new PageAccessRequirement()));
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

    services.AddControllers();

    // Add HTTP client services
    services.AddHttpClient("WebAPI",client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
    });

    services.AddSession(options =>
    {
        options.Cookie.HttpOnly = true;
        options.IdleTimeout = TimeSpan.FromHours(8);
        options.Cookie.IsEssential = true;
    });

    // Add memory cache services
    services.AddMemoryCache();

    // Add lazy cache services
    services.AddLazyCache();

    // Add AutoMapper with configuration
    services.AddAutoMapper(typeof(AutoMapperConfig));
    
    services.AddScoped<ICurrentUserService, CurrentUserService>();

    // Register the PageAccessHandler
    services.AddScoped<IAuthorizationHandler, PageAccessHandler>();

    services.AddHttpContextAccessor();

    // Add repositories and services
    services.AddScoped<ISiteSelectionRepository>(provider =>
    {
        IConfiguration config = provider.GetRequiredService<IConfiguration>();
        string ssDataFilePath = config["ss_data_path"];
        IMapper mapper = provider.GetRequiredService<IMapper>();
        return new SiteSelectionRepository(ssDataFilePath, mapper);
    });

    services.AddScoped<IEmailService, EmailService>();
    services.AddScoped<INavigationRepository, NavigationRepository>();
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

    app.UseSession();

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