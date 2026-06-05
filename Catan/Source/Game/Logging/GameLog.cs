using System;
using System.Collections.Generic;

namespace Catan.Source.Game.Logging
{
    public class GameLog
    {
        private readonly Queue<string> _messages = new();
        private readonly int _maxMessages;

        public GameLog(int maxMessages = 50)
        {
            if (maxMessages <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxMessages), "O limite de mensagens deve ser maior que zero.");
            }

            _maxMessages = maxMessages;
        }

        public IReadOnlyList<string> Messages => _messages.ToArray();

        public void Add(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            _messages.Enqueue(message.Trim());

            while (_messages.Count > _maxMessages)
            {
                _messages.Dequeue();
            }
        }
    }
}
