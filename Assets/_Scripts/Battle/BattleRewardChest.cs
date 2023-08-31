using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using MoreMountains.Feedbacks;

public class BattleRewardChest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
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
        _audioManager = AudioManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;
        _feelPlayer = GetComponent<MMF_Player>();

        _audioManager.PlaySFX(_spawnSound, transform.position);
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
        LootGold lootGold = ScriptableObject.CreateInstance<LootGold>();
        lootGold.GoldRange = new Vector2Int(500, 1000);
        lootGold.Initialize();
        lootGold.Collect();
        DisplayText(lootGold.GetDisplayText(), lootGold.GetDisplayColor());

        yield return new WaitForSeconds(3f);
        _glowEffect.transform.DOScale(0, 0.5f)
            .OnComplete(() => _glowEffect.SetActive(false));
        _beamEffect.transform.DOScale(0, 0.5f)
            .OnComplete(() => _beamEffect.SetActive(false));
        yield return new WaitForSeconds(1f);
        _audioManager.PlaySFX(_closeSound, transform.position);
        _lid.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f)
                    .SetEase(Ease.OutBack);
        yield return new WaitForSeconds(1f);

        transform.DOScale(0, 1f).OnComplete(() => Destroy(gameObject));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        if (_isOpened) return;

        _tooltipManager.ShowInfo(new BattleInfoElement("Open"));
        transform.DOShakePosition(0.5f, 0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_tooltipManager == null) return;
        _tooltipManager.HideInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (_isOpened) return;

        StartCoroutine(Open());
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
