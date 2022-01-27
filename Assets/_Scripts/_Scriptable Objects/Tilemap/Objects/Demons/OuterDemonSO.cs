using UnityEngine;

[CreateAssetMenu(menuName = "Tilemap/OuterDemon")]
public class OuterDemonSO : BaseScriptableObject
{
    public Sprite defaultSprite;
    public Sprite[] alternativeSprites;
    //asdasd
    GameObject obj;

    void Initialize(GameObject _obj)
    {
        obj = _obj;
    }
}
