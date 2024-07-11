using AutoMapper;
using Spider_EMT.Configuration;
using Spider_EMT.Configuration.IService;
using Spider_EMT.Configuration.Service;
using Spider_EMT.DAL;
using Spider_EMT.Middlewares;
using Spider_EMT.Repository.Domain;
using Spider_EMT.Repository.Skeleton;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AddPageRoute("/Dashboard", "");
    });
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddLazyCache();

SqlDBHelper.CONNECTION_STRING = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

builder.Services.AddTransient<ISiteSelectionRepository>(provider =>
{
    IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
    string ssDataFilePath = configuration["ss_data_path"];
    IMapper mapper = provider.GetRequiredService<IMapper>();
    return new SiteSelectionRepository(ssDataFilePath, mapper);
});

builder.Services.AddTransient<INavigationRepository, NavigationRepository>();
builder.Services.AddScoped<IErrorLogRepository, ErrorLogRepository>();
builder.Services.AddScoped<IUniquenessCheckService, UniquenessCheckService>();

builder.Services.AddResponseCaching();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=600");
    }
});

app.UseResponseCaching();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseCustomExceptionHandlerMiddleware();
app.MapRazorPages();
app.MapControllers();

app.Run();
