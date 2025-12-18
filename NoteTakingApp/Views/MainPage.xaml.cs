using NoteTakingApp.ViewModels;

namespace NoteTakingApp.Views;

public partial class MainPage : ContentPage
{
    public MainPage(AllNotesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AllNotesViewModel vm)
        {
            vm.LoadNotesCommand.Execute(null);
        }
    }
}
