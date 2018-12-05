using System;

namespace InfiniBot
{
    public class ReminderContainer
    {
        public string text;
        public DateTime time;

        public ReminderContainer(string text, DateTime time)
        {
            this.text = text;
            this.time = time;
        }
    }
}
