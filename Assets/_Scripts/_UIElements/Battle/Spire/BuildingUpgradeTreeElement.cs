using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StoreyUpgradeTreeElement : VisualElement
{
    const string _ussClassName = "storey-upgrade-tree__";
    const string _ussMain = _ussClassName + "main";
    const string _ussArrowLabel = _ussClassName + "arrow-label";

    GameManager _gameManager;

    BuildingUpgradeTree _tree;

    public List<StoreyUpgradeElement> UpgradeElements = new();

    public event Action<BuildingUpgrade> OnUpgradePurchased;
    public StoreyUpgradeTreeElement(BuildingUpgradeTree tree)
    {
        _gameManager = GameManager.Instance;
        var ss = _gameManager.GetComponent<AddressableManager>().GetStyleSheetByName(StyleSheetType.StoreyUpgradeTreeStyles);
        if (ss != null)
            styleSheets.Add(ss);

        _tree = tree;

        AddToClassList(_ussMain);
        CreateTree();

    }

    void CreateTree()
    {
        for (int i = 0; i < _tree.Nodes.Count; i++)
        {
            if (i > 0)
            {
                Label arrow = new("--->");
                arrow.AddToClassList(_ussArrowLabel);
                Add(arrow);
            }

            StoreyUpgradeElement el = new(_tree.Nodes[i]);
            Add(el);
            UpgradeElements.Add(el);

            el.OnPurchased += NodePurchased;

            if (i != _tree.CurrentNodeIndex + 1)
                el.SetEnabled(false);
        }
    }

    void NodePurchased(BuildingUpgrade storeyUpgrade)
    {
        UpgradeElements[_tree.CurrentNodeIndex + 1].SetEnabled(false);

        _tree.CurrentValue.SetValue(_tree.Nodes[_tree.CurrentNodeIndex + 1].Value);
        _tree.CurrentNodeIndex++;

        if (_tree.CurrentNodeIndex < _tree.Nodes.Count - 1)
            UpgradeElements[_tree.CurrentNodeIndex + 1].SetEnabled(true);

        OnUpgradePurchased?.Invoke(storeyUpgrade);
    }
}
