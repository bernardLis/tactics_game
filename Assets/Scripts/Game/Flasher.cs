using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Flasher : MonoBehaviour
{
    SpriteRenderer rend;

    public void StartFlashing(Color col)
    {
        Color startColor = col * 0.7f;
        Color endColor = col * 0.6f; // https://docs.unity3d.com/ScriptReference/Color-operator_multiply.html

        rend = GetComponent<SpriteRenderer>();

        rend.color = startColor;
        transform.DOScale(0.9f, 1f).SetEase(Ease.InOutBack).SetLoops(-1, LoopType.Yoyo);
        rend.DOColor(endColor, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopFlashing()
    {
        // TODO: errors
        
        DOTween.Kill(transform);
        gameObject.SetActive(false);
        Invoke("SelfDestroy", 1f);
    }

    void SelfDestroy()
    {
        Destroy(gameObject);

    }

    //Material mat;
    /*
	void Start()
	{

		//mat = GetComponent<Renderer>().material;
	}
    */


    /*

	IEnumerator Flash(Color col)
	{
		mat.color = col;

		float flashTimer = 0f;
		while (true)
		{
			// *2 coz I want it to flash quicker 
			// +0.2f coz I don't want it to disapear completly
			float a = 1.3f - Mathf.PingPong(flashTimer / 2f, 1f);
			a = Mathf.Clamp(a, 0f, 1f);
			col.a = a;
			mat.color = col;
			//mat.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(flashTimer, 1f));
			flashTimer += Time.deltaTime;
			yield return null;
		}
	}
    */

}
