using Spider_EMT.Configuration;
using Spider_EMT.DAL;
using Spider_EMT.Repository.Domain;
using Spider_EMT.Repository.Skeleton;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();

SqlDBHelper.CONNECTION_STRING = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddTransient<ISiteSelectionRepository, SiteSelectionRepository>();

builder.Services.AddTransient<IAtmTransactionRepository, AtmTransactionRepository>();
builder.Services.AddTransient<IBankRepository, BankRepository>();
builder.Services.AddTransient<ITransactionFeeRepository, TransactionFeeRepository>();
builder.Services.AddTransient<IBankTransactionSummaryRepository, BankTransactionSummaryRepository>();
builder.Services.AddTransient<ITerminalDetailsRepository, TerminalDetailsRepository>();
builder.Services.AddTransient<ICurrentBankDetailsRepository, CurrentBankDetailsRepository>();
builder.Services.AddTransient<ISSDataRepository>(provider =>
{
    IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
    string ssDataFilePath = configuration["ss_data_path"];
    return new SSDataRepository(ssDataFilePath);
});

builder.Services.AddAutoMapper(typeof(AutoMapperConfig));
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
