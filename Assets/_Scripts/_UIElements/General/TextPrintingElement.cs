using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class TextPrintingElement : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "text-printing__";
        const string _ussMain = _ussClassName + "main";

        readonly GameManager _gameManager;

        readonly Label _textLabel;

        readonly string _textToPrint;
        readonly float _durationMs;

        int _currentLetterIndex = 0;

        List<string> _sentencesToPrint = new();
        int _currentSentenceIndex = 0;

        IVisualElementScheduledItem _textPrintingScheduler;

        public event Action OnFinishedPrinting;
        public TextPrintingElement(string text, float durationSeconds)
        {
            _gameManager = GameManager.Instance;

            var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (commonStyles != null)
                styleSheets.Add(commonStyles);
            var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.TextPrintingStyles);
            if (ss != null)
                styleSheets.Add(ss);

            AddToClassList(_ussCommonTextPrimary);
            AddToClassList(_ussMain);

            _textLabel = new();
            _textLabel.style.whiteSpace = WhiteSpace.Normal;
            Add(_textLabel);

            _durationMs = durationSeconds * 1000;
            _textToPrint = text;
        }

        public void PrintSentences()
        {
            _sentencesToPrint = new();
            string[] parts = Regex.Split(_textToPrint, @"(?<=[.!?])");
            _sentencesToPrint.AddRange(parts);

            int sentenceDelay = Mathf.RoundToInt(_durationMs / _sentencesToPrint.Count);
            _textPrintingScheduler = schedule.Execute(AddSentence).Every(sentenceDelay);
        }

        void AddSentence()
        {
            _textLabel.text = _sentencesToPrint[_currentSentenceIndex];
            _currentSentenceIndex++;

            if (_currentSentenceIndex >= _sentencesToPrint.Count)
            {
                _textPrintingScheduler.Pause();
                OnFinishedPrinting?.Invoke();
                return;
            }
        }

        public void PrintLetters()
        {
            int letterDelay = Mathf.RoundToInt(_durationMs / _textToPrint.Length);
            _textPrintingScheduler = schedule.Execute(AddLetter).Every(letterDelay);
        }

        void AddLetter()
        {
            if (_currentLetterIndex >= _textToPrint.Length)
            {
                _textPrintingScheduler.Pause();
                OnFinishedPrinting?.Invoke();
                return;
            }

            _textLabel.text += _textToPrint[_currentLetterIndex];
            _currentLetterIndex++;
        }

    }
}
