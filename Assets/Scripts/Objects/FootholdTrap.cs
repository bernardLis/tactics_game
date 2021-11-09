using UnityEngine;

public class FootholdTrap : MonoBehaviour
{
    EnemyCharMovementController enemyCharMovementController;
    public int damage = 50;
    public bool pushed;
    public void OnTriggerEnter2D(Collider2D col)
    {
        if (pushed)
            return;

        if (!col.transform.CompareTag("EnemyCollider"))
            return;

        GameObject enemy = col.transform.parent.gameObject;
        enemyCharMovementController = enemy.GetComponent<EnemyCharMovementController>();
        if (enemyCharMovementController != null)
        {
            // trap is triggered
            enemyCharMovementController.TrappedOnTheWay(damage, transform.position);

            // trap no longer collides
            // TODO: swap gfx with triggered sprite
            transform.GetComponent<BoxCollider2D>().enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
