using Lis.Core;
using Lis.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle
{
    public class EntityInfoElement : VisualElement
    {
        const string _ussCommonTextPrimary = "common__text-primary";

        const string _ussClassName = "unit-info__";
        const string _ussMain = _ussClassName + "main";

        protected GameManager _gameManager;

        readonly Label _name;
        readonly ResourceBarElement _bar;

        public EntityInfoElement(UnitController _)
        {
            _gameManager = GameManager.Instance;
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.EntityInfoElementStyles);
            if (ss != null) styleSheets.Add(ss);

            AddToClassList(_ussMain);
            AddToClassList(_ussCommonTextPrimary);

            _name = new();
            _name.style.fontSize = 32;
            _name.style.unityFontStyleAndWeight = FontStyle.Bold;
            _name.style.position = Position.Absolute;

            Color c = _gameManager.GameDatabase.GetColorByName("Health").Primary;
            _bar = new(c, "Health", ScriptableObject.CreateInstance<FloatVariable>());

            _bar.Add(_name);
            _bar.HideText();

            _bar.style.backgroundImage = null;
            _bar.style.minWidth = 300;
            _bar.style.height = 50;
            _bar.style.opacity = 0.8f;

            _bar.ResourceBar.style.height = Length.Percent(100);
            _bar.ResourceBar.style.width = Length.Percent(100);
            _bar.ResourceBar.style.marginLeft = Length.Percent(0);
            _bar.ResourceBar.style.marginRight = Length.Percent(0);

            _bar.MissingBar.style.height = Length.Percent(100);

            Add(_bar);
        }

        public void UpdateEntityInfo(UnitController unitController)
        {
            Debug.Log($"UpdateEntityInfo {unitController}");
            _name.text = unitController.Unit.UnitName;
            _bar.UpdateTrackedVariables(unitController.Unit.CurrentHealth, totalStat: unitController.Unit.MaxHealth);
        }
    }
}