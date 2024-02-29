using System.Collections;
using Lis.Core;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Lis
{
    public class BattleBuildingFriendBallProduction : BattleBuilding
    {
        BattlePickupManager _pickupManager;

        [SerializeField] Image _friendBallIcon;
        [SerializeField] TMP_Text _friendBallCountText;
        int _activeFriendBalls;
        int _productionLimit;

        IEnumerator _productionCoroutine;

        Transform _originalParentOfPickup;
        float _radians;

        public override void Initialize(Vector3 pos, Building building)
        {
            base.Initialize(pos, building);

            _pickupManager = BattleManager.GetComponent<BattlePickupManager>();
            _productionLimit = Building.BuildingUpgrade.GetValue();

            _radians = 2 * Mathf.PI / _productionLimit;

            _friendBallIcon.sprite = GameManager.Instance.GameDatabase.FriendBallIcon;
        }

        protected override IEnumerator ShowBuildingCoroutine()
        {
            yield return base.ShowBuildingCoroutine();

            StartProduction();
        }

        void StartProduction()
        {
            if (_productionCoroutine != null) return;
            _productionCoroutine = ProduceFriendBalls();
            StartCoroutine(_productionCoroutine);
        }

        IEnumerator ProduceFriendBalls()
        {
            yield return new WaitForSeconds(1.5f);
            while (_activeFriendBalls < _productionLimit)
            {
                BattlePickup bp = _pickupManager.GetFriendBall(transform.position);
                _activeFriendBalls++;
                bp.OnCollected += OnFriendBallCollected;

                if (_originalParentOfPickup == null)
                    _originalParentOfPickup = bp.transform.parent;

                bp.transform.parent = transform;
                float vertical = Mathf.Sin(_radians * _activeFriendBalls);
                float horizontal = Mathf.Cos(_radians * _activeFriendBalls);
                Vector3 spawnDir = new(horizontal, 0, vertical);
                bp.transform.localPosition = spawnDir;

                UpdateFriendBallCountText();

                yield return new WaitForSeconds(5f);
            }
        }

        void UpdateFriendBallCountText()
        {
            _friendBallCountText.text = $"{_activeFriendBalls}/{_productionLimit}";
        }

        void OnFriendBallCollected(BattlePickup bp)
        {
            _activeFriendBalls--;
            _friendBallCountText.text = $"{_activeFriendBalls}/{_productionLimit}";
            bp.transform.parent = _originalParentOfPickup;
            bp.OnCollected -= OnFriendBallCollected;
            StartProduction();
        }
    }
}