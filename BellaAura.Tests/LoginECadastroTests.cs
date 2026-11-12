using BellaAura.Controllers;
using BellaAura.Data;
using BellaAura.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace BellaAura.Tests;

public class LoginECadastroTests
{
    // ===== Ajudantes =====

    // Cria um banco novo e vazio na memória para cada teste
    private AppDbContext CriarBanco()
    {
        var opcoes = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opcoes);
    }

    // Cria o controller com sessão e TempData "de mentira"
    private HomeController CriarController(AppDbContext banco)
    {
        var http = new DefaultHttpContext();
        http.Session = new SessaoFalsa();

        var controller = new HomeController(banco);
        controller.ControllerContext = new ControllerContext { HttpContext = http };
        controller.TempData = new TempDataDictionary(http, new TempDataFalso());
        return controller;
    }

    // Já coloca no banco um usuário com senha embaralhada
    private void CriarUsuario(AppDbContext banco, string email, string senha)
    {
        var usuario = new Usuario
        {
            Nome = "Gabi",
            Email = email,
            Cep = "00000-000",
            Numero = "1",
            Endereco = "Rua A",
            Bairro = "Centro",
            CidadeEstado = "SP"
        };
        usuario.Senha = new PasswordHasher<Usuario>().HashPassword(usuario, senha);
        banco.Usuarios.Add(usuario);
        banco.SaveChanges();
    }

    // ===== Testes do Cadastro =====

    [Fact]
    public void Cadastro_salva_a_senha_embaralhada()
    {
        var banco = CriarBanco();
        var controller = CriarController(banco);

        var resultado = controller.Cadastrar("Gabi", "gabi@teste.com", "senha123",
            "00000-000", "1", "Rua A", "Centro", "SP");

        // Depois de cadastrar, vai para a tela de Login
        var redirecionamento = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Login", redirecionamento.ActionName);

        // A senha no banco NÃO pode ser a original...
        var usuario = banco.Usuarios.Single();
        Assert.NotEqual("senha123", usuario.Senha);

        // ...mas o hash tem que bater com ela
        var conferencia = new PasswordHasher<Usuario>()
            .VerifyHashedPassword(usuario, usuario.Senha, "senha123");
        Assert.Equal(PasswordVerificationResult.Success, conferencia);
    }

    [Fact]
    public void Cadastro_com_email_repetido_mostra_erro()
    {
        var banco = CriarBanco();
        CriarUsuario(banco, "gabi@teste.com", "senha123");
        var controller = CriarController(banco);

        var resultado = controller.Cadastrar("Outra", "gabi@teste.com", "outra123",
            "00000-000", "2", "Rua B", "Centro", "SP");

        Assert.IsType<ViewResult>(resultado);
        Assert.Equal("Este e-mail já está cadastrado! Tente fazer o login.", controller.ViewData["Erro"]);
        Assert.Equal(1, banco.Usuarios.Count()); // não duplicou
    }

    // ===== Testes do Login =====

    [Fact]
    public void Login_com_senha_certa_entra_e_guarda_o_nome()
    {
        var banco = CriarBanco();
        CriarUsuario(banco, "gabi@teste.com", "senha123");
        var controller = CriarController(banco);

        var resultado = controller.Login("gabi@teste.com", "senha123");

        var redirecionamento = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Index", redirecionamento.ActionName);
        Assert.Equal("Gabi", controller.HttpContext.Session.GetString("UsuarioNome"));
    }

    [Fact]
    public void Login_com_senha_errada_mostra_erro()
    {
        var banco = CriarBanco();
        CriarUsuario(banco, "gabi@teste.com", "senha123");
        var controller = CriarController(banco);

        var resultado = controller.Login("gabi@teste.com", "senhaERRADA");

        Assert.IsType<ViewResult>(resultado);
        Assert.Equal("E-mail ou senha incorretos!", controller.ViewData["Erro"]);
        Assert.Null(controller.HttpContext.Session.GetString("UsuarioNome")); // não logou
    }
}

// ===== Peças "de mentira" pros testes =====

// Sessão falsa: guarda os dados num dicionário
public class SessaoFalsa : ISession
{
    private readonly Dictionary<string, byte[]> _dados = new();
    public bool IsAvailable => true;
    public string Id => "teste";
    public IEnumerable<string> Keys => _dados.Keys;
    public void Clear() => _dados.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _dados.Remove(key);
    public void Set(string key, byte[] value) => _dados[key] = value;
    public bool TryGetValue(string key, [NotNullWhen(true)] out byte[]? value) => _dados.TryGetValue(key, out value);
}

// TempData falso: não guarda nada, só existe pro controller não quebrar
public class TempDataFalso : ITempDataProvider
{
    public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
    public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
}