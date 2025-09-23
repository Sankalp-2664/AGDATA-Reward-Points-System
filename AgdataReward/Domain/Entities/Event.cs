using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Event
    {
        public int EventId { get; private set; }
        public string EventName { get; private set; }
        public string Description { get; private set; }
        public int PointsReward { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        private List<UserProfile> Participants { get; set; } = new List<UserProfile>();

        public Event(int eventId, string name, string description, int pointsReward, DateTime startDate, DateTime endDate)
        {
            EventId = eventId;
            EventName = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? "";
            PointsReward = pointsReward;
            StartDate = startDate;
            EndDate = endDate;
        }

        public void AddParticipant(UserProfile user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            Participants.Add(user);
        }

        public void CalculateRewards()
        {
            foreach (var user in Participants)
            {
                user.ParticipateInEvent(this);
            }
        }

        public void UpdateDetails(string eventName, string description, int pointsReward, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(eventName)) throw new ArgumentException("Event name is required.");
            if (pointsReward <= 0) throw new ArgumentException("Reward points must be positive.");
            if (startDate >= endDate) throw new ArgumentException("Start date must be before end date.");

            EventName = eventName;
            Description = description;
            PointsReward = pointsReward;
            StartDate = startDate;
            EndDate = endDate;
        }

    }
}
