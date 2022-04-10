using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObstacle : Obstacle, IPushable<Vector3, GameObject, Ability>, IUITextDisplayable
{
    // global
    BattleManager _battleManager;

    // push
    Vector3 _finalPos;
    CharacterStats _targetStats;
    int _damage = 50;

    // display info
    string _displayText = "Boulder, you can move if you know the technique.";

    // Start is called before the first frame update
    void Start()
    {
        _battleManager = BattleManager.instance;
    }

    public void GetPushed(Vector3 dir, GameObject attacker, Ability ability)
    {
        _finalPos = transform.position + dir;

        StartCoroutine(MoveToPosition(_finalPos, 0.5f));

        // TODO: this is quite bad;
        Invoke("CollisionCheck", 0.35f);
    }

    protected IEnumerator MoveToPosition(Vector3 finalPos, float time)
    {
        Vector3 startingPos = transform.position;

        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _battleManager.SnapToGrid(transform);
        UpdateAstar();
    }

    void UpdateAstar()
    {
        // TODO: is that alright? 
        // Recalculate all graphs
        AstarPath.active.Scan();
    }

    void CollisionCheck()
    {
        // check what is in character's new place and act accordingly
        BoxCollider2D boxCol = transform.GetComponentInChildren<BoxCollider2D>();
        boxCol.enabled = false;

        Collider2D col = Physics2D.OverlapCircle(_finalPos, 0.2f);
        if (col == null)
        {
            boxCol.enabled = true;
            return;
        }

        // player/enemy get dmged by 50 and boulder is destroyed
        // character colliders are children
        if (col.transform.gameObject.CompareTag(Tags.PlayerCollider) || col.transform.gameObject.CompareTag(Tags.EnemyCollider))
        {
            _targetStats = col.transform.parent.GetComponent<CharacterStats>();

            _targetStats.TakeDamageNoDodgeNoRetaliation(_damage);

            Destroy(gameObject);
        }
        // boulder is destroyed when it hits another boulder
        else if (col.transform.gameObject.CompareTag(Tags.PushableObstacle))
            Destroy(gameObject);
        // boulder destroys traps
        else if (col.transform.gameObject.CompareTag(Tags.Trap))
            Destroy(col.transform.gameObject);
        // currently you can't target it on the river bank

        if (boxCol != null)
            boxCol.enabled = true;
    }

    public string DisplayText() { return _displayText; }
}
