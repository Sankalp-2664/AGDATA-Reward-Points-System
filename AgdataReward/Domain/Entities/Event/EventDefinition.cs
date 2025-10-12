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
        public string Code { get; private set; } = null!;
        public string Title { get; private set; } = null!;

        public virtual ICollection<EventInstance> Instances { get; private set; } = new List<EventInstance>();
        public virtual ICollection<EventRewardRule> RewardRules { get; private set; } = new List<EventRewardRule>();
        protected EventDefinition() { } // For ORM

        public EventDefinition(Guid id, string code, string title)
        {
            Id = id != Guid.Empty ? id : throw new ArgumentException("Id cannot be empty.");
            Code = !string.IsNullOrWhiteSpace(code) ? code : throw new ArgumentException("Code is required.");
            Title = !string.IsNullOrWhiteSpace(title) ? title : throw new ArgumentException("Title is required.");
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

