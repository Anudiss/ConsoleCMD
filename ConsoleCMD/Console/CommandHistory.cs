using System;
using System.Collections.Generic;

namespace ConsoleCMD.Console
{
    public static class CommandHistory
    {
        public delegate void HistoryLineChangedHandler(string historyLine);
        public static event HistoryLineChangedHandler OnHistoryLineChanged;

        private readonly static List<string> _history = new List<string>() { "" };

        private static int _selectedLine;
        public static int SelectedLine
        {
            get => _selectedLine;
            private set
            {
                _selectedLine = Math.Max(0, Math.Min(_history.Count - 1, value));
                OnHistoryLineChanged?.Invoke(Current);
            }
        }

        public static string Current => _history[SelectedLine];
        public static void MoveNext() => SelectedLine++;
        public static void MovePrevious() => SelectedLine--;

        public static void AddHistoryLine(string historyLine)
        {
            if (string.IsNullOrEmpty(historyLine))
                return;
            _history.Insert(1, historyLine);
            Reset();
        }

        public static void SetCurrentHistoryLine(string historyLine) => _history[0] = historyLine;
        public static void Clear() => _history.RemoveRange(1, _history.Count - 1);
        public static void Reset() => SelectedLine = 0;
    }
}
