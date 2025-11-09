using System;

namespace Wordle.Infrastructure.Persistence
{
    [Serializable]
    public class LocalizationEntry
    {
        public string key;
        public string value;
    }
}