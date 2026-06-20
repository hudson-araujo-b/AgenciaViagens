using System.Linq;
using AgenciaViagens.Interfaces;
using AgenciaViagens.Models;
using AgenciaViagens.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configuração de Injeção de Dependência dos Repositórios
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IPassagemRepositorio, PassagemRepositorio>();
builder.Services.AddScoped<ICompraRepositorio, CompraRepositorio>();

// Configuração de Autenticação baseada em Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuario/Login"; // Página padrão de redirecionamento para login
    });


// Adiciona suporte a Controllers e Views (MVC)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configura o sistema para utilizar a cultura e formatação brasileira (PT-BR)
var supportedCultures = new[] { new System.Globalization.CultureInfo("pt-BR") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configura o pipeline de requisições HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Passagem/Error"); // Página amigável para tratamento de erros
}
app.UseStaticFiles(); // Habilita o uso de arquivos na pasta wwwroot (CSS, JS, imagens)

app.UseRouting();

// Habilita os middlewares de identificação e restrição de acesso
app.UseAuthentication();
app.UseAuthorization();

// Define a rota padrão de inicialização do MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Passagem}/{action=Index}/{id?}");

app.Run();
