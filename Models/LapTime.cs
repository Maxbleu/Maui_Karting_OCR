namespace MauiApp_Karting_OCR.Models
{
    public class LapTime
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CircuitId { get; set; }
        public TimeSpan Time { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public int LapNumber { get; set; }
        public string Notes { get; set; }
    }
}
