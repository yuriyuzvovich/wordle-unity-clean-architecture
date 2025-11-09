using Cysharp.Threading.Tasks;

namespace Wordle.Core.Interfaces
{
    public interface IStatisticsRepository
    {
        UniTask SaveStatisticsAsync(int totalGamesPlayed, int totalGamesWon, int totalGamesLost, int currentStreak, int maxStreak, int[] winDistribution);
        UniTask<(int totalGamesPlayed, int totalGamesWon, int totalGamesLost, int currentStreak, int maxStreak, int[] winDistribution)> LoadStatisticsAsync();
        UniTask<bool> HasStatisticsAsync();
        UniTask ClearStatisticsAsync();
    }
}
