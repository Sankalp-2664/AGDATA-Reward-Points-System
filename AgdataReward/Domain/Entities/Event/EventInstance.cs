using Domain.Entities.User;

namespace Domain.Entities.Event;

/// <summary>
/// Represents an individual occurrence of an event (e.g., Hackathon 2025 edition).
/// </summary>
public class EventInstance
{
    public Guid Id { get; private set; } // Primary Key
    public Guid EventId { get; private set; } // Foreign Key to Event (Event.Id)
    public Guid? WinnerUserId { get; private set; } // Foreign Key to UserProfile (UserProfile.Id)
    public int? Rank { get; private set; } // Rank achieved by the winner (1 for first place, etc.)

    public virtual EventDefinition? Event { get; private set; } // For navigation between EventDefinition and EventInstance
    public virtual UserProfile? WinnerUser { get; private set; } // For navigation between UserProfile and EventInstance

    protected EventInstance() { } // For ORM

    public EventInstance(Guid id, Guid eventId)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.", nameof(id));
        if (eventId == Guid.Empty) throw new ArgumentException("EventId cannot be empty.", nameof(eventId));

        Id = id;
        EventId = eventId;
    }

    private readonly HashSet<Guid> _participantIds = new();
    public IReadOnlyCollection<Guid> ParticipantIds => _participantIds;

    public void AddParticipant(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.");

        if (_participantIds.Contains(userId))
            throw new InvalidOperationException("You have already participated.");

        _participantIds.Add(userId);
    }


    /// <summary>
    /// Assigns a winner to this event instance.
    /// </summary>
    public void AssignWinner(Guid userId, int rank)
    {
        if (userId == Guid.Empty) throw new ArgumentException("Invalid user ID.", nameof(userId));
        if (rank <= 0) throw new ArgumentOutOfRangeException(nameof(rank), "Rank must be greater than zero.");

        WinnerUserId = userId;
        Rank = rank;
    }
}

