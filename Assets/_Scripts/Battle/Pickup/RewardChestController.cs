using System.Collections;
using DG.Tweening;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Hero;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Lis.Battle.Pickup
{
    public class RewardChestController : MonoBehaviour
    {
        [SerializeField] private Sound _spawnSound;
        [SerializeField] private Sound _openSound;
        [SerializeField] private Sound _closeSound;

        [SerializeField] private GameObject _lid;
        [SerializeField] private GameObject _glowEffect;
        [SerializeField] private GameObject _beamEffect;
        private AudioManager _audioManager;
        private MMF_Player _feelPlayer;
        private GameManager _gameManager;

        private bool _isOpened;
        private TooltipManager _tooltipManager;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _audioManager = AudioManager.Instance;
            _tooltipManager = TooltipManager.Instance;
            _feelPlayer = GetComponent<MMF_Player>();

            _audioManager.PlaySfx(_spawnSound, transform.position);
        }


        private void OnTriggerEnter(Collider collider)
        {
            if (!collider.TryGetComponent(out HeroController hero)) return;
            if (_isOpened) return;

            StartCoroutine(Open());
        }

        private IEnumerator Open()
        {
            if (_isOpened) yield break;
            _isOpened = true;

            _audioManager.PlaySfx(_openSound, transform.position);

            transform.DOShakePosition(0.5f, 0.1f);
            transform.DOShakeScale(0.5f, 0.2f);

            yield return new WaitForSeconds(0.5f);
            _lid.transform.DOLocalRotate(new(-45, 0, 0), 1f)
                .SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.5f);

            _beamEffect.SetActive(true);
            int gold = Random.Range(500, 1000);
            DisplayText($"+{gold} Gold", _gameManager.GameDatabase.GetColorByName("Gold").Primary);
            _audioManager.PlaySfx("Collect Gold", transform.position);

            yield return new WaitForSeconds(2f);
            _glowEffect.transform.DOScale(0, 0.5f)
                .OnComplete(() => _glowEffect.SetActive(false));
            _beamEffect.transform.DOScale(0, 0.5f)
                .OnComplete(() => _beamEffect.SetActive(false));
            yield return new WaitForSeconds(0.5f);
            _audioManager.PlaySfx(_closeSound, transform.position);
            _lid.transform.DOLocalRotate(new(0, 0, 0), 1f)
                .SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.5f);

            transform.DOScale(0, 1f).OnComplete(() => Destroy(gameObject));
        }

        private void DisplayText(string text, Color color)
        {
            MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
            floatingText.Value = text;
            floatingText.ForceColor = true;
            floatingText.AnimateColorGradient = Helpers.GetGradient(color);
            Vector3 pos = transform.position + new Vector3(0, transform.localScale.y * 0.8f, 0);
            _feelPlayer.PlayFeedbacks(pos);
        }
    }
}