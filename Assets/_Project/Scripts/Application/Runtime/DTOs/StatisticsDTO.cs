namespace Wordle.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for player statistics.
    /// Contains game statistics and win distribution data.
    /// </summary>
    public readonly struct StatisticsDTO
    {
        public readonly int TotalGamesPlayed;
        public readonly int TotalGamesWon;
        public readonly int TotalGamesLost;
        public readonly int CurrentStreak;
        public readonly int MaxStreak;
        public readonly int[] WinDistribution;
        public readonly float WinRate;

        public StatisticsDTO(
            int totalGamesPlayed,
            int totalGamesWon,
            int totalGamesLost,
            int currentStreak,
            int maxStreak,
            int[] winDistribution)
        {
            TotalGamesPlayed = totalGamesPlayed;
            TotalGamesWon = totalGamesWon;
            TotalGamesLost = totalGamesLost;
            CurrentStreak = currentStreak;
            MaxStreak = maxStreak;
            WinDistribution = winDistribution;
            WinRate = totalGamesPlayed > 0 ? (float)totalGamesWon / totalGamesPlayed : 0f;
        }

        public static StatisticsDTO CreateEmpty()
        {
            return new StatisticsDTO(
                0,
                0,
                0,
                0,
                0,
                new int[6]
            );
        }
    }
}
