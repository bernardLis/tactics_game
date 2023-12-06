using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttack : MonoBehaviour
{
    protected BattleManager _battleManager;
    BattleMinionManager _battleMinionManager;

    List<BattleProjectileOpponent> _projectilePool = new();

    protected BossAttack _attack;
    BattleBoss _battleBoss;
    public virtual void Initialize(BossAttack bossAttack, BattleBoss battleBoss)
    {
        _attack = bossAttack;
        _battleBoss = battleBoss;

        _battleManager = BattleManager.Instance;
        _battleMinionManager = _battleManager.GetComponent<BattleMinionManager>();
        _projectilePool = _battleMinionManager.Projectiles;
    }

    public virtual IEnumerator Attack(int difficulty)
    {
        // Meant to be overwritten
        Debug.Log("Boss Attack");
        yield return new WaitForSeconds(1f);
    }

    protected void SpawnProjectile(Vector3 dir)
    {
        if (_attack.SpecialProjectilePrefab != null)
        {
            SpawnSpecialProjectile(dir, _attack.ProjectileDuration, _attack.ProjectilePower);
            return;
        }
        Vector3 spawnPos = transform.position;
        spawnPos.y = 1f;
        BattleProjectileOpponent p = _projectilePool.Find(x => !x.gameObject.activeSelf);
        p.transform.position = spawnPos;
        p.Initialize(1);
        p.Shoot(_battleBoss, dir, _attack.ProjectileDuration, _attack.ProjectilePower);
    }

    void SpawnSpecialProjectile(Vector3 dir, float time, int power)
    {
        Vector3 spawnPos = transform.position;
        spawnPos.y = 1f;

        GameObject go = Instantiate(_attack.SpecialProjectilePrefab, spawnPos, Quaternion.identity);
        BattleProjectileOpponent p = go.GetComponent<BattleProjectileOpponent>();
        p.Initialize(1);
        p.Shoot(_battleBoss, dir, time, power);
        p.OnDestroy += () => Destroy(go);

    }

}
