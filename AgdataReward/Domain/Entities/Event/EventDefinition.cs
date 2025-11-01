using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Event
{
    /// <summary>
    /// Represents a definition/template of an event.
    /// </summary>
    public class EventDefinition
    {
        public Guid Id { get; private set; } // Primary Key
        public string Code { get; private set; } = null!; // Unique code for the event (e.g., "HACKATHON2025")
        public string Title { get; private set; } = null!; // Title of the event (e.g., "Annual Hackathon 2025")

        public virtual ICollection<EventInstance> Instances { get; private set; } = new List<EventInstance>(); // Navigation property to EventInstance
        public virtual ICollection<EventRewardRule> RewardRules { get; private set; } = new List<EventRewardRule>(); // Navigation property to EventRewardRule
        protected EventDefinition() { } // For ORM

        public EventDefinition(Guid id, string code, string title)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.", nameof(id));
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code is required.", nameof(code));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.", nameof(title));

            Id = id;
            Code = code.Trim();
            Title = title.Trim();
        }

        /// <summary>
        /// Registers a new instance (occurrence) of this event.
        /// </summary>
        public void AddInstance(EventInstance instance)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            Instances.Add(instance);
        }
    }
}

