using System.Collections.ObjectModel;
using System.Windows.Input;
using MauiApp_Karting_OCR.Models;
using MauiApp_Karting_OCR.Services;

namespace MauiApp_Karting_OCR.ViewModels;

public class CircuitDetailViewModel : BaseViewModel
{
    private readonly LapTimeService _lapTimeService;
    private readonly CircuitService _circuitService;
    private Circuit _circuit;
    private TimeSpan _averageTime;
    private int _totalLaps;

    public Circuit Circuit
    {
        get => _circuit;
        set => SetProperty(ref _circuit, value);
    }
    public ObservableCollection<LapTime> LapTimes { get; } = new ObservableCollection<LapTime>();

    public TimeSpan AverageTime
    {
        get => _averageTime;
        set => SetProperty(ref _averageTime, value);
    }
    public int TotalLaps
    {
        get => _totalLaps;
        set => SetProperty(ref _totalLaps, value);
    }

    public ICommand AddLapTimeCommand { get; }

    public CircuitDetailViewModel(Circuit circuit)
    {
        Circuit = circuit;
        _lapTimeService = IPlatformApplication.Current.Services.GetService<LapTimeService>();
        _circuitService = IPlatformApplication.Current.Services.GetService<CircuitService>();

        AddLapTimeCommand = new Command(async () => await AddLapTimeAsync());
    }

    /// <summary>
    /// Este método se encarga de cargar
    /// todos los tiempos por vuelta 
    /// registrados en ese circuito
    /// </summary>
    /// <returns></returns>
    public async Task LoadLapTimesAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;

        try
        {
            LapTimes.Clear();
            var lapTimes = await _lapTimeService.GetLapTimesForCircuitAsync(Circuit.Id);

            // Sort by date descending
            foreach (var lapTime in lapTimes.OrderBy(lt => lt.Time))
            {
                LapTimes.Add(lapTime);
            }

            // Update stats
            TotalLaps = LapTimes.Count;

            if (TotalLaps > 0)
            {
                long totalTicks = LapTimes.Sum(lt => lt.Time.Ticks);
                AverageTime = TimeSpan.FromTicks(totalTicks / TotalLaps);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar los tiempos: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    /// <summary>
    /// Este método se encaga de cargar una
    /// nueva vuelta rápida en un circuito.
    /// </summary>
    /// <returns></returns>
    private async Task AddLapTimeAsync()
    {
        string timeStr = await Shell.Current.DisplayPromptAsync("Nuevo Tiempo", "Tiempo (mm:ss.fff):", "Guardar", "Cancelar");

        if (!string.IsNullOrWhiteSpace(timeStr))
        {
            if (TimeSpan.TryParseExact(timeStr, @"mm\:ss\.fff", null, out TimeSpan time))
            {
                var newLapTime = new LapTime
                {
                    CircuitId = Circuit.Id,
                    Time = time,
                    Date = DateTime.Now,
                    LapNumber = TotalLaps + 1
                };

                await _lapTimeService.SaveLapTimeAsync(newLapTime);
                LapTimes.Insert(0, newLapTime);

                // Update circuit best time if needed
                if (time < Circuit.BestTime)
                {
                    Circuit.BestTime = time;
                    Circuit.LastSessionDate = DateTime.Now;

                    await this._circuitService.SaveCircuitAsync(Circuit);
                }

                // Update stats
                TotalLaps = LapTimes.Count;

                if (TotalLaps > 0)
                {
                    long totalTicks = LapTimes.Sum(lt => lt.Time.Ticks);
                    AverageTime = TimeSpan.FromTicks(totalTicks / TotalLaps);
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Formato de tiempo inválido. Use mm:ss.fff", "OK");
            }
        }
    }
}