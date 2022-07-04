using UnityEngine;

public class MapReset : MonoBehaviour
{
    bool _isResetting;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isResetting && other.CompareTag(Tags.Player))
            Reset();
    }
    void Reset()
    {
        // TODO: need to remove player chars
        BattleManager.Instance.GetComponent<BoardManager>().GenerateMap();
    }
}
