using Microsoft.Maui.Controls;
using SuporteIA.Services;
using System.ComponentModel;

namespace SuporteIA.Views
{
    public partial class DashboardPage : ContentPage, INotifyPropertyChanged
    {
        private readonly ILocalDataService _localDataService;
        
        private int _ticketsAbertos;
        private int _ticketsConcluidos;

        public int TicketsAbertos
        {
            get => _ticketsAbertos;
            set
            {
                _ticketsAbertos = value;
                OnPropertyChanged(nameof(TicketsAbertos));
            }
        }

        public int TicketsConcluidos
        {
            get => _ticketsConcluidos;
            set
            {
                _ticketsConcluidos = value;
                OnPropertyChanged(nameof(TicketsConcluidos));
            }
        }

        public new event PropertyChangedEventHandler? PropertyChanged;

        public DashboardPage()
        {
            InitializeComponent();
            _localDataService = new LocalDataService();
            BindingContext = this;
            
            _ = LoadDashboardData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _ = LoadDashboardData();
        }

        public async Task CarregarDadosDashboard()
        {
            await LoadDashboardData();
        }

        private async Task LoadDashboardData()
        {
            try
            {
                var chamados = await _localDataService.GetChamadosAsync();
                var chamadosConcluidos = await _localDataService.GetChamadosConcluidosAsync();
                
                Console.WriteLine($"🔍 DASHBOARD - Chamados: {chamados?.Count ?? 0}");
                Console.WriteLine($"🔍 DASHBOARD - Concluídos: {chamadosConcluidos.Count}");

                if (chamados != null && chamados.Any())
                {
                    var totalAbertos = 0;
                    var totalConcluidos = 0;

                    foreach (var chamado in chamados)
                    {
                        var foiConcluidoLocalmente = chamadosConcluidos.Contains(chamado.ChamadoId);
                        
                        if (foiConcluidoLocalmente)
                        {
                            totalConcluidos++;
                        }
                        else
                        {
                            totalAbertos++;
                        }
                    }

                    TicketsAbertos = totalAbertos;
                    TicketsConcluidos = totalConcluidos;
                    
                    Console.WriteLine($"📊 DASHBOARD FINAL: {TicketsAbertos} ABERTOS, {TicketsConcluidos} CONCLUÍDOS");
                }
                else
                {
                    TicketsAbertos = 0;
                    TicketsConcluidos = chamadosConcluidos.Count;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro: {ex.Message}");
                TicketsAbertos = 0;
                TicketsConcluidos = 0;
            }
        }

        private async void OnNovoTicketClicked(object sender, System.EventArgs e)
        {
            await Shell.Current.GoToAsync("novoticket");
        }

        protected new virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}