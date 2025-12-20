using NoteTakingApp.ViewModels;

namespace NoteTakingApp.Views;

public partial class TaskDetailsPage : ContentPage
{
	public TaskDetailsPage(TaskDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
