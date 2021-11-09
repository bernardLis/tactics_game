using System.Collections.Generic;
using UnityEngine;

public class StoneScript : MonoBehaviour
{
	public List<Sprite> stoneSprites;
	SpriteRenderer spriteRenderer;
	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		int num = Random.Range(0, stoneSprites.Count);
		spriteRenderer.sprite = stoneSprites[num];
	}
}
