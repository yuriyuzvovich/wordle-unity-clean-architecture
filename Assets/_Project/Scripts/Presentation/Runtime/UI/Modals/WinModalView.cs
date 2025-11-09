using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    /// Modal displayed when the player wins the game.
    /// Shows congratulations message with attempts used and provides "Play Again" option.
    /// </summary>
    public class WinModalView : BaseModalView
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _playAgainButton;

        [Inject] private ICommandQueue _commandQueue;
        [Inject] private ILocalizationService _localizationService;
        [Inject] private IWordRepository _wordRepository;

        private string _targetWord;
        private int _attemptsUsed;

        protected override void AfterAwake()
        {
            if (!_titleText) throw new System.Exception("WinModalView: TitleText reference is missing.");
            if (!_messageText) throw new System.Exception("WinModalView: MessageText reference is missing.");
            if (!_playAgainButton) throw new System.Exception("WinModalView: PlayAgainButton reference is missing.");

            _playAgainButton.onClick.AddListener(OnPlayAgainClicked);
            UpdatePlayAgainButtonText();

            _eventBus.Subscribe<GameWonEvent>(OnGameWon);
            _eventBus.Subscribe<RowRevealCompleteEvent>(OnRowRevealComplete);
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
                _eventBus.Unsubscribe<GameWonEvent>(OnGameWon);
                _eventBus.Unsubscribe<RowRevealCompleteEvent>(OnRowRevealComplete);
                _eventBus.Unsubscribe<LanguageChangedEvent>(OnLanguageChanged);
            }
        }

        protected override void ApplyTextColors(Core.Theme.ColorScheme colorScheme)
        {
            _titleText.color = colorScheme.TextPrimaryColor.ToUnityColor();
            _messageText.color = colorScheme.TextSecondaryColor.ToUnityColor();
        }

        private void OnGameWon(GameWonEvent evt)
        {
            _targetWord = evt.TargetWord;
            _attemptsUsed = evt.AttemptsUsed;
        }

        private void OnRowRevealComplete(RowRevealCompleteEvent evt)
        {
            if (evt.IsWinningRow)
            {
                DisplayWinMessage(_targetWord, _attemptsUsed);
                Show();
            }
        }

        private void DisplayWinMessage(string targetWord, int attempts)
        {
            _titleText.text = GetWinTitle(attempts);

            var messageKey = attempts == 1 ? "modal.win.message.singular" : "modal.win.message.plural";
            var parameters = new Dictionary<string, string> {
                { "word", targetWord },
                { "attempts", attempts.ToString() }
            };
            _messageText.text = _localizationService.GetString(messageKey, parameters);
        }

        private string GetWinTitle(int attempts)
        {
            var key = attempts switch {
                1 => "modal.win.genius",
                2 => "modal.win.magnificent",
                3 => "modal.win.impressive",
                4 => "modal.win.splendid",
                5 => "modal.win.great",
                6 => "modal.win.phew",
                _ => "modal.win.default"
            };

            return _localizationService.GetString(key);
        }

        private void OnLanguageChanged(LanguageChangedEvent evt)
        {
            UpdatePlayAgainButtonText();

            if (!string.IsNullOrEmpty(_targetWord) && _attemptsUsed > 0)
            {
                DisplayWinMessage(_targetWord, _attemptsUsed);
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
                Debug.LogError($"WinModalView: Failed to start new game: {ex.Message}");
            }
        }
    }
}