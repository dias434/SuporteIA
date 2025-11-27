using Microsoft.Maui.Controls;

namespace SuporteIA.Views
{
    public partial class ConfiguracoesPage : ContentPage
    {
        public ConfiguracoesPage()
        {
            InitializeComponent();
        }

        private async void OnSairClicked(object sender, System.EventArgs e)
        {
            bool confirmar = await DisplayAlertAsync("Sair", "Deseja realmente sair da sua conta?", "Sim", "Cancelar");
            if (confirmar)
            {
                if (Application.Current?.Windows.Count > 0)
                {
                    Application.Current.Windows[0].Page = new LoginPage();
                }
            }
        }
    }
}