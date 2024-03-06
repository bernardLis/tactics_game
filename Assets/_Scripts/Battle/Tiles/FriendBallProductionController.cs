using System.Collections;
using Lis.Battle.Pickup;
using Lis.Battle.Tiles;
using Lis.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace Lis
{
    public class FriendBallProductionController : ProductionController
    {
        PickupManager _pickupManager;

        [SerializeField] Image _friendBallIcon;
        [SerializeField] TMP_Text _friendBallCountText;
        int _activeFriendBalls;
        int _productionLimit;

        IEnumerator _productionCoroutine;

        Transform _originalParentOfPickup;
        float _radians;

        protected override void OnTileUnlocked(Controller tile)
        {
            base.OnTileUnlocked(tile);
            _pickupManager = BattleManager.GetComponent<PickupManager>();
            _productionLimit = Upgrade.GetValue();

            _radians = 2 * Mathf.PI / _productionLimit;

            _friendBallIcon.sprite = GameManager.Instance.GameDatabase.FriendBallIcon;
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
                PickupController bp = _pickupManager.GetFriendBall(transform.position);
                _activeFriendBalls++;
                bp.OnCollected += OnFriendBallCollected;

                if (_originalParentOfPickup == null)
                    _originalParentOfPickup = bp.transform.parent;

                bp.transform.parent = transform;
                float vertical = Mathf.Sin(_radians * _activeFriendBalls);
                float horizontal = Mathf.Cos(_radians * _activeFriendBalls);
                Vector3 spawnDir = new(horizontal, 0, vertical);
                bp.transform.localPosition = spawnDir;
                bp.GetComponentInChildren<Rigidbody>().isKinematic = true;
                bp.transform.DOMoveY(2, 5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetDelay(1f);

                UpdateFriendBallCountText();

                yield return new WaitForSeconds(5f);
            }
        }

        void UpdateFriendBallCountText()
        {
            _friendBallCountText.text = $"{_activeFriendBalls}/{_productionLimit}";
        }

        void OnFriendBallCollected(PickupController bp)
        {
            _activeFriendBalls--;
            _friendBallCountText.text = $"{_activeFriendBalls}/{_productionLimit}";
            bp.GetComponentInChildren<Rigidbody>().isKinematic = false;
            bp.transform.parent = _originalParentOfPickup;
            bp.OnCollected -= OnFriendBallCollected;
            StartProduction();
        }
    }
}