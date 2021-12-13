using System.Collections;
using UnityEngine;

public class FootholdTrap : MonoBehaviour, IPushable<Vector3, Ability>, IUITextDisplayable
{
    // global
    GameManager gameManager;

    // push
    public int damage = 50;
    public bool isPushed;
    Vector3 finalPos;
    CharacterStats targetStats;

    // display text
    public string displayText = "Foothold trap. Damage to enemies: 50.";

    void Start()
    {
        gameManager = GameManager.instance;
        gameManager.SnapToGrid(transform);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (isPushed)
            return;

        if (!col.transform.CompareTag("EnemyCollider"))
            return;


/*
        // TODO: needs a rewrite with enemies
        // functionality when enemy walks onto the trap
        GameObject enemy = col.transform.parent.gameObject;
        EnemyCharMovementController enemyCharMovementController = enemy.GetComponent<EnemyCharMovementController>();
        if (enemyCharMovementController != null)
        {
            // trap is triggered
            enemyCharMovementController.TrappedOnTheWay(damage, transform.position);

            // trap no longer collides
            // TODO: swap gfx with triggered sprite
            transform.GetComponentInChildren<BoxCollider2D>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
        */
    }

    public void GetPushed(Vector3 _dir, Ability _ability)
    {
        finalPos = transform.position + _dir;

        StartCoroutine(MoveToPosition(finalPos, 0.5f));
        isPushed = true;

        // TODO: this is quite bad;
        Invoke("CollisionCheck", 0.35f);
        // TODO: reset should be after trap stops moving and not at 1s randomly...
        Invoke("ResetPushed", 1f);
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

        gameManager.SnapToGrid(transform);
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

        Collider2D col = Physics2D.OverlapCircle(finalPos, 0.2f);
        if (col == null)
        {
            boxCol.enabled = true;
            return;
        }

        // trap is triggered on player/enemy
        // character colliders are children
        // enemy triggers trap from trap script
        if (col.transform.gameObject.CompareTag("PlayerCollider") || col.transform.gameObject.CompareTag("EnemyCollider"))
        {
            targetStats = col.transform.parent.GetComponent<CharacterStats>();

            targetStats.TakeDamageNoDodgeNoRetaliation(damage);
            // movement range is down by 1 for each trap enemy walks on
            //targetStats.movementRange.AddModifier(-1);

            Destroy(gameObject);
        }
        // trap is destroyed when it hits a boulder
        else if (col.transform.gameObject.CompareTag("Stone"))
            Destroy(gameObject);
        // one trap is destroyed when it hits another traps
        else if (col.transform.gameObject.CompareTag("Trap"))
            Destroy(gameObject);
        // currently you can't target it on the river bank
    }

    void ResetPushed() { isPushed = false; }

    public string DisplayText() { return displayText; }

}
