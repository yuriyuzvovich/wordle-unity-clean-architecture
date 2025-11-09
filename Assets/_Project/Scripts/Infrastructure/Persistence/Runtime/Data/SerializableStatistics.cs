using System;

namespace Wordle.Infrastructure.Persistence
{
    [Serializable]
    internal class SerializableStatistics
    {
        public int totalGamesPlayed;
        public int totalGamesWon;
        public int totalGamesLost;
        public int currentStreak;
        public int maxStreak;
        public int[] winDistribution;

        public SerializableStatistics(int totalGamesPlayed, int totalGamesWon, int totalGamesLost, int currentStreak, int maxStreak, int[] winDistribution)
        {
            this.totalGamesPlayed = totalGamesPlayed;
            this.totalGamesWon = totalGamesWon;
            this.totalGamesLost = totalGamesLost;
            this.currentStreak = currentStreak;
            this.maxStreak = maxStreak;
            this.winDistribution = winDistribution ?? new int[6];
        }
    }
}