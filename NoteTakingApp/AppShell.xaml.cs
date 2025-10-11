using NoteTakingApp.Views;
namespace NoteTakingApp

{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(NotePage), typeof(NotePage));
        }
    }
}
