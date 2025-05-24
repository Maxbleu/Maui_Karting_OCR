using System.Globalization;
using System.Windows.Input;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using MauiApp_Karting_OCR.Models;
using MauiApp_Karting_OCR.Services;
using MauiApp_Karting_OCR.Utils;
using MauiApp_Karting_OCR.ViewModels;

namespace KartinMauiApp_Karting_OCRgApp.ViewModels;

public class OcrScanViewModel : BaseViewModel
{
    private readonly OcrService _ocrService;
    private readonly CircuitService _circuitService;
    private readonly LapTimeService _lapTimeService;
    
    private ImageSource _previewImageSource;
    private FileResult _capturedImage;
    private AnalyzeResult _analyzeResult;

    private bool _isProcessing;
    private bool _hasDetectedTime;
    private bool _isDataKeppIt = false;

    public AnalyzeResult AnalyzeResult
    {
        get { return _analyzeResult; }
        set
        {
            if(_analyzeResult != value)
            {
                _analyzeResult = value;
                OnPropertyChanged();
            }
        }
    }
    public ImageSource PreviewImageSource
    {
        get => _previewImageSource;
        set => SetProperty(ref _previewImageSource, value);
    }

    public bool IsDataKeepIt
    {
        get => this._isDataKeppIt;
        set
        {
            if(this._isDataKeppIt != value)
            {
                this._isDataKeppIt = value;
                OnPropertyChanged();
            }
        }
    }
    public bool IsProcessing
    {
        get => _isProcessing;
        set => SetProperty(ref _isProcessing, value);
    }
    public bool HasDetectedTime
    {
        get => _hasDetectedTime;
        set => SetProperty(ref _hasDetectedTime, value);
    }

    public ICommand TakePhotoCommand { get; }
    public ICommand PickImageCommand { get; }
    public ICommand SaveTimeCommand { get; }

    public OcrScanViewModel()
    {
        _ocrService = new OcrService();
        _circuitService = IPlatformApplication.Current.Services.GetService<CircuitService>();
        _lapTimeService = IPlatformApplication.Current.Services.GetService<LapTimeService>();

        TakePhotoCommand = new Command(async () => await TakePhotoAsync());
        PickImageCommand = new Command(async () => await PickImageAsync());
        SaveTimeCommand = new Command(async () => await SaveDetectedTimeAsync());
    }

    /// <summary>
    /// Toma una foto usando la cámara del dispositivo y la procesa si es capturada exitosamente.
    /// Muestra un mensaje de error si la captura no es compatible con el dispositivo.
    /// </summary>
    private async Task TakePhotoAsync()
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            _capturedImage = await MediaPicker.Default.CapturePhotoAsync();

