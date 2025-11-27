using Microsoft.Maui.Controls;
using SuporteIA.Models;
using SuporteIA.Services;

namespace SuporteIA.Views
{
    public partial class NovoTicketPage : ContentPage
    {
        private string _categoriaSelecionada = string.Empty;
        private readonly ILocalDataService _localDataService;

        public NovoTicketPage()
        {
            InitializeComponent();
            _localDataService = new LocalDataService();
        }

        private async void OnAbrirOpcoesCategoria(object sender, EventArgs e)
        {
            var opcoes = new string[] { "Dúvidas", "Suporte Desktop", "Suporte Web", "Suporte Mobile", "Solicitação" };
            
            var resultado = await DisplayActionSheet("Selecione uma categoria", "Cancelar", null, opcoes);
            
            if (!string.IsNullOrEmpty(resultado) && resultado != "Cancelar")
            {
                _categoriaSelecionada = resultado;
                CategoriaSelecionadaLabel.Text = resultado;
                CategoriaSelecionadaLabel.TextColor = Color.FromArgb("#212529");
            }
        }

        private async void OnEnviarTicketClicked(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(_categoriaSelecionada) || CategoriaSelecionadaLabel.Text == "Selecione uma categoria")
            {
                await DisplayAlert("Atenção", "Selecione uma categoria", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(AssuntoEntry.Text) ||
                string.IsNullOrWhiteSpace(DescricaoEditor.Text))
            {
                await DisplayAlert("Atenção", "Preencha todos os campos obrigatórios", "OK");
                return;
            }

            try
            {
                var novoChamado = new Chamado
                {
                    Titulo = AssuntoEntry.Text,
                    Descricao = DescricaoEditor.Text,
                    Categoria = _categoriaSelecionada,
                    Status = "Aberto",
                    UsuarioId = 1,
                    DataAbertura = DateTime.Now,
                    Prioridade = "Média"
                };

                var chamadoCriado = await _localDataService.CriarChamadoAsync(novoChamado);

                if (chamadoCriado != null)
                {
                    await DisplayAlert("Sucesso", "Chamado criado com sucesso!", "OK");
                    
                    // Atualizar dashboard
                    await AtualizarDashboard();
                    
                    // Limpar formulário
                    AssuntoEntry.Text = string.Empty;
                    _categoriaSelecionada = string.Empty;
                    CategoriaSelecionadaLabel.Text = "Selecione uma categoria";
                    CategoriaSelecionadaLabel.TextColor = Color.FromArgb("#6C757D");
                    DescricaoEditor.Text = string.Empty;
                    
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Erro", "Falha ao criar chamado", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Erro ao criar chamado: {ex.Message}", "OK");
            }
        }

        private async Task AtualizarDashboard()
        {
            try
            {
                Console.WriteLine("🔄 [NovoTicket] Atualizando dashboard...");
                
                foreach (var page in Navigation.NavigationStack)
                {
                    if (page is DashboardPage dashboardPage)
                    {
                        Console.WriteLine("✅ [NovoTicket] DashboardPage encontrada, atualizando dados...");
                        await Task.Delay(300);
                        await dashboardPage.CarregarDadosDashboard();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ [NovoTicket] Erro ao atualizar dashboard: {ex.Message}");
            }
        }

        private async void OnCancelarClicked(object sender, System.EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnAnexarArquivoClicked(object sender, System.EventArgs e)
        {
            await DisplayAlert("Anexo", "Funcionalidade em desenvolvimento", "OK");
        }
    }
}