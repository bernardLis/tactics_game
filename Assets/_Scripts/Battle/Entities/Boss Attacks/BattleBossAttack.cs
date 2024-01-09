using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossAttack : MonoBehaviour
{
    protected BattleManager _battleManager;
    BattleProjectileManager _battleProjectileManager;

    protected BossAttack _attack;
    BattleBoss _battleBoss;
    public virtual void Initialize(BossAttack bossAttack, BattleBoss battleBoss)
    {
        _attack = bossAttack;
        _battleBoss = battleBoss;

        _battleManager = BattleManager.Instance;

        _battleProjectileManager = _battleManager.GetComponent<BattleProjectileManager>();
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
        BattleProjectileOpponent p = _battleProjectileManager.GetObjectFromPool();
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
        p.OnExplode += () => Destroy(go);

    }

}
