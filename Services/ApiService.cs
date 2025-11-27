using System.Text;
using System.Text.Json;
using SuporteIA.Models;

namespace SuporteIA.Services
{
    public interface IApiService
    {
        Task<UsuarioResposta?> LoginAsync(string email, string password);
        Task<List<Chamado>?> GetChamadosAsync();
        Task<List<Categoria>?> GetCategoriasAsync();
        Task<Chamado?> CriarChamadoAsync(Chamado chamado);
        Task<bool> HealthCheckAsync();
        Task<bool> AtualizarChamadoAsync(Chamado chamado);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        private const string BaseUrl = "http://192.168.100.12:5059/api";

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<bool> HealthCheckAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/Usuario/Health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<UsuarioResposta?> LoginAsync(string email, string password)
        {
            try
            {
                Console.WriteLine($"🔍 Tentando login na API: {BaseUrl}/Usuario/Login");

                var loginRequest = new UsuarioLogin { Email = email, Password = password };
                var json = JsonSerializer.Serialize(loginRequest, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{BaseUrl}/Usuario/Login", content);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📡 Status Code: {(int)response.StatusCode} {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);
                    
                    if (loginResponse?.Success == true)
                    {
                        return new UsuarioResposta
                        {
                            UserId = loginResponse.UserId,
                            Name = loginResponse.Name,
                            Email = loginResponse.Email,
                            UserType = loginResponse.UserType
                        };
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Erro no login: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Chamado>?> GetChamadosAsync()
        {
            try
            {
                Console.WriteLine($"🔍 Buscando chamados na API: {BaseUrl}/Chamado");
                
                var response = await _httpClient.GetAsync($"{BaseUrl}/Chamado");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var chamados = JsonSerializer.Deserialize<List<Chamado>>(content, _jsonOptions);
                    
                    if (chamados != null)
                    {
                        Console.WriteLine($"✅ Encontrados {chamados.Count} chamados");
                    }
                    
                    return chamados;
                }
                else
                {
                    Console.WriteLine($"❌ Erro ao buscar chamados: {response.StatusCode}");
                }
                
                return new List<Chamado>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Erro ao buscar chamados: {ex.Message}");
                return new List<Chamado>();
            }
        }

        public async Task<List<Categoria>?> GetCategoriasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/Categoria");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Categoria>>(content, _jsonOptions);
                }
                
                return new List<Categoria>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar categorias: {ex.Message}");
                return new List<Categoria>();
            }
        }

