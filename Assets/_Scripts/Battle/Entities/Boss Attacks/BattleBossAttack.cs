using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttack : MonoBehaviour
{
    protected BattleManager _battleManager;
    protected BattleBossManager _battleBossManager;

    List<BattleProjectileOpponent> _projectilePool = new();

    BossAttack _bossAttack;
    BattleBoss _battleBoss;
    public virtual void Initialize(BossAttack bossAttack, BattleBoss battleBoss)
    {
        _bossAttack = bossAttack;
        _battleBoss = battleBoss;

        _battleManager = BattleManager.Instance;
        _battleBossManager = _battleManager.GetComponent<BattleBossManager>();
        _projectilePool = _battleBossManager.Projectiles;
    }

    public virtual IEnumerator Attack(int difficulty)
    {
        // Meant to be overwritten
        Debug.Log("Boss Attack");
        yield return new WaitForSeconds(1f);
    }

    protected void SpawnProjectile(Vector3 dir, float time, int power)
    {
        if (_bossAttack.SpecialProjectilePrefab != null)
        {
            SpawnSpecialProjectile(dir, time, power);
            return;
        }
        Vector3 spawnPos = transform.position;
        spawnPos.y = 1f;
        BattleProjectileOpponent p = _projectilePool.Find(x => !x.gameObject.activeSelf);
        p.transform.position = spawnPos;
        p.Initialize(1);
        p.Shoot(_battleBoss, dir, time, power);
    }

    void SpawnSpecialProjectile(Vector3 dir, float time, int power)
    {
        Vector3 spawnPos = transform.position;
        spawnPos.y = 1f;

        GameObject go = Instantiate(_bossAttack.SpecialProjectilePrefab, spawnPos, Quaternion.identity);
        BattleProjectileOpponent p = go.GetComponent<BattleProjectileOpponent>();
        p.Initialize(1);
        p.Shoot(_battleBoss, dir, time, power);
        p.OnDestroy += () => Destroy(go);

    }

}
