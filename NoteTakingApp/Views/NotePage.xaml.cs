using NoteTakingApp.ViewModels;

namespace NoteTakingApp.Views;

public partial class NotePage : ContentPage
{
    // The NotePage no longer has any logic. It just connects to its ViewModel.
    public NotePage(NoteViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
