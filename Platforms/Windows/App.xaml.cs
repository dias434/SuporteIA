using Microsoft.UI.Xaml;

namespace SuporteIA.WinUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Microsoft.Maui.Essentials.Platform.OnLaunched(args);
            CreateMainWindow();
        }

        private void CreateMainWindow()
        {
            var window = new Microsoft.UI.Xaml.Window();
            window.Activate();
        }
    }
}