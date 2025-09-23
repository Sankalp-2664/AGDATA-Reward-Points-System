using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public sealed class EventDefinition
{
    public Guid Id { get; }
    public string Code { get; }
    public string Title { get; }

    public EventDefinition(Guid id, string code, string title)
    {
        Id = id;
        Code = code;
        Title = title;
    }
}

