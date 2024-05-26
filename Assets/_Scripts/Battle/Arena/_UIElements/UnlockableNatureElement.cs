using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Pawn;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Arena
{
    public class UnlockableNatureElement : VisualElement
    {
        const string _ussCommonButton = "common__button";
        const string _ussClassName = "unlockable-nature-element__";
        const string _ussMain = _ussClassName + "main";

        readonly GameManager _gameManager;

        readonly UnlockableNature _unlockableNature;

        readonly PurchaseButton _purchaseButton;

        public UnlockableNatureElement(UnlockableNature n)
        {
            _gameManager = GameManager.Instance;
            StyleSheet ss = _gameManager.GetComponent<AddressableManager>()
                .GetStyleSheetByName(StyleSheetType.UnlockableNatureElementStyles);
            if (ss != null) styleSheets.Add(ss);
            AddToClassList(_ussMain);

            _unlockableNature = n;

            AddPawnElements();

            Add(new NatureElement(n.Nature));
            Add(new Label($"{n.Description}"));

            if (n.IsUnlocked) return;
            _purchaseButton = new("", _ussCommonButton, Unlock, n.Price);
            Add(_purchaseButton);
        }

        void AddPawnElements()
        {
            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;
            Add(container);

            Pawn p = _gameManager.UnitDatabase.GetPawnByNature(_unlockableNature.Nature);
            for (int i = 0; i < p.Upgrades.Length; i++)
            {
                Pawn newPawn = ScriptableObject.Instantiate(p);
                newPawn.InitializeBattle(1);
                for (int j = 0; j < i; j++)
                    newPawn.Upgrade();
                container.Add(new UnitIcon(newPawn));
            }
        }

        void Unlock()
        {
            // HERE: balance
            if (_gameManager.Gold < _unlockableNature.Price)
            {
                Helpers.DisplayTextOnElement(BattleManager.Instance.Root, this, "Not enough gold", Color.red);
                return;
            }

            _gameManager.ChangeGoldValue(-_unlockableNature.Price);
            _unlockableNature.IsUnlocked = true;
            _purchaseButton.RemoveFromHierarchy();
        }
    }
}