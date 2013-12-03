using System;

namespace ClassLibrary2
{
    public interface IHaveTTL
    {
        bool HasExpired();
        void SetExpiry(TimeSpan duration);
    }
}