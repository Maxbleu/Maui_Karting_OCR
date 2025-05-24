using System.Text.Json;
using MauiApp_Karting_OCR.Models;

namespace MauiApp_Karting_OCR.Services;

public class LapTimeService
{
    private readonly string _fileName = "laptimes.json";
    private readonly string _folderPath;
    public List<LapTime> LapTimes { get; set; }

    public LapTimeService()
    {
        _folderPath = FileSystem.AppDataDirectory;
        this.LapTimes = null;
    }
    /// <summary>
    /// Obtiene la ruta completa del archivo de almacenamiento donde se guardan los tiempos de vuelta.
    /// </summary>
    /// <returns>Ruta completa al archivo de datos.</returns>
    private string GetFilePath()
    {
        return Path.Combine(_folderPath, _fileName);
    }
    /// <summary>
    /// Obtiene todos los tiempos de vuelta guardados en el archivo local.
    /// Si no existe el archivo, retorna una lista vacía.
    /// </summary>
    /// <returns>Lista de objetos <see cref="LapTime"/>.</returns>
    public async Task<List<LapTime>> GetLapTimesAsync()
    {
        if (this.LapTimes == null)
        {
            string filePath = GetFilePath();

            if (!File.Exists(filePath))
            {
                return new List<LapTime>();
            }

            using var stream = File.OpenRead(filePath);
            this.LapTimes = await JsonSerializer.DeserializeAsync<List<LapTime>>(stream);
        }

        return this.LapTimes;
    }
    /// <summary>
    /// Obtiene todos los tiempos de vuelta asociados a un circuito específico.
    /// </summary>
    /// <param name="circuitId">Identificador del circuito.</param>
    /// <returns>Lista de tiempos de vuelta para el circuito especificado.</returns>
    public async Task<List<LapTime>> GetLapTimesForCircuitAsync(string circuitId)
    {
        var allLapTimes = await GetLapTimesAsync();
        return allLapTimes.Where(lt => lt.CircuitId == circuitId).ToList();
    }
    /// <summary>
    /// Guarda o actualiza un tiempo de vuelta en la lista persistente.
    /// Si el tiempo ya existe (por Id), lo actualiza; si no, lo agrega.
    /// </summary>
    /// <param name="lapTime">Objeto <see cref="LapTime"/> a guardar o actualizar.</param>
    public async Task SaveLapTimeAsync(LapTime lapTime)
    {
        this.LapTimes = await GetLapTimesAsync();

        var existingLapTime = this.LapTimes.FirstOrDefault(lt => lt.Id == lapTime.Id);
        if (existingLapTime != null)
        {
            this.LapTimes[this.LapTimes.IndexOf(existingLapTime)] = lapTime;
        }
        else
        {
            this.LapTimes.Add(lapTime);
        }

        string filePath = GetFilePath();
        using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, this.LapTimes);
    }
}