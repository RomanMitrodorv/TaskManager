using System.Text.Json.Serialization;

namespace Habit.API.Models;

public class Periodicity
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}