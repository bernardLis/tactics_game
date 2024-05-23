using System;
using Lis.Core;
using Lis.Units;
using UnityEngine.UIElements;

namespace Lis.Battle.Fight
{
    public class FightOptionElement : VisualElement
    {
        const string _ussCommonButton = "common__button";

        const string _ussClassName = "fight-option__";
        const string _ussMain = _ussClassName + "main";

        readonly MyButton _chooseButton;

        readonly FightOption _option;

        public FightOptionElement(FightOption option)
        {
            StyleSheet ss = GameManager.Instance.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.FightOptionElementStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);

            _option = option;

            Label label = new("Fight Option");
            Add(label);

            GoldElement goldElement = new(option.GoldReward);
            Add(goldElement);

            ScrollView scrollView = new();
            Add(scrollView);
            foreach (Unit u in option.Army)
            {
                UnitIcon icon = new(u);
                scrollView.Add(icon);
            }

            _chooseButton = new("Choose", _ussCommonButton, ChooseOption);
            Add(_chooseButton);
        }

        void ChooseOption()
        {
            _option.Choose();
        }

        public void DisableSelf()
        {
            _chooseButton.SetEnabled(false);
            if (_option.IsChosen) return;
            SetEnabled(false);
        }
    }
}