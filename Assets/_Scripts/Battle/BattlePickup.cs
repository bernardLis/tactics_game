using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Linq;
using MoreMountains.Feedbacks;

public class BattlePickup : MonoBehaviour, IPointerClickHandler
{
    MMF_Player _feelPlayer;

    [SerializeField] Shader _dissolveShader;

    [SerializeField] List<Pickup> _pickupChoices = new();
    [HideInInspector] public Pickup Pickup;

    GameObject _GFX;
    GameObject _effect;

    bool _isCollected;

    void Start()
    {
        _feelPlayer = GetComponent<MMF_Player>();
    }

    public void Initialize()
    {
        List<Pickup> ordered = new(_pickupChoices.OrderBy(o => o.PickupChance).ToList());
        float roll = Random.value;

        foreach (Pickup p in ordered)
        {
            if (roll <= p.PickupChance)
            {
                Pickup = Instantiate(p);
                Pickup.Initialize();
                break;
            }
            roll -= p.PickupChance; // TODO: it's weird but I don't know how to make it work otherwise "cumulative chance"
        }

        // TODO: bad design? 
        if (Pickup == null)
        {
            Destroy(gameObject);
            return;
        }

        _effect = Instantiate(Pickup.Effect, transform.position, Quaternion.identity);
        _effect.transform.parent = transform;

        _GFX = GetComponentInChildren<MeshRenderer>().gameObject;
        GetComponentInChildren<MeshRenderer>().material.color = Pickup.PickupColor;

        float endY = Random.Range(2f, 4f);
        float timeY = Random.Range(1f, 3f);

        _GFX.transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        _GFX.transform.DOMoveY(endY, timeY)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        float timeRot = Random.Range(3f, 5f);
        _GFX.transform.DORotate(new Vector3(0, 360, 360), timeRot, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Restart);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isCollected) return;
        _isCollected = true;

        BattleManager.Instance.CollectPickup(Pickup);

        _effect.SetActive(false);

        GameObject clickEffect = Instantiate(Pickup.ClickEffect, _GFX.transform.position, Quaternion.identity);
        clickEffect.transform.parent = transform;

        Rigidbody rb = _GFX.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        DOTween.Kill(_GFX.transform);

        Material mat = GetComponentInChildren<Renderer>().material;
        Texture2D tex = mat.mainTexture as Texture2D;
        mat.shader = _dissolveShader;
        mat.color = Pickup.PickupColor;
        mat.SetTexture("_Base_Texture", tex);
        DOTween.To(x => mat.SetFloat("_Dissolve_Value", x), 0, 1, 10f)
                .OnComplete(() => gameObject.SetActive(false));

        DisplayFloatingText(Pickup.GetDisplayText(), Pickup.GetDisplayColor());
    }

    void DisplayFloatingText(string text, Color color)
    {
        if (_feelPlayer == null) return;
        MMF_FloatingText floatingText = _feelPlayer.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = text;
        floatingText.ForceColor = true;
        floatingText.AnimateColorGradient = Helpers.GetGradient(color);
        _feelPlayer.PlayFeedbacks(transform.position);
    }
}
