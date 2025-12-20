namespace NoteTakingApp.Models;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class Note : ObservableObject
{
    [ObservableProperty]
    private string _filename = string.Empty;
    [ObservableProperty]
    private string _text = string.Empty;
    [ObservableProperty]
    private DateTime _date = DateTime.Now;
    [ObservableProperty]
    private bool _isCompleted;
}
