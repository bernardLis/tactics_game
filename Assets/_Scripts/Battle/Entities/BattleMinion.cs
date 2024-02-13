using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleMinion : BattleEntity
    {
        Minion _minion;

        [SerializeField] GameObject _earthGfx;
        [SerializeField] GameObject _fireGfx;
        [SerializeField] GameObject _waterGfx;
        [SerializeField] GameObject _windGfx;


        BattleHero _targetHero;

        public override void InitializeEntity(Entity entity, int team)
        {
            if (Gfx != null) Gfx.SetActive(true);
            if (entity.Element.ElementName == ElementName.Earth) _earthGfx.SetActive(true);
            if (entity.Element.ElementName == ElementName.Fire) _fireGfx.SetActive(true);
            if (entity.Element.ElementName == ElementName.Water) _waterGfx.SetActive(true);
            if (entity.Element.ElementName == ElementName.Wind) _windGfx.SetActive(true);

            base.InitializeEntity(entity, team);
            _minion = (Minion)entity;

            // minion pool
            IsDead = false;
            IsDeathCoroutineStarted = false;
            Collider.enabled = true;

            Agent.speed = _minion.Speed.GetValue() + _minion.Level.Value * Random.Range(0.1f, 0.2f);

            _targetHero = BattleManager.GetComponent<BattleHeroManager>().BattleHero;
            StartRunEntityCoroutine();
        }
        
        protected override IEnumerator RunEntity()
        {
            if (IsDead) yield break;

            Gfx.transform.localScale = Vector3.one;
            Gfx.transform.localPosition = Vector3.zero; // idk, gfx moves up for some reason

            yield return PathToHero();
        }

        IEnumerator PathToHero()
        {
            Agent.stoppingDistance = 0.7f;
            yield return PathToTarget(_targetHero.transform);

            // something is blocking path, so just die...
            if (Vector3.Distance(transform.position, _targetHero.transform.position) > 2.5f)
            {
                StartCoroutine(Die(hasLoot: false));
                yield break;
            }

            ReachedHero();
        }

        void ReachedHero()
        {
            StartCoroutine(Attack());
        }

        IEnumerator Attack()
        {
            Gfx.transform.DOPunchScale(Vector3.one * 1.1f, 0.2f, 1, 0.5f);
            StartCoroutine(_targetHero.GetHit(_minion));
            yield return new WaitForSeconds(0.5f);
            StartRunEntityCoroutine();
        }

        public override IEnumerator Die(BattleEntity attacker = null, bool hasLoot = true)
        {
            yield return base.Die(attacker, hasLoot);
            Gfx.SetActive(false);
            StopAllCoroutines();
            yield return new WaitForSeconds(1f);

            _earthGfx.SetActive(false);
            _fireGfx.SetActive(false);
            _waterGfx.SetActive(false);
            _windGfx.SetActive(false);

            gameObject.SetActive(false);
        }
    }
}