using System;

namespace AdvancedDddCqrs
{
    public interface IHaveTTL
    {
        bool HasExpired();
        void SetExpiry(TimeSpan duration);
    }
}