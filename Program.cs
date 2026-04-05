using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Load web.config so connection strings can be stored there.
builder.Configuration.AddXmlFile("web.config", optional: true, reloadOnChange: true);

// Add authentication and authorization.
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Login";
        options.Cookie.Name = "KardexAuth";
    });

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Logout");
    options.Conventions.AllowAnonymousToPage("/Privacy");
    options.Conventions.AllowAnonymousToPage("/Error");
});
builder.Services.AddControllers();
builder.Services.AddTransient<kardex_Web.Services.UsuarioService>();
builder.Services.AddTransient<kardex_Web.Services.PeriodoService>();
builder.Services.AddTransient<kardex_Web.Services.UnidadMedidaService>();
builder.Services.AddTransient<kardex_Web.Services.AutorizaService>();
builder.Services.AddTransient<kardex_Web.Services.CategoriaService>();
builder.Services.AddTransient<kardex_Web.Services.AreaTrabajoService>();
builder.Services.AddTransient<kardex_Web.Services.ProveedorService>();
builder.Services.AddTransient<kardex_Web.Services.ProyectoService>();
builder.Services.AddTransient<kardex_Web.Services.RetiraService>();
builder.Services.AddTransient<kardex_Web.Services.AsignarService>();
builder.Services.AddTransient<kardex_Web.Services.SalidaService>();
builder.Services.AddTransient<kardex_Web.Services.OrdenCompraService>();
builder.Services.AddTransient<kardex_Web.Services.ProductoService>();
builder.Services.AddTransient<kardex_Web.Services.EntradaService>();
builder.Services.AddTransient<kardex_Web.Services.MovimientoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
