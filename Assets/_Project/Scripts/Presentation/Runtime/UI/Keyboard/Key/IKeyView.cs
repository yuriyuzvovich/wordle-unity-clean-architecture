using UnityEngine;

namespace Wordle.Presentation.UI.Keyboard
{
    /// <summary>
    /// View interface for keyboard key rendering.
    /// Defines contract between KeyPresenter and KeyView.
    /// </summary>
    public interface IKeyView
    {
        void SetText(string text);
        void SetBackgroundColor(Color color);
        void SetTextColor(Color color);
        void PlayPressAnimation();
        void ResetVisuals();
    }
}
