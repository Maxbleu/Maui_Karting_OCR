using KartinMauiApp_Karting_OCRgApp.ViewModels;

namespace MauiApp_Karting_OCR.Pages;

public partial class OcrScanPage : ContentPage
{
    private OcrScanViewModel _orcScanViewModel;
    private BoundingBoxDrawable _overlayDrawable;

    public OcrScanPage(OcrScanViewModel ocrScanViewModel)
    {
        InitializeComponent();
        _orcScanViewModel = ocrScanViewModel;
        BindingContext = _orcScanViewModel;

        _orcScanViewModel.PropertyChanged += _orcScanViewModel_PropertyChanged; ;

        _overlayDrawable = new BoundingBoxDrawable();
        OverlayCanvas.Drawable = _overlayDrawable;
        _overlayDrawable.AnalysisResult = null;
        _overlayDrawable.ImageDisplayWidth = 0;
        _overlayDrawable.ImageDisplayHeight = 0;
        OverlayCanvas.Invalidate();
    }

    private async void _orcScanViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(OcrScanViewModel.AnalyzeResult))
        {
            await Task.Delay(200);

            _overlayDrawable.ImageDisplayWidth = (float)PreviewImage.Width;
            _overlayDrawable.ImageDisplayHeight = (float)PreviewImage.Height;
            _overlayDrawable.AnalysisResult = this._orcScanViewModel.AnalyzeResult;
        }
        else
        {
            _overlayDrawable.AnalysisResult = null;
        }

        OverlayCanvas.Invalidate();
    }
}