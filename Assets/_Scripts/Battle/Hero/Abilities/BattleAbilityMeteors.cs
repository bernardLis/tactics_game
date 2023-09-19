using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
public class BattleAbilityMeteors : BattleAbility
{
    [SerializeField] LayerMask _floorLayerMask;

    [SerializeField] GameObject _circlePrefab;    // start lifetime determines how long the circle will be growing (4 seconds now)
    [SerializeField] GameObject _meteorPrefab;

    GameObject _circleInstance;
    GameObject _meteorInstance;

    IEnumerator _updateCirclePositionCoroutine;

    public override void Initialize(Ability ability)
    {
        base.Initialize(ability);
        transform.localPosition = new Vector3(-0.5f, 1f, 0f);
    }

    protected override IEnumerator FireAbilityCoroutine()
    {
        yield return base.FireAbilityCoroutine();

        yield return ManageCircle();
        ManageMeteors();

        for (int i = 0; i < 3; i++) // TODO: hardcoded "duration"
        {
            ExplosionDamage();
            yield return new WaitForSeconds(1f);
        }

        _circleInstance.transform.DOScale(0, 0.5f).OnComplete(() => Destroy(_circleInstance));
        _meteorInstance.transform.DOScale(0, 0.5f).OnComplete(() => Destroy(_meteorInstance));  
    }

    void ExplosionDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(_meteorInstance.transform.position,
                                                        _ability.GetScale() * 0.5f);
        foreach (Collider hit in hitColliders)
        {
            if (hit.TryGetComponent(out BattleEntity be))
            {
                if (be.Team == 0) continue; // TODO: hardcoded team number
                StartCoroutine(be.GetHit(_ability));
            }
        }
    }

    void ManageMeteors()
    {
        _meteorInstance = Instantiate(_meteorPrefab, _circleInstance.transform.position, Quaternion.identity);

        ParticleSystem.ShapeModule shape = _meteorInstance.GetComponent<ParticleSystem>().shape;
        shape.radius = _ability.GetScale() * 0.5f;
        int burstCount = Mathf.FloorToInt(_ability.GetScale() - 7f);
        short burstCountShort = (short)burstCount;
        ParticleSystem.EmissionModule emission = _meteorInstance.GetComponent<ParticleSystem>().emission;
        emission.SetBursts(new ParticleSystem.Burst[] {
                new ParticleSystem.Burst(0f, burstCountShort, burstCountShort, 20, 0.1f)
                });

        _meteorInstance.SetActive(true);
    }

    IEnumerator ManageCircle()
    {
        _circleInstance = Instantiate(_circlePrefab, transform.position, Quaternion.identity);
        _circleInstance.SetActive(true);

        foreach (Transform child in _circleInstance.transform)
        {
            if (child.TryGetComponent(out ParticleSystem ps))
            {
                ParticleSystem.MainModule main = ps.main;
                main.startSize = _ability.GetScale();
            }
        }

        _updateCirclePositionCoroutine = UpdateCirclePosition();
        StartCoroutine(_updateCirclePositionCoroutine);
        yield return new WaitForSeconds(2f); // TODO: hardcoded number - it is the same as in the circle prefab
        StopCoroutine(_updateCirclePositionCoroutine);
    }

    IEnumerator UpdateCirclePosition()
    {
        while (_circleInstance != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, _floorLayerMask))
            {
                Vector3 pos = new(hit.point.x, 0, hit.point.z);
                _circleInstance.transform.position = pos;
            }
            yield return new WaitForFixedUpdate();
        }
    }

}
