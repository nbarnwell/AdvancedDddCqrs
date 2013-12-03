using AdvancedDddCqrs;

namespace ConsoleRunner
{
    internal class ThreadBoundaryMonitor
    {
        public ThreadBoundary<TEvent> Wrap<TEvent>(IHandler<TEvent> handler)
        {
            return new ThreadBoundary<TEvent>(handler);
        }
    }
}