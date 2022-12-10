namespace Habit.API.Models
{
    public class HabitRequest
    {
        public string Name { get; set; }

        public int PeriodicityId { get; set; }
        public int Count { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
