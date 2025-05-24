using MauiApp_Karting_OCR.ViewModels;

namespace MauiApp_Karting_OCR.Pages;

public partial class MainPage : ContentPage
{
    private MainViewModel _viewModel;

    public MainPage(MainViewModel mainViewModel)
    {
        InitializeComponent();
        _viewModel = mainViewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCircuitsAsync();
    }
}