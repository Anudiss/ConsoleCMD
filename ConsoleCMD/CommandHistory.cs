using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace ConsoleCMD
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
                OnHistoryLineChanged?.Invoke(_history[_selectedLine]);
            }
        }

        public static string Current => _history[SelectedLine];
        public static void MoveNext() => SelectedLine++;
        public static void MovePrevious() => SelectedLine--;

        public static void AddHistoryLine(string historyLine) => _history.Insert(1, historyLine);
        public static void SetCurrentHistoryLine(string historyLine) => _history[0] = historyLine;
        public static void Clear() => _history.RemoveRange(1, _history.Count - 2);
        public static void Reset() => SelectedLine = 0;
    }
}
