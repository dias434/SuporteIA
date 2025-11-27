using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;

namespace SuporteIA.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _nome = string.Empty;
        private string _email = string.Empty;
        private string _senha = string.Empty;

        public string Nome
        {
            get => _nome;
            set
            {
                _nome = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Senha
        {
            get => _senha;
            set
            {
                _senha = value;
                OnPropertyChanged();
            }
        }

        public Command LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new Command(ExecuteLogin);
            
            Nome = "Dias";
            Email = "dias@gmail.com";
            Senha = "1234";
        }

        private async void ExecuteLogin()
        {
            if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
            {
                var currentApp = Application.Current;
                if (currentApp?.Windows.Count > 0)
                {
                    var page = currentApp.Windows[0].Page;
                    if (page != null)
                    {
                        await page.DisplayAlertAsync("Erro", "Preencha todos os campos", "OK");
                    }
                }
                return;
            }

            if (Nome.ToLower() == "dias" && Email.ToLower() == "dias@gmail.com" && Senha == "1234")
            {
                LoginSucesso?.Invoke();
            }
            else
            {
                var currentApp = Application.Current;
                if (currentApp?.Windows.Count > 0)
                {
                    var page = currentApp.Windows[0].Page;
                    if (page != null)
                    {
                        await page.DisplayAlertAsync("Erro", "Credenciais inválidas", "OK");
                    }
                }
            }
        }

        public event Action? LoginSucesso;
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}