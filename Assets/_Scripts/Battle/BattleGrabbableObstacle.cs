using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using DG.Tweening;
using MoreMountains.Feedbacks;

public class BattleGrabbableObstacle : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] GameObject _dustParticlePrefab;
    BattleGrabManager _grabManager;
    bool _wasGrabbed;

    Rigidbody _rb;
    Collider _collider;
    MMF_Player _feelPlayer;
    Material _material;

    Color _defaultColor;
    Color _endColor = new Color(0.875f, 0.32f, 0.28f, 1f); // reddish

    int _maxGrabbingTime = 5;
    int _secondsToBreak;

    IEnumerator _cooldownCoroutine;

    void Start()
    {

        _secondsToBreak = _maxGrabbingTime;
        _grabManager = BattleGrabManager.Instance;
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _feelPlayer = GetComponent<MMF_Player>();
        _material = GetComponent<Renderer>().material;
        _defaultColor = _material.color;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (_secondsToBreak <= 0)
        {
            DisplayText("No more grabbing!", Color.red);
            return;
        }
        if (!_grabManager.IsGrabbingAllowed()) return;
        if (_cooldownCoroutine != null) StopCoroutine(_cooldownCoroutine);
        _grabManager.TryGrabbing(gameObject);

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

        float particleObjectCount = Random.Range(2, 6);
        for (int i = 0; i < particleObjectCount; i++)
            SpawnDustParticle();

    }
    void SpawnDustParticle()
    {
        Vector3 pos = new(Random.Range(_collider.bounds.min.x, _collider.bounds.max.x), 0.5f,
                Random.Range(_collider.bounds.min.z, _collider.bounds.max.z));
        Vector3 rotation = new Vector3(-90, 0, 0);
        GameObject dust = Instantiate(_dustParticlePrefab, pos, Quaternion.Euler(rotation));
        dust.SetActive(true);
    }
}
