using System;
using Cysharp.Threading.Tasks;
using Wordle.Core.Interfaces;

namespace Wordle.Application.UseCases
{
    /// <summary>
    /// Use case: Validate if a word is valid for submission.
    /// Checks word length and whether it exists in the word list.
    /// </summary>
    public class ValidateWordUseCase
    {
        private readonly IWordValidator _wordValidator;

        public ValidateWordUseCase(IWordValidator wordValidator)
        {
            _wordValidator = wordValidator ?? throw new ArgumentNullException(nameof(wordValidator));
        }

        public UniTask<ValidationResult> ExecuteAsync(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                return UniTask.FromResult(new ValidationResult(false, "Word cannot be empty"));
            }

            if (word.Length != 5)
            {
                return UniTask.FromResult(new ValidationResult(false, "Word must be 5 letters"));
            }

            bool isValid = _wordValidator.IsValidWord(word);

            if (!isValid)
            {
                return UniTask.FromResult(new ValidationResult(false, "Word is not in the word list"));
            }

            return UniTask.FromResult(new ValidationResult(true, "Word is valid"));
        }
    }

    public readonly struct ValidationResult
    {
        public readonly bool IsValid;
        public readonly string Message;

        public ValidationResult(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }
    }
}
