using System;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class ConfirmPopUp : VisualElement
    {

        const string _ussCommonTextPrimary = "common__text-primary";
        const string _ussCommonButton = "common__button";

        const string _ussClassName = "confirm-popup__";
        const string _ussMain = _ussClassName + "main";
        const string _ussText = _ussClassName + "text";
        const string _ussButtonContainer = _ussClassName + "button-container";

        GameManager _gameManager;
        VisualElement _root;

        MyButton _confirmButton;
        MyButton _cancelButton;

        public void Initialize(VisualElement root, Action callback, string displayText = null)
        {
            _gameManager = GameManager.Instance;
            var commonStyles = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (commonStyles != null)
                styleSheets.Add(commonStyles);
            var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.ConfirmPopupStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _root = root;
            root.Add(this);
            AddToClassList(_ussMain);

            if (displayText == null)
                displayText = "Are you sure?";

            Label confirm = new Label(displayText);
            confirm.AddToClassList(_ussText);
            confirm.AddToClassList(_ussCommonTextPrimary);
            Add(confirm);
            AddButtons(callback);
        }

        void AddButtons(Action callback)
        {
            VisualElement container = new VisualElement();
            container.AddToClassList(_ussButtonContainer);
            Add(container);

            _confirmButton = new MyButton("Yaasss Queen", _ussCommonButton, callback);
            container.Add(_confirmButton);
            _confirmButton.clickable.clicked += Hide;

            VisualElement spacer = new VisualElement();
            spacer.style.width = 50;
            container.Add(spacer);

            _cancelButton = new MyButton("Cancel!@!", _ussCommonButton, Hide);
            container.Add(_cancelButton);
        }

        public void HideCancelButton()
        {
            _cancelButton.style.display = DisplayStyle.None;
        }

        void Hide()
        {
            this.SetEnabled(false);
            _root.Remove(this);
        }
    }
}

