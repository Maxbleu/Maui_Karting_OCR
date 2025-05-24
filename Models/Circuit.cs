using System.Collections.ObjectModel;

namespace MauiApp_Karting_OCR.Models
{
    public class Circuit
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public TimeSpan BestTime { get; set; }
        public DateTime LastSessionDate { get; set; }
        public ObservableCollection<LapTime> LapTimes { get; set; } = new ObservableCollection<LapTime>();

        public void UpdateBestTime()
        {
            if (LapTimes.Count > 0)
            {
                BestTime = TimeSpan.FromTicks(LapTimes.Min(lt => lt.Time.Ticks));
                LastSessionDate = LapTimes.Max(lt => lt.Date);
            }
        }
    }
}
