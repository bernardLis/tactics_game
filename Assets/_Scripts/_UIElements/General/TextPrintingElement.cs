using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Threading.Tasks;

public class TextPrintingElement : VisualElement
{
    const string _ussCommonTextPrimary = "common__text-primary";

    const string _ussClassName = "text-printing__";
    const string _ussMain = _ussClassName + "main";

    GameManager _gameManager;

    Label _textLabel;

    string _textToPrint;
    int _currentLetterIndex = 0;
    int _letterDelay;

    IVisualElementScheduledItem _textPrintingScheduler;

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

        _textToPrint = text;
        _letterDelay = Mathf.RoundToInt(durationSeconds / text.Length * 1000);

        PrintText();
    }

    void PrintText()
    {
        _textPrintingScheduler = schedule.Execute(AddLetter).Every(_letterDelay);
    }

    void AddLetter()
    {
        if (_currentLetterIndex >= _textToPrint.Length)
        {
            _textPrintingScheduler.Pause();
            return;
        }

        _textLabel.text += _textToPrint[_currentLetterIndex];
        _currentLetterIndex++;
    }

}
