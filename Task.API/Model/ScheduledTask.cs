namespace Task.API.Model
{
    public class ScheduledTask : IValidatableObject
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Notes { get; set; }
        public DateTime Date { get; set; }
        public virtual TaskStatus? Status { get; set; }
        public int StatusId { get; set; }
        public virtual TaskLabel? Label { get; set; }
        public int LabelId { get; set; }
        public bool IsActive { get; set; }
        public int ItemIndex { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Name.Length < 3)
                results.Add(new ValidationResult("The number of characters must be greater than 3", new[] { "Name" }));
            if (DateOnly.FromDateTime(Date) < DateOnly.FromDateTime(DateTime.Now))
                results.Add(new ValidationResult("The date must not be less than the current one", new[] { "Date" }));

            return results;

        }
    }
}
