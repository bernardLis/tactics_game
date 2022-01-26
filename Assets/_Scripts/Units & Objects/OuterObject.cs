using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class OuterObject : MonoBehaviour
{
    public Light2D spotLightPrefab;
    string tweenID;

    public void Initialise(TilemapObject _obj)
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = _obj.sprite;
        if (Random.Range(0f, 1f) > 0.5f)
            sr.flipX = true;

        tweenID = _obj.id;
        if (_obj.isMoving)
        {
            sr.sortingOrder = 2;
            StartCoroutine(ChangeMovement());
        }

        if (_obj.emitsLight)
        {
            Light2D l = Instantiate(spotLightPrefab, transform.position, Quaternion.identity);
            l.transform.parent = transform;
            l.color = _obj.lightColor;
            l.intensity = Random.Range(_obj.lightIntensity.x, _obj.lightIntensity.y);
            l.pointLightInnerRadius = Random.Range(_obj.innerRadius.x, _obj.innerRadius.y);
            l.pointLightOuterRadius = Random.Range(_obj.outerRadius.x, _obj.outerRadius.y);
        }
    }

    void MoveRandomly()
    {

        float x = Random.Range(0f, 1f);
        float y = Random.Range(0f, 1f);
        float multiplier = Random.Range(0.1f, 0.3f);
        Vector3 direction = new Vector3(x * multiplier, y * multiplier);

        float duration = Random.Range(2f, 5f);
        transform.DOPunchPosition(direction, duration, 0, 1f, false).SetLoops(3, LoopType.Yoyo).SetId(tweenID); // TODO: cool! 
    }

    IEnumerator ChangeMovement()
    {
        while (true)//you can add a variable that enables you to kill routine
        {
            MoveRandomly();
            yield return new WaitForSeconds(Random.Range(10f, 20f));
        }
    }


}
