using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattleEntityTooltipManager : Singleton<BattleEntityTooltipManager>
{

    VisualElement _root;
    VisualElement _bottomPanel;

    VisualElement _entityInfoContainer;

    BattleEntityElement _tooltip;

    void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;
        _bottomPanel = _root.Q<VisualElement>("bottomPanel");
        _entityInfoContainer = _root.Q<VisualElement>("entityInfoContainer");
    }

    public void ShowInfo(BattleEntity entity)
    {
        _entityInfoContainer.Clear();
        _entityInfoContainer.style.display = DisplayStyle.Flex;
        Label name = new(entity.ArmyEntity.Name);
        ElementalElement el = new(entity.ArmyEntity.Element);

        IntVariable totalHealth = ScriptableObject.CreateInstance<IntVariable>();
        totalHealth.SetValue((int)entity.ArmyEntity.Health);
        IntVariable currentHealth = ScriptableObject.CreateInstance<IntVariable>();
        currentHealth.SetValue((int)entity.GetCurrentHealth());
        entity.OnHealthChanged += (float newVal) => currentHealth.SetValue((int)newVal);
        ResourceBarElement bar = new(Color.red, "health", totalIntVar: totalHealth, currentIntVar: currentHealth);

        _entityInfoContainer.Add(name);
        _entityInfoContainer.Add(el);
        _entityInfoContainer.Add(bar);


    }

    public void HideInfo()
    {
        _entityInfoContainer.style.display = DisplayStyle.None;
        _entityInfoContainer.Clear();

    }

    public void DisplayTooltip(BattleEntity entity)
    {
        HideTooltip();

        _tooltip = new(entity);
        _bottomPanel.Add(_tooltip);
    }

    public void HideTooltip()
    {
        if (_tooltip == null) return;
        _bottomPanel.Remove(_tooltip);
    }
}
