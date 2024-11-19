using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rotativa.AspNetCore;
using ServiceStack.Text;
using SISGESAL.Areas.Identity.Data;
using SISGESAL.Data;
using SISGESAL.Models;

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexi�n desde la configuraci�n
var sisgesalConnectionString = builder.Configuration.GetConnectionString("SISGESALDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'SISGESALDbContextConnection' not found.");

// Registrar CodigoGenerator con la cadena de conexi�n
builder.Services.AddScoped<CodigoGenerator>(provider =>
    new CodigoGenerator(sisgesalConnectionString));

// Configuraci�n de la cadena de conexi�n para SISGESALDbContext
builder.Services.AddDbContext<SISGESALDbContext>(options =>
    options.UseSqlServer(sisgesalConnectionString)
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

// Configuraci�n de la cadena de conexi�n para SisgesalContext
var sisgesalContextConnectionString = builder.Configuration.GetConnectionString("SisgesalContext")
    ?? throw new InvalidOperationException("Connection string 'SisgesalContext' not found.");
builder.Services.AddDbContext<SisgesalContext>(options =>
    options.UseSqlServer(sisgesalContextConnectionString)
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information));

// Configuraci�n de Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<SISGESALDbContext>();

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();

// Agregar servicios para la aplicaci�n
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// **Registrar el servicio RequisicionService aqu�**
builder.Services.AddTransient<RequisicionService>(); // <-- Aqu� se agrega el servicio

var app = builder.Build();

// Configuraci�n del pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();



//app.UseMvc(routes =>
//{
//    routes.MapRoute(
//        name: "default",
//        template: "{controller=Home}/{action=Index}/{id?}");
//});



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//uso de rotativa para imprimir pdf
app.UseRotativa();

app.Run();

//RotativaConfiguration.Setup(Env, "path/relative/to/root");