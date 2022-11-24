using Icos5.core6.DbManager;
using Icos5.core6.Services;
using Icos5.core6.Services.BmServices;
using Icos5.core6.Services.Ec;
using Icos5.core6.Services.FileServices;
using Icos5.core6.Services.HeaderServices;
using Icos5.core6.Services.KPlotServices;
using Icos5.core6.Services.ProfileServices;
using Icos5.core6.Services.SensorServices;
using Icos5.core6.Services.StorageServices;
using Icos5.core6.Services.VariablesServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IcosDbContext>(x => x.UseSqlServer(connectionString));

//Inject services
builder.Services.AddScoped<IUtilityServices, UtilityServices>();
builder.Services.AddScoped<IFIleService, EFFileService>();
builder.Services.AddScoped<IHeaderService, EFHeaderService>();
builder.Services.AddScoped<IStorageService, EFStorageService>();
builder.Services.AddScoped<IVariableService, VariableService>();
builder.Services.AddScoped<ISensorService, EFSensorService>();
builder.Services.AddScoped<IProfileService, EFProfileService>();
builder.Services.AddScoped<IKPlotService, EFKPlotService>();
builder.Services.AddScoped<IBmService, EFBmService>();
builder.Services.AddScoped<IEcServices, EFEcService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
