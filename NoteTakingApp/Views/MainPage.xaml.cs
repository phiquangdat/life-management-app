using NoteTakingApp.ViewModels;

namespace NoteTakingApp.Views;

public partial class MainPage : ContentPage
{
    public MainPage(AllNotesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
