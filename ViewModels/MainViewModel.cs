using System.Collections.ObjectModel;
using System.Windows.Input;
using KartinMauiApp_Karting_OCRgApp.ViewModels;
using MauiApp_Karting_OCR.Factory;
using MauiApp_Karting_OCR.Models;
using MauiApp_Karting_OCR.Pages;
using MauiApp_Karting_OCR.Services;

namespace MauiApp_Karting_OCR.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly CircuitService _circuitService;
    private readonly OcrScanViewModel _ocrScanViewModel;
    private bool _isRefreshing;

    public ObservableCollection<Circuit> Circuits { get; set; }
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }
    
    public ICommand NavigateToCircuitDetailsCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand AddCircuitCommand { get; }
    public ICommand NavigateToCameraCommand { get; }

    public MainViewModel()
    {
        _circuitService = IPlatformApplication.Current.Services.GetService<CircuitService>();
        this.Circuits = new ObservableCollection<Circuit>();

        RefreshCommand = new Command(async () => await LoadCircuitsAsync());
        AddCircuitCommand = new Command(async () => await AddCircuitAsync());
        NavigateToCameraCommand = new Command(NavigateToCameraAsync);
        NavigateToCircuitDetailsCommand = new Command<object>(async (object parameter) => await NavigateToCircuitDetailsAsync(parameter));

        this._ocrScanViewModel = IPlatformApplication.Current.Services.GetService<OcrScanViewModel>();
        this._ocrScanViewModel.PropertyChanged += OcrScanViewModel_PropertyChanged;

        this.LoadCircuitsAsync();
    }

    private void OcrScanViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (this._ocrScanViewModel.IsDataKeepIt)
        {
            this.Circuits.Clear();
            foreach(Circuit circuit in this._circuitService.Circuits)
            {
                this.Circuits.Add(circuit);
            }
            this._ocrScanViewModel.IsDataKeepIt = false;
        }
    }

    /// <summary>
    /// Este metodo hace navegar desde
    /// la pagina que se encuentran hasta
    /// la pagina de la camara
    /// </summary>
    private async void NavigateToCameraAsync()
    {
        await (Shell.Current as Page).Navigation.PushAsync(IPlatformApplication.Current.Services.GetService<OcrScanPage>());
    }
    /// <summary>
    /// Este método se encarga de navegar desde la 
    /// página principal a la página detalle del
    /// circuito
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    private async Task NavigateToCircuitDetailsAsync(object parameter)
    {
        await (Shell.Current as Page).Navigation.PushAsync(CircuitDetailPageFactory.Create(parameter as Circuit));
    }
    /// <summary>
    /// Este método se encarga de cargar
    /// todos los circuitos registrados
    /// en la aplicación por el usuario.
    /// </summary>
    /// <returns></returns>
    public async Task LoadCircuitsAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        IsRefreshing = true;

        try
        {
            Circuits.Clear();
            this._circuitService.Circuits = await _circuitService.GetCircuitsAsync();
            foreach (var circuit in this._circuitService.Circuits)
            {
                Circuits.Add(circuit);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar los circuitos: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }
    /// <summary>
    /// Este método se encarga de añadir un
    /// circuito en caso de que no haya ninguno
    /// cargado
    /// </summary>
    /// <returns></returns>
    private async Task AddCircuitAsync()
    {
        string name = await Shell.Current.DisplayPromptAsync("Nuevo Circuito", "Nombre del circuito:", "Guardar", "Cancelar");

        if (!string.IsNullOrWhiteSpace(name))
        {
            var newCircuit = new Circuit
            {
                Name = name
            };

            await _circuitService.SaveCircuitAsync(newCircuit);
        }
    }
}