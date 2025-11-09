using Wordle.Core.ValueObjects;

namespace Wordle.Presentation.UI.Board
{
    /// <summary>
    /// View interface for board rendering.
    /// Defines contract between BoardPresenter and BoardView.
    /// </summary>
    public interface IBoardView
    {
        /// <summary>
        /// Displays a letter at the specified tile position.
        /// </summary>
        void DisplayLetterAtTile(int row, int col, char letter);

        /// <summary>
        /// Sets the evaluation state for a tile (without animation).
        /// </summary>
        void SetTileEvaluation(int row, int col, LetterEvaluation evaluation);

        /// <summary>
        /// Plays the flip animation for a tile with the given evaluation.
        /// </summary>
        void PlayTileFlipAnimation(int row, int col, LetterEvaluation evaluation);

        /// <summary>
        /// Plays the shake animation for an entire row (invalid word).
        /// </summary>
        void PlayRowShakeAnimation(int row);

        /// <summary>
        /// Plays the victory animation for the winning row.
        /// </summary>
        void PlayRowVictoryAnimation(int row);

        /// <summary>
        /// Clears a specific tile (removes letter and resets state).
        /// </summary>
        void ClearTile(int row, int col);

        /// <summary>
        /// Resets all tiles on the board to empty state.
        /// </summary>
        void ResetAllTiles();

        /// <summary>
        /// Gets the tile at the specified position.
        /// </summary>
        LetterTileView GetTile(int row, int col);

        /// <summary>
        /// Number of rows in the board.
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// Number of columns in the board.
        /// </summary>
        int ColumnCount { get; }
    }
}
