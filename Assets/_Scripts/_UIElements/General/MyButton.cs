using System;

using UnityEngine.UIElements;

namespace Lis
{
    public class MyButton : Button
    {
        const string _ussCommonButtonBasic = "common__button-basic";

        protected GameManager _gameManager;
        AudioManager _audioManager;
        CursorManager _cursorManager;

        protected Label _text;


        Action _currentCallback;
        public MyButton(string buttonText = null, string className = null, Action callback = null)
        {
            _gameManager = GameManager.Instance;
            var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (ss != null)
                styleSheets.Add(ss);

            _audioManager = AudioManager.Instance;
            _cursorManager = CursorManager.Instance;

            _text = new Label(buttonText);
            _text.style.whiteSpace = WhiteSpace.Normal;
            Add(_text);
            if (buttonText == "")
                _text.style.display = DisplayStyle.None;

            if (className != null)
            {
                AddToClassList(className);
                AddToClassList(_ussCommonButtonBasic);
                RemoveFromClassList("unity-button");
            }

            if (callback != null)
            {
                _currentCallback = callback;
                clicked += callback;
            }

            RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            RegisterCallback<PointerLeaveEvent>(OnPointerLeave);

            RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            this.Blur();
            _cursorManager.ClearCursor();
        }

        public void ChangeCallback(Action newCallback)
        {
            clickable = new Clickable(() => { });
            clicked += newCallback;
        }

        public void ClearCallbacks() { clickable = new Clickable(() => { }); }

        public void SetText(string newText)
        {
            _text.text = newText;
            _text.style.display = DisplayStyle.Flex;
        }

        void OnPointerEnter(PointerEnterEvent evt)
        {
            if (!enabledSelf)
                return;
            if (_audioManager != null)
                _audioManager.PlayUI("UI Click");
            if (_cursorManager != null)
                _cursorManager.SetCursorByName("Hover");
        }

        void OnPointerLeave(PointerLeaveEvent evt)
        {
            _cursorManager.ClearCursor();
        }

        void PreventInteraction(MouseEnterEvent evt)
        {
            evt.PreventDefault();
            evt.StopImmediatePropagation();
        }

        void OnDisable()
        {
            UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
            UnregisterCallback<PointerLeaveEvent>(OnPointerLeave);

            // https://forum.unity.com/threads/hover-state-control-from-code.914504/
            RegisterCallback<MouseEnterEvent>(PreventInteraction);
        }

        void OnEnable()
        {
            RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            RegisterCallback<PointerLeaveEvent>(OnPointerLeave);

            UnregisterCallback<MouseEnterEvent>(PreventInteraction);
        }

    }
}
