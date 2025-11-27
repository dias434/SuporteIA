using Microsoft.Maui.Controls;
using SuporteIA.Services;
using SuporteIA.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SuporteIA.Views
{
    public partial class MeusTicketsPage : ContentPage, INotifyPropertyChanged
    {
        private readonly ILocalDataService _localDataService;
        public ObservableCollection<ChamadoDisplay> Chamados { get; set; }

        public new event PropertyChangedEventHandler? PropertyChanged;

        public MeusTicketsPage()
        {
            InitializeComponent();
            _localDataService = new LocalDataService();
            Chamados = new ObservableCollection<ChamadoDisplay>();
            BindingContext = this;
            
            LoadChamados();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadChamados();
        }

        private async void LoadChamados()
        {
            try
            {
                var chamados = await _localDataService.GetChamadosAsync();
                var chamadosConcluidos = await _localDataService.GetChamadosConcluidosAsync();
                
                if (chamados != null)
                {
                    Chamados.Clear();
                    foreach (var chamado in chamados)
                    {
                        var foiConcluidoLocalmente = chamadosConcluidos.Contains(chamado.ChamadoId);
                        var chamadoDisplay = new ChamadoDisplay(chamado, foiConcluidoLocalmente);
                        Chamados.Add(chamadoDisplay);
                    }
                    
                    TicketsCollectionView.ItemsSource = Chamados;
                    Console.WriteLine($"📋 Carregados {Chamados.Count} chamados ({chamadosConcluidos.Count} concluídos)");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao carregar chamados: {ex.Message}", "OK");
            }
        }

        private async void OnVerChamadoClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var chamadoDisplay = (ChamadoDisplay)button.CommandParameter;
            
            var chamado = new Chamado
            {
                ChamadoId = chamadoDisplay.ChamadoId,
                Titulo = chamadoDisplay.Titulo,
                Descricao = chamadoDisplay.Descricao,
                Categoria = chamadoDisplay.Categoria,
                Status = chamadoDisplay.StatusOriginal,
                DataAbertura = chamadoDisplay.DataAbertura,
                UsuarioId = 1
            };
            
            await Navigation.PushAsync(new DetalhesChamadoPage(chamado));
        }

        protected new virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ChamadoDisplay
    {
        public int ChamadoId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string StatusOriginal { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusDisplay { get; set; } = string.Empty;
        public Color StatusColor { get; set; }
        public DateTime DataAbertura { get; set; }

        public ChamadoDisplay(Chamado chamado, bool foiConcluidoLocalmente = false)
        {
            ChamadoId = chamado.ChamadoId;
            Titulo = chamado.Titulo;
            Descricao = chamado.Descricao;
            Categoria = chamado.Categoria;
            StatusOriginal = chamado.Status;
            DataAbertura = chamado.DataAbertura;
            
            if (foiConcluidoLocalmente)
            {
                Status = "Concluído";
                StatusDisplay = "Concluído";
                StatusColor = Color.FromArgb("#28A745");
            }
            else
            {
                Status = chamado.Status;
                StatusDisplay = (chamado.Status == "Concluído" || chamado.Status == "Fechado" || chamado.Status == "Resolvido") 
                    ? "Concluído" 
                    : "Em Aberto";
                
                StatusColor = StatusDisplay switch
                {
                    "Em Aberto" => Color.FromArgb("#007BFF"),
                    "Concluído" => Color.FromArgb("#28A745"),
                    _ => Color.FromArgb("#6C757D")
                };
            }

            Console.WriteLine($"📋 Chamado {ChamadoId}: StatusAPI='{StatusOriginal}', Local={foiConcluidoLocalmente}, Exibição='{StatusDisplay}'");
        }
    }
}