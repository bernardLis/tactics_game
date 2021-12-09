using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour, IPushable<Vector3, Ability>, IUITextDisplayable
{
    // global
    GameManager gameManager;

    public List<Sprite> stoneSprites;
    SpriteRenderer spriteRenderer;

    // push
    Vector3 finalPos;
    CharacterStats targetStats;
    int damage = 50;

    // display info
    public string displayText = "Boulder, you can move if you know the technique.";

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        // TODO: stones are going to be created by map maker but for now, just for fun.
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = stoneSprites[Random.Range(0, stoneSprites.Count)];

        gameManager.SnapToGrid(transform);
    }

    public void GetPushed(Vector3 _dir, Ability _ability)
    {
        finalPos = transform.position + _dir;

        StartCoroutine(MoveToPosition(finalPos, 0.5f));

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

        // player/enemy get dmged by 50 and boulder is destroyed
        // character colliders are children
        if (col.transform.gameObject.CompareTag("PlayerCollider") || col.transform.gameObject.CompareTag("EnemyCollider"))
        {
            targetStats = col.transform.parent.GetComponent<CharacterStats>();

            targetStats.TakeDamageNoDodgeNoRetaliation(damage);

            Destroy(gameObject);
        }
        // boulder is destroyed when it hits another boulder
        else if (col.transform.gameObject.CompareTag("Stone"))
            Destroy(gameObject);
        // boulder destroys traps
        else if (col.transform.gameObject.CompareTag("Trap"))
            Destroy(col.transform.gameObject);
        // currently you can't target it on the river bank

        if (boxCol != null)
            boxCol.enabled = true;
    }

    public string DisplayText() { return displayText; }
}