            if (_capturedImage != null)
            {
               await ProcessImageAsync(_capturedImage);
            }
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "La captura de fotos no está soportada en este dispositivo", "OK");
        }
    }
    /// <summary>
    /// Permite al usuario seleccionar una imagen de la galería y la procesa si es seleccionada exitosamente.
    /// </summary>
    private async Task PickImageAsync()
    {
        _capturedImage = await MediaPicker.Default.PickPhotoAsync();

        if (_capturedImage != null)
        {
            await ProcessImageAsync(_capturedImage);
        }
    }
    /// <summary>
    /// Muestra una imagen en la UI a partir de un flujo de memoria.
    /// </summary>
    /// <param name="memoryStream">Flujo de memoria que contiene la imagen a mostrar.</param>
    private void ShowPreviewImage(MemoryStream memoryStream)
    {
        MemoryStream uiImageStream = new MemoryStream(memoryStream.ToArray());
        uiImageStream.Position = 0;
        PreviewImageSource = ImageSource.FromStream(() => uiImageStream);
    }
    /// <summary>
    /// Procesa una imagen para extraer texto mediante un servicio OCR, 
    /// muestra una vista previa de la imagen y notifica el resultado mediante la UI.
    /// </summary>
    /// <param name="image">Resultado del archivo de imagen seleccionado o capturado.</param>
    private async Task ProcessImageAsync(FileResult image)
    {
        try
        {
            HasDetectedTime = false;

            MemoryStream imageBytesStream = new MemoryStream();
            using (var sourceStream = await image.OpenReadAsync())
            {
                await sourceStream.CopyToAsync(imageBytesStream);
            }
            imageBytesStream.Position = 0;

            this.ShowPreviewImage(imageBytesStream);

            IsProcessing = true;
            this.AnalyzeResult = await this._ocrService.RecognizeTextAsync(imageBytesStream);
            HasDetectedTime = true;

            GuiUtils.SendSnakbarMessage("Los datos han sido capturados");

        }
        catch (Exception ex)
        {
            GuiUtils.SendSnakbarMessage($"Error al procesar la imagen: {ex.Message}");
        }
        finally
        {
            IsProcessing = false;
        }
    }
    /// <summary>
    /// Guarda los tiempos detectados en el OCR en el circuito seleccionado o creado por el usuario.
    /// Si el tiempo es el mejor del circuito, lo actualiza como el nuevo mejor tiempo.
    /// </summary>
    private async Task SaveDetectedTimeAsync()
    {
        if (!HasDetectedTime)
            return;

        try
        {
            Circuit selectedCircuit = null;

            var circuits = await _circuitService.GetCircuitsAsync();

            if (circuits.Count == 0)
            {
                string name = await Shell.Current.DisplayPromptAsync(
                    "No hay circuitos. Indique el nombre del circuito al que pertence los tipos",
                    "Nombre del circuito:",
                    "Guardar", "Cancelar"
                );

                if (!string.IsNullOrWhiteSpace(name))
                {
                    selectedCircuit = new Circuit
                    {
                        Name = name,
                        LastSessionDate = DateTime.Now
                    };

                    await _circuitService.SaveCircuitAsync(selectedCircuit);
                    circuits.Add(selectedCircuit);
                }
                else
                {
                    return;
                }
            }
            else
            {
                string[] circuitsNames = circuits.Select(x=>x.Name).ToArray();
                string circuit = await GuiUtils.DisplayActionSheet(
                    Shell.Current as Page,
                    "Indique el circuito",
                    "Guardar",
                    "Cancelar",
                    circuitsNames
                );
                selectedCircuit = circuits.FirstOrDefault(x=> x.Name ==circuit);
            }

            if (this.AnalyzeResult != null && selectedCircuit != null)
            {
                foreach(DocumentTable table in this.AnalyzeResult.Tables)
                {
                    
                    TimeSpan time = TimeSpan.FromSeconds(00.000);
                    string[] formats = { @"mm\:ss\.fff", @"mm\:ss\.ff", @"mm\:ss\.f" };
                    int lapCounter = selectedCircuit.LapTimes.Count;
                    foreach (DocumentTableCell cell in table.Cells)
                    {
                        if (TimeSpan.TryParseExact(cell.Content, formats, CultureInfo.InvariantCulture, TimeSpanStyles.None, out time))
                        {
                            time = TimeSpan.ParseExact(cell.Content, @"mm\:ss\.fff", CultureInfo.InvariantCulture);

                            lapCounter++; // Incrementas manualmente
                            var newLapTime = new LapTime
                            {
                                CircuitId = selectedCircuit.Id,
                                Time = time,
                                Date = DateTime.Now,
                                LapNumber = lapCounter
                            };

                            await _lapTimeService.SaveLapTimeAsync(newLapTime);
                            selectedCircuit.LapTimes.Add(newLapTime);
                        }
                    }
                }
                selectedCircuit.UpdateBestTime();
                selectedCircuit.LastSessionDate = DateTime.Now;
                await _circuitService.SaveCircuitAsync(selectedCircuit);

                this.IsDataKeepIt = true;
                await Shell.Current.DisplayAlert("Éxito", "Tiempo guardado correctamente", "OK");
                await Shell.Current.Navigation.PopAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Formato de tiempo inválido", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al guardar el tiempo: {ex.Message}", "OK");
        }

        await Shell.Current.Navigation.PopAsync(true);
    }
}