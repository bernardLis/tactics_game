using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class TextWithTooltip : ElementWithTooltip
    {
        readonly string _tooltipText;
        readonly Label _text;

        public TextWithTooltip(string text, string tooltipText) : base()
        {
            _tooltipText = tooltipText;
            _text = new Label(text);
            Add(_text);
        }

        public void UpdateText(string newText) { _text.text = newText; }

        public void UpdateFontSize(int newSize) { _text.style.fontSize = newSize; }

        public void UpdateTextColor(Color c) { _text.style.color = c; }

        protected override void DisplayTooltip()
        {
            Label t = new(_tooltipText);
            t.style.whiteSpace = WhiteSpace.Normal;
            _tooltip = new(this, t);
            base.DisplayTooltip();
        }

    }
}
