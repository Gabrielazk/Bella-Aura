using BellaAura.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BellaAura.Data;
using Microsoft.AspNetCore.Identity;

namespace BellaAura.Controllers
{
    public class HomeController : Controller
    {
        // Criamos uma variável para guardar o acesso ao banco
        private readonly AppDbContext _context;

        // Ferramenta que embaralha e confere senhas
        private readonly PasswordHasher<Usuario> _hasher = new PasswordHasher<Usuario>();

        // O construtor do controller recebe o banco automaticamente
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult EsqueciSenha()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        public IActionResult Kits()
        {
            // Esta Action serve apenas para abrir a nossa nova página de Kits
            return View();
        }

        public IActionResult MontarKit()
        {
            return View();
        }

        public IActionResult Lancamentos()
        {
            return View();
        }

        public IActionResult Sucesso()
        {
            return View();
        }

        // Adicione isso abaixo da public IActionResult Detalhes()
        public IActionResult Checkout()
        {
            return View();
        }

        public IActionResult Carrinho()
        {
            return View();
        }

        public IActionResult DetalhesPremium()
        {
            return View();
        }

        public IActionResult MinhasCompras()
        {
            return View();
        }

        public IActionResult PerfumesMarcantes()
        {
            return View();
        }

        public IActionResult KitsPresenteaveis()
        {
            return View();
        }

        public IActionResult MaisDesejados()
        {
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string senha)
        {
            // Agora busca só pelo e-mail (a senha no banco está embaralhada)
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);

            if (usuario != null)
            {
                // Confere se a senha digitada bate com o hash salvo
                var resultado = _hasher.VerifyHashedPassword(usuario, usuario.Senha, senha);

                if (resultado != PasswordVerificationResult.Failed)
                {
                    HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
                    return RedirectToAction("Index");
                }
            }

            ViewBag.Erro = "E-mail ou senha incorretos!";
            return View();
        }

        [HttpPost]
        public IActionResult EsqueciSenha(string email)
        {
            // 1. Procura no banco se esse e-mail realmente pertence a uma cliente Bella Aura
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);

            if (usuario == null)
            {
                // Se o e-mail não existir no banco, avisa a usuária de um jeito amigável
                ViewBag.Erro = "Este e-mail não está cadastrado no nosso sistema.";
                return View();
            }

            // 2. Se achou o usuário, aqui a gente simula a montagem do e-mail premium
            string assunto = "Bella Aura ✨ - Recuperação de Senha";
            string linkRedefinir = "https://localhost:7193/Home/RedefinirSenha?email=" + email; // URL de teste

            string corpoEmail = $@"
        Olá, {usuario.Nome}! 
        Recebemos um pedido para redefinir a sua senha na Bella Aura.
        Para criar uma nova senha, clique no link abaixo ou copie e cole no seu navegador:
        {linkRedefinir}
        
        Se não foi você quem pediu, pode ignorar este e-mail com segurança. ✨";

            // 3. Como estamos em ambiente de teste, vamos salvar o e-mail no TempData 
            // para mostrar o sucesso e o conteúdo dele direto na tela para você ver!
            TempData["EmailSimulado"] = corpoEmail;
            TempData["MensagemSucesso"] = "Link de recuperação enviado com sucesso! Verifique sua caixa de entrada. ✨";

            return RedirectToAction("EsqueciSenha");
        }

        [HttpPost]
        public IActionResult Cadastrar(string nome, string email, string senha, string cep, string numero, string endereco, string bairro, string cidadeEstado)
        {
            // 1. Validar se os campos obrigatórios vieram preenchidos
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                ViewBag.Erro = "Por favor, preencha todos os campos obrigatórios (Nome, E-mail e Senha).";
                return View();
            }

            // 2. Verificar se o e-mail já não está cadastrado no banco para não duplicar
            var emailExiste = _context.Usuarios.Any(u => u.Email == email);
            if (emailExiste)
            {
                ViewBag.Erro = "Este e-mail já está cadastrado! Tente fazer o login.";
                return View();
            }

            // 3. Criar o objeto Usuario com os dados recebidos da tela
            var novoUsuario = new Usuario
            {
                Nome = nome,
                Email = email,
                Cep = cep,
                Numero = numero,
                Endereco = endereco,
                Bairro = bairro,
                CidadeEstado = cidadeEstado
            };

            // Salva a senha embaralhada, nunca a original
            novoUsuario.Senha = _hasher.HashPassword(novoUsuario, senha);

            // 4. Mágica do Entity Framework: Adiciona e salva no MySQL
            _context.Usuarios.Add(novoUsuario);
            _context.SaveChanges(); // É aqui que o comando vira um "INSERT" lá no banco!

            // Criamos a mensagem de sucesso que vai aparecer na tela de Login
            TempData["MensagemSucesso"] = "Conta criada com sucesso! Faça seu login abaixo com seus dados reais. ✨";

            // Redireciona na hora para a tela de Login
            return RedirectToAction("Login");
        }

        public IActionResult FinalizarKit(string caixa, double preco)
        {
            // Passamos os dados básicos do kit montado para a tela final
            ViewBag.Caixa = string.IsNullOrEmpty(caixa) ? "Caixa Clássica Bella" : caixa;
            ViewBag.Preco = preco == 0 ? 25.00 : preco;
            return View();
        }

        public IActionResult Favoritos()
        {
            // Simulação: se 'true', mostra os produtos favoritados. Se 'false', mostra a tela de lista vazia!
            ViewBag.TemFavoritos = true;
            return View();
        }

        // Agora a rota aceita os dados do produto clicado!
        public IActionResult Detalhes(string nome, string preco, string cheiro, string imagem)
        {
            // Passamos os dados para a View usando a ViewBag
            ViewBag.Nome = nome ?? "Essência Gold Bella";
            ViewBag.Preco = preco ?? "R$ 280,00";
            ViewBag.Cheiro = cheiro ?? "Âmbar - Baunilha - Marcante";
            ViewBag.Imagem = imagem ?? "https://images.unsplash.com/photo-1594035910387-fea47794261f?auto=format&fit=crop&w=800&q=80";

            return View();
        }

        public IActionResult Pagamento(string caixa, double preco)
        {
            // Simulação: mude para 'true' para testar como fica o endereço quando o usuário já está logado!
            ViewBag.UsuarioLogado = false;

            ViewBag.Caixa = string.IsNullOrEmpty(caixa) ? "Caixa Clássica Bella" : caixa;
            ViewBag.Preco = preco == 0 ? 25.00 : preco;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Logout()
        {
            // Limpa todas as informações que guardamos na sessão do usuário
            HttpContext.Session.Clear();

            // Redireciona o usuário de volta para a página inicial (já deslogado!)
            return RedirectToAction("Index");
        }
    }
}
