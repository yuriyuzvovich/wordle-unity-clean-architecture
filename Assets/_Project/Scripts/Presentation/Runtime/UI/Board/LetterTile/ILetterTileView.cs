using System;
using UnityEngine;

namespace Wordle.Presentation.UI.Board
{
    /// <summary>
    /// View interface for letter tile rendering.
    /// Defines contract between LetterTilePresenter and LetterTileView.
    /// </summary>
    public interface ILetterTileView
    {
        void SetText(string text);
        void SetTextColor(Color color);
        void SetBackgroundColor(Color color);
        void PlayPopAnimation();
        void PlayFlipAnimation(Action onFlipHalf);
        void PlayShakeAnimation();
        void ResetVisuals();
    }
}
