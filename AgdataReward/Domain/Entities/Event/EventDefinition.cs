namespace Domain.Entities.Event;

/// <summary>
/// Represents a definition/template of an event.
/// </summary>
public class EventDefinition
{
    public Guid Id { get; private set; } // Primary Key
    public string Code { get; private set; } = null!; // Unique code for the event (e.g., "HACKATHON2025")
    public string Title { get; private set; } = null!; // Title of the event (e.g., "Annual Hackathon 2025")
    public DateTime StartDate { get; private set; } // Start date of the event
    public DateTime EndDate { get; private set; } // End date of the event
    public string Status { get; private set; } = "Upcoming"; // Status: Upcoming, Active, Completed, Cancelled

    public virtual ICollection<EventInstance> Instances { get; private set; } = new List<EventInstance>(); // Navigation property to EventInstance
    public virtual ICollection<EventRewardRule> RewardRules { get; private set; } = new List<EventRewardRule>(); // Navigation property to EventRewardRule
    protected EventDefinition() { } // For ORM

    public EventDefinition(Guid id, string code, string title, DateTime startDate, DateTime endDate)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code is required.", nameof(code));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (endDate < startDate)
            throw new ArgumentException("End date must be after start date.", nameof(endDate));

        Id = id;
        Code = code.Trim();
        Title = title.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Status = "Upcoming";
    }

    /// <summary>
    /// Registers a new instance (occurrence) of this event.
    /// </summary>
    public void AddInstance(EventInstance instance)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        Instances.Add(instance);
    }
    
    /// <summary>
    /// Updates the status of the event.
    /// </summary>
    public void UpdateStatus(string newStatus)
    {
        if (string.IsNullOrWhiteSpace(newStatus))
            throw new ArgumentException("Status cannot be empty.", nameof(newStatus));
        Status = newStatus;
    }
    
    /// <summary>
    /// Updates the event details (code, title, dates).
    /// </summary>
    public void UpdateDetails(string? code, string? title, DateTime? startDate, DateTime? endDate)
    {
        if (code != null)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be empty.", nameof(code));
            Code = code.Trim();
        }
        
        if (title != null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));
            Title = title.Trim();
        }
        
        if (startDate.HasValue)
            StartDate = startDate.Value;
            
        if (endDate.HasValue)
        {
            if (endDate.Value < StartDate)
                throw new ArgumentException("End date must be after start date.", nameof(endDate));
            EndDate = endDate.Value;
        }
    }
    
    /// <summary>
    /// Computes the event status based on current date and event dates.
    /// Only changes if status is not already Completed or Cancelled.
    /// </summary>
    public string GetComputedStatus()
    {
        // If already completed or cancelled, don't change
        if (Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) ||
            Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            return Status;
        }
        
        var now = DateTime.UtcNow.Date;
        var start = StartDate.Date;
        var end = EndDate.Date;
        
        if (now < start)
            return "Upcoming";
        if (now >= start && now <= end)
            return "Active";
        if (now > end)
            return "Completed";
            
        return Status;
    }
}

