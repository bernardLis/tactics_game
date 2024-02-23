using UnityEngine;

namespace Lis
{
    public class BossInfoElement : EntityInfoElement
    {
        readonly BattleBoss _battleBoss;

        readonly ResourceBarElement _stunBar;

        public BossInfoElement(BattleBoss bs) : base(bs)
        {
            _battleBoss = bs;

            style.minWidth = 600;

            string text = "Stun";
            if (_gameManager.UpgradeBoard.GetUpgradeByName("Boss Stun").CurrentLevel == -1)
                text = "Buy Upgrade To Stun Boss";

            Color c = _gameManager.GameDatabase.GetColorByName("Stun").Primary;
            _stunBar = new(c, text, bs.CurrentDamageToStun, bs.TotalDamageToStun);

            _stunBar.HideText();
            _stunBar.style.backgroundImage = null;
            _stunBar.style.minWidth = 300;
            _stunBar.style.height = 20;
            _stunBar.style.opacity = 0.8f;

            Add(_stunBar);

            bs.OnStunFinished += OnStunFinished;

            UpdateEntityInfo(bs);
        }

        void OnStunFinished()
        {
            _stunBar.UpdateTrackedVariables(_battleBoss.CurrentDamageToStun,
                _battleBoss.TotalDamageToStun);
        }
    }
}