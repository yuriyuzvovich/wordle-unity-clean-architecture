using UnityEngine;

namespace Wordle.Presentation.UI.Theme
{
    /// <summary>
    /// View interface for theme toggle rendering.
    /// Defines contract between ThemeTogglePresenter and ThemeToggleView.
    /// </summary>
    public interface IThemeToggleView
    {
        void SetToggleState(bool isOn);
        void SetLabelText(string text);
        void SetTextColor(Color color);
        void SetBackgroundColor(Color color);
    }
}
