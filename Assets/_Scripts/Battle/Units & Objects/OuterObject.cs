using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OuterObject : MonoBehaviour
{
    string tweenID;

    public void Initialise(TilemapObject _obj)
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        sr.sprite = _obj.Sprite;
        if (Random.Range(0f, 1f) > 0.5f)
            sr.flipX = true;

        tweenID = _obj.Id;
        if (_obj.IsMoving)
        {
            sr.sortingOrder = 2;
            StartCoroutine(ChangeMovement());
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
