using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Habit.API.Models;

public class Notification
{
    [JsonIgnore]
    public int Id { get; set; }
    [DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
    public TimeSpan Time { get; set; }
    [JsonIgnore]
    public virtual HabitModel Habit { get; set; }
    [JsonIgnore]
    public int HabitId { get; set; }
    [JsonIgnore]
    public string JobName { get; set; }
}