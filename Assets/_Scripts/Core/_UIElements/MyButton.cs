using System;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class MyButton : Button
    {
        const string _ussCommonButtonBasic = "common__button-basic";
        readonly AudioManager _audioManager;
        readonly CursorManager _cursorManager;

        protected readonly Action CurrentCallback;

        protected readonly GameManager GameManager;

        protected readonly Label Text;

        public MyButton(string buttonText = null, string className = null, Action callback = null)
        {
            GameManager = GameManager.Instance;
            StyleSheet ss = GameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.CommonStyles);
            if (ss != null) styleSheets.Add(ss);

            _audioManager = AudioManager.Instance;
            _cursorManager = CursorManager.Instance;

            Text = new(buttonText);
            Text.style.whiteSpace = WhiteSpace.Normal;
            Text.style.fontSize = 24;
            Add(Text);
            if (buttonText == "")
                Text.style.display = DisplayStyle.None;

            if (className != null)
            {
                AddToClassList(className);
                AddToClassList(_ussCommonButtonBasic);
                RemoveFromClassList("unity-button");
            }

            if (callback != null)
            {
                CurrentCallback = callback;
                clicked += callback;
            }

            RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            RegisterCallback<PointerLeaveEvent>(OnPointerLeave);

            RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        void OnPointerUp(PointerUpEvent evt)
        {
            Blur();
            _cursorManager.ClearCursor();
        }

        public void SetText(string newText)
        {
            Text.text = newText;
            Text.style.display = DisplayStyle.Flex;
        }

        void OnPointerEnter(PointerEnterEvent evt)
        {
            if (!enabledSelf)
                return;
            if (_audioManager != null)
                _audioManager.CreateSound().WithSound(_audioManager.GetSound("UI Click")).Play();
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