using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class TextPrintingElement : VisualElement
    {
        private const string _ussCommonTextPrimary = "common__text-primary";

        private const string _ussClassName = "text-printing__";
        private const string _ussMain = _ussClassName + "main";
        private readonly float _durationMs;

        private readonly GameManager _gameManager;

        private readonly Label _textLabel;

        private readonly string _textToPrint;

        private int _currentLetterIndex;
        private int _currentSentenceIndex;

        private List<string> _sentencesToPrint = new();

        private IVisualElementScheduledItem _textPrintingScheduler;

        public TextPrintingElement(string text, float durationSeconds)
        {
            _gameManager = GameManager.Instance;

            StyleSheet commonStyles = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (commonStyles != null)
                styleSheets.Add(commonStyles);
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.TextPrintingStyles);
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

        public event Action OnFinishedPrinting;

        public void PrintSentences()
        {
            _sentencesToPrint = new();
            string[] parts = Regex.Split(_textToPrint, @"(?<=[.!?])");
            _sentencesToPrint.AddRange(parts);

            int sentenceDelay = Mathf.RoundToInt(_durationMs / _sentencesToPrint.Count);
            _textPrintingScheduler = schedule.Execute(AddSentence).Every(sentenceDelay);
        }

        private void AddSentence()
        {
            _textLabel.text = _sentencesToPrint[_currentSentenceIndex];
            _currentSentenceIndex++;

            if (_currentSentenceIndex >= _sentencesToPrint.Count)
            {
                _textPrintingScheduler.Pause();
                OnFinishedPrinting?.Invoke();
            }
        }

        public void PrintLetters()
        {
            int letterDelay = Mathf.RoundToInt(_durationMs / _textToPrint.Length);
            _textPrintingScheduler = schedule.Execute(AddLetter).Every(letterDelay);
        }

        private void AddLetter()
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