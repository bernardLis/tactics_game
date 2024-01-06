using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Feedbacks;
using Random = UnityEngine.Random;

public class BattlePickup : MonoBehaviour
{
    protected AudioManager _audioManager;
    protected BattleManager _battleManager;
    MMF_Player _feelPlayer;

    [SerializeField] Transform _gfx;

    SphereCollider _sphereCollider;
    Hero _hero;

    public Pickup Pickup;

    public event Action OnPickedUp;

    public void Awake()
    {
        _audioManager = AudioManager.Instance;
        _battleManager = BattleManager.Instance;

        _feelPlayer = GetComponent<MMF_Player>();

        _hero = _battleManager.GetComponent<BattleHeroManager>().BattleHero.Hero;
        _sphereCollider = GetComponent<SphereCollider>();

        _hero.Pull.OnValueChanged += SetPickUpRadius;
    }

    public void Initialize(Pickup pickup, Vector3 position)
    {
        if (_gfx.childCount > 0)
            Destroy(_gfx.GetChild(0).gameObject);
        Instantiate(pickup.GFX, _gfx);

        transform.position = position;
        gameObject.SetActive(true);

        transform.DOLocalMoveY(0.5f, 0.5f).SetEase(Ease.OutBack);

        transform.localScale = Vector3.zero;
        transform.DOScale(1, 1f).SetEase(Ease.OutBack);

        transform.DORotate(new Vector3(0, 360, 0), 15f, RotateMode.FastBeyond360)
                 .SetLoops(-1).SetEase(Ease.InOutSine);

        Pickup = pickup;
        Pickup.Initialize();
        _audioManager.PlaySFX(pickup.DropSound, transform.position);

        SetPickUpRadius(_hero.Pull.GetValue());
        _sphereCollider.enabled = true;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.TryGetComponent(out BattleHero hero)) return;

        PickUp(hero);
    }

    protected virtual void PickUp(BattleHero hero)
    {
        if (Pickup == null) return;
        
        _sphereCollider.enabled = false;
        transform.DOKill();
        DisplayText(Pickup.GetCollectedText(), Pickup.Color.Primary);

        _audioManager.PlaySFX(Pickup.CollectSound, transform.position);
        Destroy(Instantiate(Pickup.CollectEffect, transform.position, Quaternion.identity), 1f);

        float punchDuration = 0.5f;
        transform.DOPunchScale(Vector3.one * 1.5f, punchDuration, 1, 1f);

        Vector3 jumpPos = new Vector3(
            hero.transform.position.x,
            hero.transform.position.y + 2f,
            hero.transform.position.z
        );
        float jumpDuration = 0.6f;

        transform.DOScale(0, jumpDuration)
            .SetDelay(punchDuration);

        transform.DOJump(jumpPos, 5f, 1, jumpDuration)
                .SetDelay(punchDuration)
                .OnComplete(() =>
                {
                    Pickup.Collected(hero.Hero);
                    gameObject.SetActive(false);
                });

        OnPickedUp?.Invoke();
    }

    void OnDestroy()
    {
        _hero.Pull.OnValueChanged -= SetPickUpRadius;
    }

    void SetPickUpRadius(int i)
    {
        _sphereCollider.radius = i;
    }

    public void GetToHero()
    {
        if (!gameObject.activeSelf) return;

        transform.DOMove(_battleManager.BattleHero.transform.position + Vector3.up, Random.Range(0.5f, 2f))
            .OnComplete(() =>
            {
                PickUp(_battleManager.BattleHero);
            });
    }

    protected void DisplayText(string text, Color color)
    {
        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = text;
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = Helpers.GetGradient(color);
        Vector3 pos = transform.position + new Vector3(0, transform.localScale.y * 0.8f, 0);
        _feelPlayer.PlayFeedbacks(pos);
    }

}