using UnityEngine;

public class MapReset : MonoBehaviour
{
    bool _isResetting;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isResetting && other.CompareTag("PlayerCollider"))
            Reset();
    }
    void Reset()
    {
        // TODO: need to remove player chars
        BattleManager.instance.GetComponent<BoardManager>().GenerateMap();
    }
}
