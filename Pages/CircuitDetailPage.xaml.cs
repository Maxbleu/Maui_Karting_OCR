using MauiApp_Karting_OCR.Models;
using MauiApp_Karting_OCR.ViewModels;

namespace MauiApp_Karting_OCR.Pages;

public partial class CircuitDetailPage : ContentPage
{
    private CircuitDetailViewModel _viewModel;

    public CircuitDetailPage(Circuit circuit)
    {
        InitializeComponent();
        _viewModel = new CircuitDetailViewModel(circuit);
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadLapTimesAsync();
    }
}