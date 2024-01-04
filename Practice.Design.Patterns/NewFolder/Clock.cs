using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Thor.Framework
{
    /// <summary>
    /// Prefer this instead of directly using <see cref="DateTimeOffset.UtcNow"/>
    /// because this can more easily be mocked for testing purposes.
    /// </summary>
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
    }

    internal class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
