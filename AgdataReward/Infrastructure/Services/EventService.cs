using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly List<Event> _events = new();
        private int _nextId = 1;

        public Event AddEvent(Event ev)
        {
            ValidateEvent(ev);

            var newEvent = new Event(
                _nextId++,
                ev.EventName,
                ev.Description,
                ev.PointsReward,
                ev.StartDate,
                ev.EndDate
            );

            _events.Add(newEvent);
            return newEvent;
        }

        public IEnumerable<Event> GetAllEvents() => _events.AsReadOnly();

        public Event? GetById(int id) => _events.FirstOrDefault(e => e.EventId == id);

        public Event UpdateEvent(Event ev)
        {
            var existing = _events.FirstOrDefault(e => e.EventId == ev.EventId);
            if (existing == null)
                throw new InvalidOperationException("Event not found.");

            ValidateEvent(ev);

            // Update fields on existing event
            existing.UpdateDetails(ev.EventName, ev.Description, ev.PointsReward, ev.StartDate, ev.EndDate);

            return existing;
        }

        public void DeleteEvent(int id)
        {
            var existing = _events.FirstOrDefault(e => e.EventId == id);
            if (existing == null)
                throw new InvalidOperationException("Event not found.");

            _events.Remove(existing);
        }

        private static void ValidateEvent(Event ev)
        {
            if (string.IsNullOrWhiteSpace(ev.EventName))
                throw new ArgumentException("Event name is required.");
            if (ev.PointsReward <= 0)
                throw new ArgumentException("Reward points must be positive.");
            if (ev.StartDate >= ev.EndDate)
                throw new ArgumentException("Start date must be before end date.");
        }
    }
}
