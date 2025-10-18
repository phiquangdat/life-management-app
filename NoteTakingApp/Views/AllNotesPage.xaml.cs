using NoteTakingApp.ViewModels;

namespace NoteTakingApp.Views;

public partial class AllNotesPage : ContentPage
{
    private readonly AllNotesViewModel _viewModel;

    // The ViewModel is now passed into the constructor (Dependency Injection).
    public AllNotesPage(AllNotesViewModel viewModel)
    {
        InitializeComponent();

        // The BindingContext is set to the ViewModel, so the XAML can find the commands and data.
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // We tell the ViewModel to load the notes every time the page appears.
        _viewModel.LoadNotesCommand.Execute(null);
    }
}
