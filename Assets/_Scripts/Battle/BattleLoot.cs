using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
using MoreMountains.Feedbacks;
using Cursor = UnityEngine.Cursor;

public class BattleLoot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    AudioManager _audioManager;

    BattleManager _battleManager;
    BattleGrabManager _grabManager;
    BattleAbilityManager _abilityManager;

    MMF_Player _feelPlayer;
    ObjectShaders _objectShaders;

    [SerializeField] Texture2D _cursorTexture;

    [SerializeField] List<Loot> _LootChoices = new();
    [HideInInspector] public Loot Loot;

    GameObject _GFX;
    Material _material;
    GameObject _effect;

    bool _isCollected;

    void Start()
    {
        _audioManager = AudioManager.Instance;
        _battleManager = BattleManager.Instance;
        transform.parent = _battleManager.EntityHolder;
        _grabManager = BattleGrabManager.Instance;
        _abilityManager = _battleManager.GetComponent<BattleAbilityManager>();

        _feelPlayer = GetComponent<MMF_Player>();
        _objectShaders = GetComponent<ObjectShaders>();
    }

    public void Initialize()
    {
        List<Loot> ordered = new(_LootChoices.OrderBy(o => o.LootChance).ToList());
        float roll = Random.value;

        foreach (Loot p in ordered)
        {
            if (roll <= p.LootChance)
            {
                Loot = Instantiate(p);
                Loot.Initialize();
                break;
            }
            roll -= p.LootChance; // TODO: it's weird but I don't know how to make it work otherwise "cumulative chance"
        }

        // TODO: bad design? 
        if (Loot == null)
        {
            Destroy(gameObject);
            return;
        }

        _audioManager = AudioManager.Instance;
        _audioManager.PlaySFX(Loot.DropSound, transform.position);

        _effect = Instantiate(Loot.Effect, transform.position, Quaternion.identity);
        _effect.transform.parent = transform;

        _GFX = GetComponentInChildren<MeshRenderer>().gameObject;
        _material = GetComponentInChildren<Renderer>().material;
        _material.color = Loot.LootColor;

        float endY = Random.Range(2f, 4f);
        float timeY = Random.Range(1f, 3f);

        _GFX.transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        _GFX.transform.DOMoveY(endY, timeY)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId(transform);

        float timeRot = Random.Range(3f, 5f);
        Vector3 rotVector = new(360, 0, 0);

        _GFX.transform.DORotate(rotVector, timeRot, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Restart)
            .SetId(transform);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isCollected) return;
        _isCollected = true;

        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (_grabManager.IsGrabbingEnabled) return;
        if (_abilityManager.IsAbilitySelected) return;

        Vector3 endScale = _GFX.transform.localScale * 1.01f;
        _GFX.transform.DOPunchScale(endScale, 0.5f, 5, 0.5f)
            .OnComplete(() => DOTween.Kill(_GFX.transform));

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        GetComponent<Collider>().enabled = false;

        _audioManager.PlaySFX(Loot.CollectSound, transform.position);

        _effect.SetActive(false);

        GameObject clickEffect = Instantiate(Loot.ClickEffect, _GFX.transform.position, Quaternion.identity);
        clickEffect.transform.parent = transform;

        Rigidbody rb = _GFX.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;

        _objectShaders.Dissolve(10f, false);
        _objectShaders.OnDissolveComplete += () =>
            {
                if (this == null) return;
                gameObject.SetActive(false);
                transform.DOKill();
                Destroy(gameObject, 1f);
            };

        DisplayFloatingText(Loot.GetDisplayText(), Loot.GetDisplayColor());

        _battleManager.CollectLoot(Loot);
        Loot.Collect();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isCollected) return;

        Cursor.SetCursor(_cursorTexture, Vector2.zero, CursorMode.Auto);
        _material.EnableKeyword("_EMISSION");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isCollected) return;

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        _material.DisableKeyword("_EMISSION");
    }

    void DisplayFloatingText(string text, Color color)
    {
        if (_feelPlayer == null) return;
        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = text;
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = Helpers.GetGradient(color);
        _feelPlayer.PlayFeedbacks(_GFX.transform.position);
    }
}
