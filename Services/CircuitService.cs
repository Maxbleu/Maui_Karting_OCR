using System.Text.Json;
using MauiApp_Karting_OCR.Models;

namespace MauiApp_Karting_OCR.Services;

public class CircuitService
{
    private readonly string _fileName = "circuits.json";
    private readonly string _folderPath;
    public List<Circuit> Circuits { get; set; }

    public CircuitService()
    {
        _folderPath = FileSystem.AppDataDirectory;
        this.Circuits = null;
    }
    
    /// <summary>
    /// Obtiene la ruta completa del archivo de almacenamiento donde se guardan los circuitos.
    /// </summary>
    /// <returns>Ruta completa al archivo de datos.</returns>
    private string GetFilePath()
    {
        return Path.Combine(_folderPath, _fileName);
    }
    /// <summary>
    /// Obtiene la lista de circuitos desde el almacenamiento local.
    /// Si el archivo no existe, retorna una lista vacía.
    /// </summary>
    /// <returns>Lista de objetos <see cref="Circuit"/>.</returns>
    public async Task<List<Circuit>> GetCircuitsAsync()
    {
        if (this.Circuits == null)
        {
            string filePath = GetFilePath();

            if (!File.Exists(filePath))
            {
                return new List<Circuit>();
            }

            using var stream = File.OpenRead(filePath);
            this.Circuits = await JsonSerializer.DeserializeAsync<List<Circuit>>(stream);
        }
        return this.Circuits;
    }
    /// <summary>
    /// Guarda o actualiza un circuito en la lista persistente.
    /// Si el circuito ya existe (por Id), lo actualiza; si no, lo agrega.
    /// </summary>
    /// <param name="circuit">Objeto <see cref="Circuit"/> a guardar o actualizar.</param>
    public async Task SaveCircuitAsync(Circuit circuit)
    {
        var existingCircuit = Circuits.FirstOrDefault(c => c.Id == circuit.Id);
        if (existingCircuit != null)
        {
            Circuits[Circuits.IndexOf(existingCircuit)] = circuit;
        }
        else
        {
            Circuits.Add(circuit);
        }

        string filePath = GetFilePath();
        using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, Circuits);
    }
}