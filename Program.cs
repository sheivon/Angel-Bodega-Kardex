var builder = WebApplication.CreateBuilder(args);

// Load web.config so connection strings can be stored there.
builder.Configuration.AddXmlFile("web.config", optional: true, reloadOnChange: true);

//kardex_Web.Services.ProductoService._connectionString = builder.Configuration.GetConnectionString("MySqlCon") ?? throw new InvalidOperationException("Connection string 'MySqlCon' not found.");
// Add services to the container.
builder.Services.AddRazorPages();
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

app.UseAuthorization();

app.MapControllers();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
