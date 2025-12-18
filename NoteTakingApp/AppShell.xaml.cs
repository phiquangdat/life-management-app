using NoteTakingApp.Views;
namespace NoteTakingApp

{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(AllNotesPage), typeof(AllNotesPage));
            Routing.RegisterRoute(nameof(NotePage), typeof(NotePage));
            Routing.RegisterRoute(nameof(NoteDetailsPage), typeof(NoteDetailsPage));
            Routing.RegisterRoute(nameof(ToDoListPage), typeof(ToDoListPage));
            Routing.RegisterRoute(nameof(AddTaskPage), typeof(AddTaskPage));
        }
    }
}
