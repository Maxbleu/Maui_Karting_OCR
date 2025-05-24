using MauiApp_Karting_OCR.Models;
using MauiApp_Karting_OCR.Pages;

namespace MauiApp_Karting_OCR.Factory
{
    public static class CircuitDetailPageFactory
    {
        public static CircuitDetailPage Create(Circuit circuit)
        {
            return new CircuitDetailPage(circuit);
        }
    }
}
