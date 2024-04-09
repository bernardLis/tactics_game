using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Battle.Fight
{
    public class FightManager : MonoBehaviour
    {
        public Fight CurrentFight;

        VisualElement _debugInfoContainer;
        Label _debugInfoLabel;

        IEnumerator _fightCoroutine;

        public void Initialize()
        {
            SetupDebugInfo();

            CreateFight();
            _fightCoroutine = StartFightCoroutine();
            StartCoroutine(_fightCoroutine);
        }


        void SetupDebugInfo()
        {
            _debugInfoLabel = new Label();
            _debugInfoContainer = BattleManager.Instance.Root.Q<VisualElement>("fightDebugContainer");
            _debugInfoContainer.Add(_debugInfoLabel);
        }

        void CreateFight()
        {
            Fight fight = ScriptableObject.CreateInstance<Fight>();
            fight.CreateFight();
            CurrentFight = fight;
        }

        IEnumerator StartFightCoroutine()
        {
            // TODO: Start fight

            yield return new WaitForSeconds(2f);
            UpdateDebugInfo();

        }


        void UpdateDebugInfo()
        {
            // TODO: Update debug info
            // _debugInfoLabel.text =
            //     "Wave: " + wave.WaveIndex + " Points: " + wave.Points + " Level: " + wave.MinionLevel;
        }

    }
}