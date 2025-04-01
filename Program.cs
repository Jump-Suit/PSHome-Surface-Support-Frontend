using PSHome_Surface_Support_Frontend.Infastructure.Data.Models;
using PSHome_Surface_Support_Frontend.Infastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<XmlConfigService>();
builder.Services.AddScoped<HomeTSSFormat>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
