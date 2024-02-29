using System.Collections;
using Lis;
using UnityEngine;

public class BattleTrackHeroPosition : MonoBehaviour
{
    Transform _heroTransform;

    IEnumerator _updatePositionCoroutine;

    void OnEnable()
    {
        _heroTransform = BattleManager.Instance.HeroController.transform;
        _updatePositionCoroutine = UpdatePosition();
        StartCoroutine(_updatePositionCoroutine);
    }

    void OnDisable()
    {
        if (_updatePositionCoroutine != null)
            StopCoroutine(_updatePositionCoroutine);
    }

    IEnumerator UpdatePosition()
    {
        while (true)
        {
            if (_heroTransform == null) yield break;
            transform.position = _heroTransform.position;
            yield return new WaitForSeconds(0.1f);
        }
    }
}