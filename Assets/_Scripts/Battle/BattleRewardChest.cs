using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using MoreMountains.Feedbacks;

public class BattleRewardChest : MonoBehaviour
{
    GameManager _gameManager;
    AudioManager _audioManager;
    BattleTooltipManager _tooltipManager;
    MMF_Player _feelPlayer;

    [SerializeField] Sound _spawnSound;
    [SerializeField] Sound _openSound;
    [SerializeField] Sound _closeSound;

    [SerializeField] GameObject _lid;
    [SerializeField] GameObject _glowEffect;
    [SerializeField] GameObject _beamEffect;

    bool _isOpened;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;
        _feelPlayer = GetComponent<MMF_Player>();

        _audioManager.PlaySFX(_spawnSound, transform.position);
    }


    void OnTriggerEnter(Collider collider)
    {
        if (!collider.TryGetComponent(out BattleHero hero)) return;
        if (_isOpened) return;

        StartCoroutine(Open());
    }

    IEnumerator Open()
    {
        if (_isOpened) yield break;
        _isOpened = true;

        _audioManager.PlaySFX(_openSound, transform.position);

        transform.DOShakePosition(0.5f, 0.1f);
        transform.DOShakeScale(0.5f, 0.2f);

        yield return new WaitForSeconds(0.5f);
        _lid.transform.DOLocalRotate(new Vector3(-45, 0, 0), 1f)
                    .SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.5f);

        _beamEffect.SetActive(true);
        int gold = Random.Range(500, 1000);
        DisplayText($"+{gold} Gold", _gameManager.GameDatabase.GetColorByName("Gold").Primary);
        _audioManager.PlaySFX("Collect Gold", transform.position);

        yield return new WaitForSeconds(2f);
        _glowEffect.transform.DOScale(0, 0.5f)
            .OnComplete(() => _glowEffect.SetActive(false));
        _beamEffect.transform.DOScale(0, 0.5f)
            .OnComplete(() => _beamEffect.SetActive(false));
        yield return new WaitForSeconds(0.5f);
        _audioManager.PlaySFX(_closeSound, transform.position);
        _lid.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f)
                    .SetEase(Ease.OutBack);
        yield return new WaitForSeconds(0.5f);

        transform.DOScale(0, 1f).OnComplete(() => Destroy(gameObject));
    }

    void DisplayText(string text, Color color)
    {
        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = text;
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = Helpers.GetGradient(color);
        Vector3 pos = transform.position + new Vector3(0, transform.localScale.y * 0.8f, 0);
        _feelPlayer.PlayFeedbacks(pos);
    }

}
