using Microsoft.Maui.Storage;
using SuporteIA.Models;
using System.Text.Json;

namespace SuporteIA.Services
{
    public interface ILocalDataService
    {
        Task<bool> LoginAsync(string email, string senha);
        Task<List<Chamado>> GetChamadosAsync();
        Task<Chamado> CriarChamadoAsync(Chamado chamado);
        Task<List<int>> GetChamadosConcluidosAsync();
        Task SalvarChamadoConcluidoAsync(int chamadoId);
        Task<bool> UsuarioEstaLogadoAsync();
        Task<Usuario> GetUsuarioLogadoAsync();
    }

    public class LocalDataService : ILocalDataService
    {
        private const string CHAMADOS_KEY = "chamados_data";
        private const string CHAMADOS_CONCLUIDOS_KEY = "chamados_concluidos";
        private const string USUARIO_LOGADO_KEY = "usuario_logado";

        public async Task<bool> LoginAsync(string email, string senha)
        {
            // Usuário fixo para demonstração
            if (email == "dias@gmail.com" && senha == "dias")
            {
                var usuario = new Usuario
                {
                    Id = 1,
                    Name = "Dias",
                    Email = email
                };

                var usuarioJson = JsonSerializer.Serialize(usuario);
                await SecureStorage.SetAsync(USUARIO_LOGADO_KEY, usuarioJson);
                
                // Criar alguns chamados de exemplo se não existirem
                await InicializarChamadosExemplo();
                
                return true;
            }
            return false;
        }

        public async Task<List<Chamado>> GetChamadosAsync()
        {
            try
            {
                var chamadosJson = Preferences.Get(CHAMADOS_KEY, string.Empty);
                if (!string.IsNullOrEmpty(chamadosJson))
                {
                    return JsonSerializer.Deserialize<List<Chamado>>(chamadosJson) ?? new List<Chamado>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar chamados: {ex.Message}");
            }
            return new List<Chamado>();
        }

        public async Task<Chamado> CriarChamadoAsync(Chamado chamado)
        {
            var chamados = await GetChamadosAsync();
            
            // Gerar ID único
            chamado.ChamadoId = chamados.Count > 0 ? chamados.Max(c => c.ChamadoId) + 1 : 1;
            chamado.DataAbertura = DateTime.Now;
            chamado.Status = "Aberto";
            chamado.UsuarioId = 1;

            chamados.Add(chamado);
            
            var chamadosJson = JsonSerializer.Serialize(chamados);
            Preferences.Set(CHAMADOS_KEY, chamadosJson);
            
            return chamado;
        }

        public async Task<List<int>> GetChamadosConcluidosAsync()
        {
            try
            {
                var chamadosJson = await SecureStorage.GetAsync(CHAMADOS_CONCLUIDOS_KEY);
                if (!string.IsNullOrEmpty(chamadosJson))
                {
                    return JsonSerializer.Deserialize<List<int>>(chamadosJson) ?? new List<int>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar chamados concluídos: {ex.Message}");
            }
            return new List<int>();
        }

        public async Task SalvarChamadoConcluidoAsync(int chamadoId)
        {
            var chamadosConcluidos = await GetChamadosConcluidosAsync();
            
            if (!chamadosConcluidos.Contains(chamadoId))
            {
                chamadosConcluidos.Add(chamadoId);
                var novaListaJson = JsonSerializer.Serialize(chamadosConcluidos);
                await SecureStorage.SetAsync(CHAMADOS_CONCLUIDOS_KEY, novaListaJson);
            }
        }

        public async Task<bool> UsuarioEstaLogadoAsync()
        {
            var usuarioJson = await SecureStorage.GetAsync(USUARIO_LOGADO_KEY);
            return !string.IsNullOrEmpty(usuarioJson);
        }

        public async Task<Usuario> GetUsuarioLogadoAsync()
        {
            var usuarioJson = await SecureStorage.GetAsync(USUARIO_LOGADO_KEY);
            if (!string.IsNullOrEmpty(usuarioJson))
            {
                return JsonSerializer.Deserialize<Usuario>(usuarioJson);
            }
            return null;
        }

        private async Task InicializarChamadosExemplo()
        {
            var chamadosExistentes = await GetChamadosAsync();
            if (chamadosExistentes.Count == 0)
            {
                var chamadosExemplo = new List<Chamado>
                {
                    new Chamado
                    {
                        ChamadoId = 1,
                        Titulo = "Não consigo fazer login no sistema",
                        Descricao = "Quando tento acessar o sistema com minhas credenciais, recebo uma mensagem de erro de autenticação. Já verifiquei que o email e senha estão corretos.",
                        Categoria = "Suporte Web",
                        Status = "Aberto",
                        DataAbertura = DateTime.Now.AddDays(-2),
                        UsuarioId = 1,
                        Prioridade = "Alta"
                    },
                    new Chamado
                    {
                        ChamadoId = 2,
                        Titulo = "Problema com impressora de rede",
                        Descricao = "A impressora HP LaserJet não está respondendo quando tento imprimir documentos. Já reiniciei a impressora e verifiquei as conexões.",
                        Categoria = "Suporte Desktop",
                        Status = "Aberto",
                        DataAbertura = DateTime.Now.AddDays(-1),
                        UsuarioId = 1,
                        Prioridade = "Média"
                    }
                };

                var chamadosJson = JsonSerializer.Serialize(chamadosExemplo);
                Preferences.Set(CHAMADOS_KEY, chamadosJson);
            }
        }
    }
}