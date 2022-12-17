using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace Task.API.Model
{
    public class Subtask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Completed { get; set; }   
        public int TaskId { get; set; }
        public virtual ScheduledTask Task { get; set; }
    }
}
