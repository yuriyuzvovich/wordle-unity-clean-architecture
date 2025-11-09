using System.Collections.Generic;
using UnityEngine;

namespace Wordle.Presentation.UI.LanguageSelector
{
    /// <summary>
    /// View interface for language selector rendering.
    /// Defines contract between LanguageSelectorPresenter and LanguageSelectorView.
    /// </summary>
    public interface ILanguageSelectorView
    {
        void SetDropdownOptions(List<string> options);
        void SetSelectedIndex(int index);
        void SetTextColor(Color color);
        void SetBackgroundColor(Color color);
    }
}
