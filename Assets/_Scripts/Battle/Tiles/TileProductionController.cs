using System.Collections;
using DG.Tweening;
using Lis.Battle;
using Lis.Battle.Tiles;
using Lis.Battle.Tiles.Building;
using Lis.Units;
using Lis.Units.Hero;
using Lis.Upgrades;
using UnityEngine;

namespace Lis
{
    public class TileProductionController : MonoBehaviour
    {
        protected BattleManager BattleManager;

        [SerializeField] GameObject _gfx;
        [SerializeField] protected Canvas Canvas;

        protected UpgradeTile Upgrade;

        void Awake()
        {
            GetComponent<TileController>().OnTileEnabled += OnTileEnabled;
            GetComponent<TileController>().OnTileUnlocked += OnTileUnlocked;
        }

        protected virtual void OnTileEnabled(UpgradeTile upgrade)
        {
            BattleManager = BattleManager.Instance;
            Upgrade = upgrade;
        }

        protected virtual void OnTileUnlocked(TileController tile)
        {
            Canvas.gameObject.SetActive(false);
            if (_gfx != null)
                _gfx.SetActive(true);

            GetComponentInChildren<PlayerUnitTracker>().OnEntityEnter += OnEntityEnter;
            GetComponentInChildren<PlayerUnitTracker>().OnEntityExit += OnEntityExit;
        }

        void OnEntityEnter(UnitController be)
        {
            if (!be.TryGetComponent(out HeroController _)) return;
            DisplayTooltip();
        }

        void OnEntityExit(UnitController be)
        {
            if (!be.TryGetComponent(out HeroController _)) return;
            HideTooltip();
        }

        void DisplayTooltip()
        {
            Canvas.gameObject.SetActive(true);
        }

        void HideTooltip()
        {
            Canvas.gameObject.SetActive(false);
        }
    }
}