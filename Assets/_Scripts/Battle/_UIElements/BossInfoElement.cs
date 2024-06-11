using Lis.Core;
using Lis.Units.Boss;
using UnityEngine;

namespace Lis.Battle
{
    public class BossInfoElement : EntityInfoElement
    {
        private readonly BossController _bossController;

        private readonly ResourceBarElement _stunBar;

        public BossInfoElement(BossController bs) : base(bs)
        {
            _bossController = bs;

            style.minWidth = 600;

            string text = "Stun";
            if (GameManager.UpgradeBoard.GetUpgradeByName("Boss Stun").CurrentLevel == -1)
                text = "Buy Upgrade To Stun Boss";

            Color c = GameManager.GameDatabase.GetColorByName("Stun").Primary;
            _stunBar = new(c, text, bs.CurrentDamageToStun, bs.TotalDamageToStun);

            _stunBar.HideText();
            _stunBar.style.backgroundImage = null;
            _stunBar.style.minWidth = 300;
            _stunBar.style.height = 20;
            _stunBar.style.opacity = 0.8f;

            Add(_stunBar);

            bs.OnStunStarted += OnStunStarted;
            bs.OnStunFinished += OnStunFinished;

            UpdateEntityInfo(bs);
        }

        private void OnStunStarted()
        {
            _stunBar.UpdateTrackedVariables(_bossController.CurrentStunDuration,
                _bossController.TotalStunDuration);
        }

        private void OnStunFinished()
        {
            _stunBar.UpdateTrackedVariables(_bossController.CurrentDamageToStun,
                _bossController.TotalDamageToStun);
        }
    }
}