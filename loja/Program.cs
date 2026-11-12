using loja.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);                

// === O CÓDIGO DO PASSO 4 ENTRA EXATAMENTE AQUI ===
var connectionString = builder.Configuration.GetConnectionString("ConexaoMySQL");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
// =================================================

// Essa é a linha que você já deve ter aí no seu arquivo:
builder.Services.AddControllersWithViews();

// Ativa o leitor de contexto para o HTML conseguir ver a sessão
builder.Services.AddHttpContextAccessor();

// Ativa o serviço de sessão com tempo de expiração
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
