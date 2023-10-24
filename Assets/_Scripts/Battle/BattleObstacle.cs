using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine.AI;

public class BattleObstacle : MonoBehaviour, IGrabbable, IPointerDownHandler
{
    AudioManager _audioManager;
    BattleGrabManager _grabManager;
    BattleTooltipManager _tooltipManager;

    [SerializeField] Sound _grabSound;
    [SerializeField] Sound _groundHitSound;

    [SerializeField] GameObject _dustParticlePrefab;

    Rigidbody _rb;
    Collider _collider;
    MMF_Player _feelPlayer;
    Material _material;

    Color _defaultColor;
    Color _endColor = new(0.875f, 0.32f, 0.28f, 1f); // reddish

    int _maxGrabbingTime = 5;
    int _secondsToBreak;

    IEnumerator _cooldownCoroutine;

    void Start()
    {
        _audioManager = AudioManager.Instance;
        _grabManager = BattleGrabManager.Instance;
        _tooltipManager = BattleTooltipManager.Instance;

        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();

        _feelPlayer = GetComponent<MMF_Player>();
        _material = GetComponent<Renderer>().material;
        _defaultColor = _material.color;

        _secondsToBreak = _maxGrabbingTime;
    }

    public void Initialize(Vector3 size)
    {
        transform.localScale = new Vector3(size.x, size.y, size.z);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!CanBeGrabbed()) return;
        _audioManager.PlaySFX(_grabSound, transform.position);
        _grabManager.TryGrabbing(gameObject, 3f);
    }

    public bool CanBeGrabbed()
    {
        if (_secondsToBreak <= 0)
        {
            DisplayText("No more grabbing!", Color.red);
            return false;
        }

        return true;
    }

    public void Grabbed()
    {
        _tooltipManager.ShowHoverInfo(new BattleInfoElement("Rotate", true));

        if (_cooldownCoroutine != null) StopCoroutine(_cooldownCoroutine);
        StartCoroutine(GrabBreaker());
    }

    public void Released()
    {
        StopAllCoroutines();
        _rb.isKinematic = false;
        DisplayText("Released!", Color.red);
        DOTween.Kill("Color");

        _cooldownCoroutine = Cooldown();
        StartCoroutine(_cooldownCoroutine);
    }

    IEnumerator Cooldown()
    {
        float time = (_maxGrabbingTime - _secondsToBreak) * 10f;

        _material.DOColor(_defaultColor, time).SetId("Color");

        while (_secondsToBreak < _maxGrabbingTime)
        {
            _secondsToBreak++;
            yield return new WaitForSeconds(10f);
        }
    }

    IEnumerator GrabBreaker()
    {
        DOTween.Kill("Color");
        _material.DOColor(_endColor, _secondsToBreak).SetId("Color");

        for (int i = _secondsToBreak; i > 0; i--)
        {
            _secondsToBreak--;
            DisplayText($"{i}", Color.red);
            yield return new WaitForSeconds(1f);
        }
        DisplayText("Release!", Color.red);
        _grabManager.OnPointerUp(default);
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == Tags.BattleFloorLayer)
            FloorCollision();
    }

    void FloorCollision()
    {
        _rb.isKinematic = true;
        _audioManager.PlaySFX(_groundHitSound, transform.position);

        float particleObjectCount = Random.Range(2, 6);
        for (int i = 0; i < particleObjectCount; i++)
            SpawnDustParticle();
    }

    void SpawnDustParticle()
    {
        Vector3 pos = new(Random.Range(_collider.bounds.min.x, _collider.bounds.max.x), 0.5f,
                Random.Range(_collider.bounds.min.z, _collider.bounds.max.z));
        Vector3 rotation = new(-90, 0, 0);
        GameObject dust = Instantiate(_dustParticlePrefab, pos, Quaternion.Euler(rotation));
        dust.SetActive(true);
        dust.transform.parent = transform;
        Destroy(dust, 3f);
    }
}
