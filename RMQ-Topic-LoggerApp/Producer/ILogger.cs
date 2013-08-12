using System.Diagnostics;

namespace Producer
{
    public interface ILogger
    {
        void Write(Sector sector, string entry, TraceEventType traceEventType);
    }
}