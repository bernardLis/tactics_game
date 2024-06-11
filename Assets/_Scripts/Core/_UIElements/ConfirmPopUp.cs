using System;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class ConfirmPopUp : VisualElement
    {
        private const string _ussCommonTextPrimary = "common__text-primary";
        private const string _ussCommonButton = "common__button";

        private const string _ussClassName = "confirm-popup__";
        private const string _ussMain = _ussClassName + "main";
        private const string _ussText = _ussClassName + "text";
        private const string _ussButtonContainer = _ussClassName + "button-container";
        private MyButton _cancelButton;

        private MyButton _confirmButton;

        private GameManager _gameManager;
        private VisualElement _root;

        public void Initialize(VisualElement root, Action callback, string displayText = null)
        {
            _gameManager = GameManager.Instance;
            StyleSheet commonStyles = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (commonStyles != null)
                styleSheets.Add(commonStyles);
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.ConfirmPopupStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _root = root;
            root.Add(this);
            AddToClassList(_ussMain);

            if (displayText == null)
                displayText = "Are you sure?";

            Label confirm = new(displayText);
            confirm.AddToClassList(_ussText);
            confirm.AddToClassList(_ussCommonTextPrimary);
            Add(confirm);
            AddButtons(callback);
        }

        private void AddButtons(Action callback)
        {
            VisualElement container = new();
            container.AddToClassList(_ussButtonContainer);
            Add(container);

            _confirmButton = new("Yaasss Queen", _ussCommonButton, callback);
            container.Add(_confirmButton);
            _confirmButton.clickable.clicked += Hide;

            VisualElement spacer = new();
            spacer.style.width = 50;
            container.Add(spacer);

            _cancelButton = new("Cancel!@!", _ussCommonButton, Hide);
            container.Add(_cancelButton);
        }

        public void HideCancelButton()
        {
            _cancelButton.style.display = DisplayStyle.None;
        }

        private void Hide()
        {
            SetEnabled(false);
            _root.Remove(this);
        }
    }
}