using System;

namespace Wordle.Application.Events
{
    /// <summary>
    /// Application event: An animation has completed.
    /// Used to coordinate UI animations and state transitions.
    /// </summary>
    public struct AnimationCompletedEvent
    {
        public readonly string EventId;
        public readonly DateTime Timestamp;
        public readonly string AnimationType;
        public readonly int TileRow;
        public readonly int TileColumn;

        public AnimationCompletedEvent(string animationType, int tileRow = -1, int tileColumn = -1)
        {
            EventId = nameof(AnimationCompletedEvent);
            Timestamp = DateTime.UtcNow;
            AnimationType = animationType;
            TileRow = tileRow;
            TileColumn = tileColumn;
        }

        public override string ToString()
        {
            if (TileRow >= 0 && TileColumn >= 0)
            {
                return $"[{EventId}] Animation '{AnimationType}' completed for tile [{TileRow},{TileColumn}]";
            }
            return $"[{EventId}] Animation '{AnimationType}' completed";
        }
    }

    /// <summary>
    /// Common animation type constants.
    /// </summary>
    public static class AnimationType
    {
        public const string TileFlip = "TileFlip";
        public const string TileShake = "TileShake";
        public const string RowBounce = "RowBounce";
        public const string KeyPress = "KeyPress";
    }
}
