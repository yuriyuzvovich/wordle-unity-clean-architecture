using System;
using System.Collections.Generic;

namespace Wordle.Infrastructure.Persistence
{
    /// <summary>
    /// Serializable container for localization strings.
    /// Used with Unity's JsonUtility for loading language files.
    /// </summary>
    [Serializable]
    public class LocalizationData
    {
        public string languageCode;
        public string languageName;
        public LocalizationEntry[] entries;

        public Dictionary<string, string> ToDictionary()
        {
            var dictionary = new Dictionary<string, string>();

            if (entries != null)
            {
                foreach (var entry in entries)
                {
                    if (!string.IsNullOrEmpty(entry.key))
                    {
                        dictionary[entry.key] = entry.value ?? string.Empty;
                    }
                }
            }

            return dictionary;
        }
    }
}
