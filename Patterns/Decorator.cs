using Task_Management.Models;

namespace Task_Management.Patterns
{
  
    public interface ICalendarEvent
    {
        string GetTitle();
        string GetColor();
        string GetDescription();
    }

   
    public class CalendarEvent : ICalendarEvent
    {
        public string Title { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; internal set; }
        public DateTime Date { get; internal set; }
        public string Id { get; internal set; }

        public virtual string GetTitle() => Title;

        public virtual string GetColor() => Color;

        public virtual string GetDescription() => Description;
    }


    public abstract class EventDecorator : ICalendarEvent
    {
        protected ICalendarEvent _event;

        public EventDecorator(ICalendarEvent calendarEvent)
        {
            _event = calendarEvent;
        }

        public virtual string GetTitle()
        {
            return _event.GetTitle();
        }

        public virtual string GetColor()
        {
            return _event.GetColor();
        }

        public virtual string GetDescription()
        {
            return _event.GetDescription();
        }
    }

    public class UrgentEventDecorator : EventDecorator
    {
        public UrgentEventDecorator(ICalendarEvent ev)
            : base(ev)
        {
        }

        public override string GetTitle()
        {
            return $"[URGENT] {_event.GetTitle()}";
        }

        public override string GetColor()
        {
            return "red";
        }
    }

    public class WorkEventDecorator : EventDecorator
    {
        public WorkEventDecorator(ICalendarEvent ev)
            : base(ev)
        {
        }

        public override string GetTitle()
        {
            return $"{_event.GetTitle()}";
        }

        public override string GetColor()
        {
            return "blue";
        }
    }

    
    public class RelaxEventDecorator : EventDecorator
    {
        public RelaxEventDecorator(ICalendarEvent ev)
            : base(ev)
        {
        }

        public override string GetTitle()
        {
            return $"{_event.GetTitle()}";
        }

        public override string GetColor()
        {
            return "green";
        }
    }

    public class ExamEventDecorator : EventDecorator
    {
        public ExamEventDecorator(ICalendarEvent ev)
            : base(ev)
        {
        }

        public override string GetTitle()
        {
            return $"{_event.GetTitle()}";
        }

        public override string GetColor()
        {
            return "purple";
        }
    }

  
    public class HolidayEventDecorator : EventDecorator
    {
        public HolidayEventDecorator(ICalendarEvent ev)
            : base(ev)
        {
        }

        public override string GetTitle()
        {
            return $"🌴 {_event.GetTitle()}";
        }

        public override string GetColor()
        {
            return "yellow";
        }
    }

    
    public class DetailedEventDecorator : EventDecorator
    {
        private string _extraInfo;

        public DetailedEventDecorator(ICalendarEvent ev, string extraInfo)
            : base(ev)
        {
            _extraInfo = extraInfo;
        }

        public override string GetDescription()
        {
            return $"{_event.GetDescription()}\nℹ️ {_extraInfo}";
        }
    }

}
