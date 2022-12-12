using System.Text.Json.Serialization;

namespace Habit.API.Models;

public class HabitModel
{
    public int Id { get; set; }

    [JsonIgnore]
    public Guid UserId { get; set; }

    public DateTime DateCreation { get; set; }

    public string Name { get; set; }
    public virtual Periodicity Periodicity { get; set; }

    public int PeriodicityId { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; }
    public int Count { get; set; } = 1;
    public int CompletedCount { get; set; }
}

