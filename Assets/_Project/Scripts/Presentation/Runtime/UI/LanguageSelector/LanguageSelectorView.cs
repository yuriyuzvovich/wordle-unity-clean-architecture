using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Wordle.Application.Attributes;
using Wordle.Application.Interfaces;
using Wordle.Infrastructure.Common.DI;

namespace Wordle.Presentation.UI.LanguageSelector
{
    /// <summary>
    /// UI component for language selection.
    /// Implements MVP pattern - delegates logic to LanguageSelectorPresenter.
    /// </summary>
    public class LanguageSelectorView : InjectableMonoBehaviour, ILanguageSelectorView
    {
        [Header("UI References")]
        [SerializeField] private TMP_Dropdown _languageDropdown;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Image _optionImage;
        [SerializeField] private TextMeshProUGUI _optionText;

        [Inject] private ILocalizationService _localizationService;
        [Inject] private IEventBus _eventBus;
        [Inject] private IThemeService _themeService;

        private LanguageSelectorPresenter _presenter;

        protected override void Awake()
        {
            base.Awake();

            if (!_languageDropdown) throw new System.Exception("LanguageSelectorView: TMP_Dropdown component not assigned");
            if (!_buttonImage) throw new System.Exception("LanguageSelectorView: Button Image component not assigned");
            if (!_optionImage) throw new System.Exception("LanguageSelectorView: Option Image component not assigned");
            if (!_optionText) throw new System.Exception("LanguageSelectorView: Option Text component not assigned");

            _presenter = new LanguageSelectorPresenter(this, _localizationService, _eventBus, _themeService);
            _presenter.Initialize();

            _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        }

        private void OnDestroy()
        {
            if (_languageDropdown)
            {
                _languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
            }

            _presenter?.Dispose();
        }

        private void OnLanguageChanged(int index)
        {
            _presenter?.OnLanguageChanged(index);
        }

        public void SetDropdownOptions(List<string> options)
        {
            if (!_languageDropdown) return;

            _languageDropdown.ClearOptions();

            var optionDataList = new List<TMP_Dropdown.OptionData>();
            foreach (var option in options)
            {
                optionDataList.Add(new TMP_Dropdown.OptionData(option));
            }

            _languageDropdown.AddOptions(optionDataList);
        }

        public void SetSelectedIndex(int index)
        {
            _languageDropdown.SetValueWithoutNotify(index);
        }

        public void SetTextColor(Color color)
        {
            _languageDropdown.captionText.color = color;
            _languageDropdown.itemText.color = color;
            _optionText.color = color;
        }

        public void SetBackgroundColor(Color color)
        {
            _buttonImage.color = color;
            _optionImage.color = color;
        }
    }
}