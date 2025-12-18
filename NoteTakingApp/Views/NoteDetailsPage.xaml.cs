using NoteTakingApp.ViewModels;

namespace NoteTakingApp.Views;

public partial class NoteDetailsPage : ContentPage
{
	public NoteDetailsPage(NoteDetailsViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
