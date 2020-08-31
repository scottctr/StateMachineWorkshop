using System;

namespace PointOfSaleStateManagement.Data
{
    public class LogEntry
    {
        private static int _id;

        public LogEntry(string message)
        {
            Id = ++_id;
            Entry = message;
            Timestamp = DateTime.Now;
        }

        public int Id { get; }
        public DateTime Timestamp { get; }
        public string Entry { get; }
    }
}