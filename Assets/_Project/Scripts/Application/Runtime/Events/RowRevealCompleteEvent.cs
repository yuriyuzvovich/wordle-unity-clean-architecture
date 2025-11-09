using System;

namespace Wordle.Application.Events
{
    /// <summary>
    /// Application event: All tile reveal animations in a row have completed.
    /// Published after the flip animations finish for all tiles in a guess row.
    /// </summary>
    public struct RowRevealCompleteEvent
    {
        public readonly string EventId;
        public readonly DateTime Timestamp;
        public readonly int RowIndex;
        public readonly bool IsWinningRow;

        public RowRevealCompleteEvent(int rowIndex, bool isWinningRow)
        {
            EventId = nameof(RowRevealCompleteEvent);
            Timestamp = DateTime.UtcNow;
            RowIndex = rowIndex;
            IsWinningRow = isWinningRow;
        }

        public override string ToString()
        {
            return $"[{EventId}] Row {RowIndex} reveal complete (winning: {IsWinningRow})";
        }
    }
}
