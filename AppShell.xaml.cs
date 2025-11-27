using Microsoft.Maui.Controls;

namespace SuporteIA
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        private void RegisterRoutes()
        {
            Routing.RegisterRoute("novoticket", typeof(Views.NovoTicketPage));
            Routing.RegisterRoute("meustickets", typeof(Views.MeusTicketsPage));
            Routing.RegisterRoute("detalheschamado", typeof(Views.DetalhesChamadoPage));
            Routing.RegisterRoute("dashboard", typeof(Views.DashboardPage));
            Routing.RegisterRoute("configuracoes", typeof(Views.ConfiguracoesPage));
        }

        private async void OnSairClicked(object sender, EventArgs e)
        {
            bool confirmar = await DisplayAlert("Sair", "Deseja realmente sair da sua conta?", "Sim", "Cancelar");
            if (confirmar)
            {
                // Limpar dados de sessão
                Preferences.Clear();
                SecureStorage.Remove("usuario_logado");
                
                if (Application.Current?.Windows.Count > 0)
                {
                    Application.Current.Windows[0].Page = new Views.LoginPage();
                }
            }
        }
    }
}