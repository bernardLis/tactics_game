using System.Collections;
using Lis.Battle.Fight;
using Lis.Core;
using Lis.Units;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class PeasantSpawner : ArenaInteractable, IInteractable
    {
        GameManager _gameManager;
        HeroManager _heroManager;

        public new string InteractionPrompt => "Press F To Interact!";

        protected override void Start()
        {
            base.Start();
            _gameManager = GameManager.Instance;
            _heroManager = HeroManager.Instance;
        }

        protected override void SetTooltipText()
        {
            TooltipText.text = InteractionPrompt;
        }

        protected override void OnFightEnded()
        {
            InteractionAvailableEffect.SetActive(true);
            IsInteractionAvailable = true;
            StartCoroutine(SpawnPeasantsCoroutine());
        }

        IEnumerator SpawnPeasantsCoroutine()
        {
            for (int i = 0; i < 3; i++)
            {
                Unit u = Instantiate(_gameManager.UnitDatabase.Peasant);
                u.InitializeBattle(0);
                _heroManager.Hero.Army.Add(u); // without update to spawn at position
                UnitController uc = FightManager.SpawnPlayerUnit(u, transform.position);
                uc.GoBackToLocker();
                yield return new WaitForSeconds(1);
            }
        }

        protected override void OnFightStarted()
        {
            InteractionAvailableEffect.SetActive(false);
            HideTooltip();
            IsInteractionAvailable = false;
        }

        public override bool Interact(Interactor interactor)
        {
            if (FightManager.IsFightActive)
            {
                Debug.Log("Fight is already active");
                return false;
            }

            FightManager.StartFight();
            return true;
        }
    }
}