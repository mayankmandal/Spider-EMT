using AutoMapper;
using Spider_EMT.Configuration;
using Spider_EMT.DAL;
using Spider_EMT.Repository.Domain;
using Spider_EMT.Repository.Skeleton;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
