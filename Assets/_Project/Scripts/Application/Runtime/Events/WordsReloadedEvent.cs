using System;

namespace Wordle.Application.Events
{
    /// <summary>
    /// Application event: Word lists have been reloaded for a new language.
    /// Used to notify components that need to respond to word list changes (e.g., GameController).
    /// </summary>
    public struct WordsReloadedEvent
    {
        public readonly string EventId;
        public readonly DateTime Timestamp;
        public readonly string LanguageCode;
        public readonly int TargetWordsCount;
        public readonly int ValidWordsCount;

        public WordsReloadedEvent(string languageCode, int targetWordsCount, int validWordsCount)
        {
            EventId = nameof(WordsReloadedEvent);
            Timestamp = DateTime.UtcNow;
            LanguageCode = languageCode;
            TargetWordsCount = targetWordsCount;
            ValidWordsCount = validWordsCount;
        }

        public override string ToString()
        {
            return $"[{EventId}] Words reloaded for '{LanguageCode}': {TargetWordsCount} target words, {ValidWordsCount} valid words";
        }
    }
}
