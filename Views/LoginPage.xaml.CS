using SuporteIA.Services;

namespace SuporteIA.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly ILocalDataService _localDataService;

        public LoginPage()
        {
            InitializeComponent();
            _localDataService = new LocalDataService();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string senha = SenhaEntry.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                await DisplayAlert("Erro", "Por favor, preencha email e senha", "OK");
                return;
            }

            try
            {
                var loginSucesso = await _localDataService.LoginAsync(email, senha);

                if (loginSucesso)
                {
                    await DisplayAlert("Sucesso", $"Bem-vindo, Dias!", "OK");
                    
                    if (Application.Current?.Windows.Count > 0)
                    {
                        Application.Current.Windows[0].Page = new AppShell();
                    }
                }
                else
                {
                    await DisplayAlert("Erro", "Credenciais inválidas", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha no login: {ex.Message}", "OK");
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Preencher automaticamente para testes
            EmailEntry.Text = "dias@gmail.com";
            SenhaEntry.Text = "dias";
        }
    }
}