        public async Task<Chamado?> CriarChamadoAsync(Chamado chamado)
        {
            try
            {
                Console.WriteLine($"🔍 Tentando criar chamado na API: {BaseUrl}/Chamado");
                
                var chamadoRequest = new
                {
                    titulo = chamado.Titulo,
                    descricao = chamado.Descricao,
                    clienteId = 1,
                    categoriaId = ConverterCategoriaParaId(chamado.Categoria),
                    prioridade = "Média",
                    status = "Aberto"
                };

                var json = JsonSerializer.Serialize(chamadoRequest, _jsonOptions);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{BaseUrl}/Chamado", content);
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📡 Status Code: {(int)response.StatusCode} {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    chamado.Status = "Aberto";
                    chamado.DataAbertura = DateTime.Now;
                    chamado.Prioridade = "Média";
                    
                    Console.WriteLine($"✅ Chamado criado com sucesso");
                    return chamado;
                }
                else
                {
                    Console.WriteLine($"❌ Erro HTTP ao criar chamado: {response.StatusCode}");
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Erro ao criar chamado: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AtualizarChamadoAsync(Chamado chamado)
        {
            try
            {
                Console.WriteLine($"🚨 [URGENTE] ATUALIZANDO CHAMADO {chamado.ChamadoId} PARA STATUS: {chamado.Status}");

                // SOLUÇÃO DEFINITIVA: Testar múltiplos formatos
                var resultados = new List<bool>();

                // TENTATIVA 1: Formato mínimo (apenas status)
                Console.WriteLine("🎯 TENTATIVA 1: Formato mínimo (apenas status)");
                var tentativa1 = new { status = chamado.Status };
                resultados.Add(await TentarAtualizacao($"{BaseUrl}/Chamado/{chamado.ChamadoId}", tentativa1, "PUT"));

                if (resultados.Any(r => r)) 
                {
                    Console.WriteLine("✅ SUCESSO via Tentativa 1");
                    return true;
                }

                // TENTATIVA 2: Formato completo
                Console.WriteLine("🎯 TENTATIVA 2: Formato completo");
                var tentativa2 = new
                {
                    chamadoId = chamado.ChamadoId,
                    titulo = chamado.Titulo,
                    descricao = chamado.Descricao,
                    status = chamado.Status,
                    categoria = chamado.Categoria,
                    prioridade = chamado.Prioridade,
                    dataAbertura = chamado.DataAbertura.ToString("yyyy-MM-ddTHH:mm:ss"),
                    usuarioId = chamado.UsuarioId
                };
                resultados.Add(await TentarAtualizacao($"{BaseUrl}/Chamado/{chamado.ChamadoId}", tentativa2, "PUT"));

                if (resultados.Any(r => r)) 
                {
                    Console.WriteLine("✅ SUCESSO via Tentativa 2");
                    return true;
                }

                // TENTATIVA 3: Endpoint específico de status
                Console.WriteLine("🎯 TENTATIVA 3: Endpoint específico de status");
                var tentativa3 = new { status = chamado.Status };
                resultados.Add(await TentarAtualizacao($"{BaseUrl}/Chamado/{chamado.ChamadoId}/status", tentativa3, "PUT"));

                if (resultados.Any(r => r)) 
                {
                    Console.WriteLine("✅ SUCESSO via Tentativa 3");
                    return true;
                }

                // TENTATIVA 4: PATCH
                Console.WriteLine("🎯 TENTATIVA 4: Método PATCH");
                var tentativa4 = new { status = chamado.Status };
                resultados.Add(await TentarAtualizacao($"{BaseUrl}/Chamado/{chamado.ChamadoId}", tentativa4, "PATCH"));

                if (resultados.Any(r => r)) 
                {
                    Console.WriteLine("✅ SUCESSO via Tentativa 4");
                    return true;
                }

                // TENTATIVA 5: POST para atualizar
                Console.WriteLine("🎯 TENTATIVA 5: POST para atualizar");
                var tentativa5 = new { status = chamado.Status };
                resultados.Add(await TentarAtualizacao($"{BaseUrl}/Chamado/atualizar-status", tentativa5, "POST"));

                Console.WriteLine($"❌ TODAS AS TENTATIVAS FALHARAM");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERRO CRÍTICO: {ex.Message}");
                return false;
            }
        }

        // MÉTODO AUXILIAR PARA TENTAR ATUALIZAÇÃO
        private async Task<bool> TentarAtualizacao(string url, object data, string method)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, _jsonOptions);
                Console.WriteLine($"📤 {method} {url}");
                Console.WriteLine($"📦 JSON: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                HttpResponseMessage response;
                
                if (method == "PUT")
                {
                    response = await _httpClient.PutAsync(url, content);
                }
                else if (method == "PATCH")
                {
                    var request = new HttpRequestMessage(new HttpMethod("PATCH"), url) { Content = content };
                    response = await _httpClient.SendAsync(request);
                }
                else if (method == "POST")
                {
                    response = await _httpClient.PostAsync(url, content);
                }
                else
                {
                    return false;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📡 Status: {(int)response.StatusCode} {response.StatusCode}");
                Console.WriteLine($"📨 Resposta: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ {method} BEM-SUCEDIDO");
                    return true;
                }
                
                Console.WriteLine($"❌ {method} FALHOU: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERRO em {method}: {ex.Message}");
                return false;
            }
        }

        private int ConverterCategoriaParaId(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
                return 1;

            return categoria.ToLower() switch
            {
                "dúvidas" or "duvidas" => 1,
                "suporte desktop" => 2,
                "suporte web" => 3,
                "suporte mobile" => 4,
                "solicitação" or "solicitacao" => 5,
                _ => 1
            };
        }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
