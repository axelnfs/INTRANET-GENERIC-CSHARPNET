using INTRANET_GENERIC.Data;

var builder = WebApplication.CreateBuilder(args);

// Obtener connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Inicializar DatabaseHelper
DatabaseHelper.Initialize(connectionString);

// Agregar servicios MVC
builder.Services.AddControllersWithViews();

// Agregar servicios para API
builder.Services.AddControllers();

var app = builder.Build();

// Configurar pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Para wwwroot (css, js, imágenes)
app.UseRouting();

// Ruta por defecto a Home/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapear controladores API
app.MapControllers();

app.Run();