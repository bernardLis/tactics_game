using UnityEngine.UIElements;

namespace Lis.Core
{
    public class PopUpElement : VisualElement
    {
        const string _ussCommonPopUpElementMain = "common__pop-up-element-main";
        const string _ussCommonButton = "common__button";

        protected readonly VisualElement Content;

        readonly VisualElement _parent;

        readonly Label _titleLabel;
        readonly MyButton _continueButton;

        protected PopUpElement(VisualElement parent)
        {
            _parent = parent;
            _parent.Add(this);
            _parent.focusable = false;

            AddToClassList(_ussCommonPopUpElementMain);

            _titleLabel = new("Title");
            _titleLabel.style.fontSize = 28;
            Add(_titleLabel);
            Add(new HorizontalSpacerElement());

            Content = new();
            Content.style.flexGrow = 1;
            Add(Content);

            _continueButton = new("Continue", _ussCommonButton, Hide);
            Add(_continueButton);
        }

        protected void SetTitle(string txt)
        {
            _titleLabel.text = txt;
        }

        void Hide()
        {
            _parent.focusable = true;
            RemoveFromHierarchy();
        }
    }
}