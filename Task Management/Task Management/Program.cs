var builder = WebApplication.CreateBuilder(args);

// Adaugă suport pentru MVC (cu Views și Controllers)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurează pipeline-ul HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Pentru CSS, JS, imagini etc.
app.UseRouting();     // Obligatoriu pentru MVC

// Definește ruta implicită: / → HomeController.Index()
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();