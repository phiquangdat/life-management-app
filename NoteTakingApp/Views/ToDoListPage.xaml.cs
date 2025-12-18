using NoteTakingApp.ViewModels;

namespace NoteTakingApp.Views;

public partial class ToDoListPage : ContentPage
{
	public ToDoListPage(ToDoListViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ToDoListViewModel vm)
        {
            vm.LoadTasksCommand.Execute(null);
        }
    }
}
