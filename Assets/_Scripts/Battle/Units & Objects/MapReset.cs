using UnityEngine;

public class MapReset : MonoBehaviour
{
    bool reset;
    void OnTriggerEnter2D(Collider2D _other)
    {
        if (!reset && _other.CompareTag("PlayerCollider"))
            Reset();
    }
    void Reset()
    {
        BattleManager.instance.GetComponent<BoardManager>().GenerateMap();
    }
}
