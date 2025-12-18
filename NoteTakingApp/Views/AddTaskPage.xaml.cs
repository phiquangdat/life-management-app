using NoteTakingApp.ViewModels;

namespace NoteTakingApp.Views;

public partial class AddTaskPage : ContentPage
{
	public AddTaskPage(AddTaskViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
