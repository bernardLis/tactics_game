using System.Collections;
using Lis.Battle;
using Lis.Battle.Tiles;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis
{
    public class ThreatLevelController : MonoBehaviour
    {
        Controller _controller;
        BoxCollider _collider;

        int _threatPoints;
        IEnumerator _threatLevelCoroutine;

        Label _tileNameLabel;

        Label _threatLevelLabel;

        public void Initialize(Controller controller)
        {
            _controller = controller;
            _collider = GetComponent<BoxCollider>();

            VisualElement root = BattleManager.Instance.Root;
            _tileNameLabel = root.Q<Label>("tileName");
            _threatLevelLabel = root.Q<Label>("threatLevel");

            _collider.enabled = true;
        }

        public int GetThreatLevel()
        {
            return Mathf.FloorToInt(_threatPoints * 0.03f);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out HeroController heroController)) return;
            heroController.SetThreatLevelController(this);
            _tileNameLabel.text = Helpers.ParseScriptableObjectName(_controller.Upgrade.name);
            StartIncreasingThreatLevel();
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out HeroController _)) return;
            StartDecreasingThreatLevel();
        }

        void StartIncreasingThreatLevel()
        {
            if (_threatLevelCoroutine != null) StopCoroutine(_threatLevelCoroutine);
            _threatLevelCoroutine = ThreatLevelIncreaseCoroutine();
            StartCoroutine(_threatLevelCoroutine);
        }

        void StartDecreasingThreatLevel()
        {
            if (_threatLevelCoroutine != null) StopCoroutine(_threatLevelCoroutine);
            _threatLevelCoroutine = ThreatLevelDecreaseCoroutine();
            StartCoroutine(_threatLevelCoroutine);
        }

        IEnumerator ThreatLevelIncreaseCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                _threatPoints++;
                _threatLevelLabel.text = $"| Threat: {GetThreatLevel()}";
                if (this == null) yield break;
            }
        }

        IEnumerator ThreatLevelDecreaseCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                _threatPoints--;
                if (this == null) yield break;
            }
        }
    }
}