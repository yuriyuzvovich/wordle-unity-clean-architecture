using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Wordle.Application.Attributes;
using Wordle.Application.Commands;
using Wordle.Application.Events;
using Wordle.Application.Interfaces;
using Wordle.Application.UseCases;
using Wordle.Core.Events;
using Wordle.Core.Interfaces;
using Wordle.Infrastructure.Common.DI;
using Wordle.Presentation.UI.Theme;

namespace Wordle.Presentation.UI.Modals
{
    /// <summary>
    /// Modal displayed when the player loses the game (runs out of attempts).
    /// Shows the target word and provides "Play Again" option.
    /// </summary>
    public class LoseModalView : BaseModalView
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _playAgainButton;

        [Inject] private ICommandQueue _commandQueue;
        [Inject] private ILocalizationService _localizationService;
        [Inject] private IWordRepository _wordRepository;

        private Sequence _showSequence;
        private Sequence _hideSequence;
        private string _targetWord;

        protected override void AfterAwake()
        {
            if (!_titleText) throw new System.Exception("LoseModalView: TitleText reference is missing.");
            if (!_messageText) throw new System.Exception("LoseModalView: MessageText reference is missing.");
            if (!_playAgainButton) throw new System.Exception("LoseModalView: PlayAgainButton reference is missing.");

            _playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            UpdatePlayAgainButtonText();

            _eventBus.Subscribe<GameLostEvent>(OnGameLost);
            _eventBus.Subscribe<LanguageChangedEvent>(OnLanguageChanged);

            Hide(immediate : true);
        }

        protected override void AfterDestroy()
        {
            if (_playAgainButton)
            {
                _playAgainButton.onClick.RemoveListener(OnPlayAgainClicked);
            }

            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<GameLostEvent>(OnGameLost);
                _eventBus.Unsubscribe<LanguageChangedEvent>(OnLanguageChanged);
            }
        }

        protected override void ApplyTextColors(Core.Theme.ColorScheme colorScheme)
        {
            _titleText.color = colorScheme.TextPrimaryColor.ToUnityColor();
            _messageText.color = colorScheme.TextSecondaryColor.ToUnityColor();
        }

        private void OnGameLost(GameLostEvent evt)
        {
            _targetWord = evt.TargetWord;
            DisplayLoseMessage(_targetWord);
            Show();
        }

        private void DisplayLoseMessage(string targetWord)
        {
            _titleText.text = _localizationService.GetString("modal.lose.title");

            var parameters = new Dictionary<string, string> {
                { "word", targetWord }
            };
            _messageText.text = _localizationService.GetString("modal.lose.message", parameters);
        }

        private void OnLanguageChanged(LanguageChangedEvent evt)
        {
            UpdatePlayAgainButtonText();

            if (!string.IsNullOrEmpty(_targetWord))
            {
                DisplayLoseMessage(_targetWord);
            }
        }

        private void UpdatePlayAgainButtonText()
        {
            var buttonText = _playAgainButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText)
            {
                buttonText.text = _localizationService.GetString("button.playagain");
            }
        }

        private async void OnPlayAgainClicked()
        {
            Hide();

            try
            {
                var randomWord = await _wordRepository.GetRandomTargetWordAsync();
                var startGameUseCase = ProjectContext.Container.Build<StartGameUseCase>();
                var command = new StartGameCommand(startGameUseCase, randomWord.Value);
                _commandQueue.Enqueue(command);
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoseModalView: Failed to start new game: {ex.Message}");
            }
        }
    }
}