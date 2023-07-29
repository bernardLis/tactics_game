using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ElementalSphere : MonoBehaviour
{
    BattleManager _battleManager;
    Element _element;

    List<BattleEntity> _corpsesToSuckIn = new();

    IEnumerator _runSphereCoroutine;

    public int Corpses;

    public void Initialize(Element e)
    {
        _battleManager = BattleManager.Instance;
        _battleManager.OnOpponentEntityDeath += AddCorpseToList;
        _battleManager.OnPlayerEntityDeath += AddCorpseToList;

        _element = e;
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = _element.Color;

        // bump up and down

        float endY = Random.Range(transform.position.y - 2, transform.position.y + 2);
        transform.DOMoveY(endY, 3f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetId("SphereBounce");

        // increase scale as you suck in corpses
        // add corpses to suck in to a list
        // and float slowly towards them
        // when you get close enough, suck em in
        _runSphereCoroutine = RunSphere();
        StartCoroutine(_runSphereCoroutine);

    }

    void AddCorpseToList(BattleEntity be)
    {
        if (be.Entity.Element != _element) return;
        _corpsesToSuckIn.Add(be);
    }

    IEnumerator RunSphere()
    {
        while (true)
        {
            if (_corpsesToSuckIn.Count > 0)
                yield return FloatTowards(_corpsesToSuckIn[0]);
            yield return new WaitForSeconds(Random.Range(2f, 3f));
        }
    }

    IEnumerator SuckInCorpse(BattleEntity be)
    {
        yield return be.transform.DOMove(transform.position, 1f)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();
        _corpsesToSuckIn.Remove(be);
        Destroy(be.gameObject);
        Corpses++;
    }

    IEnumerator FloatTowards(BattleEntity be)
    {
        Vector3 endPos = new Vector3(be.transform.position.x, transform.position.y, be.transform.position.z);

        // need to check dist ignoring y hmm... this never works
        while (Vector3.Distance(transform.position, endPos) > 0.4f)
        {
            endPos = new Vector3(be.transform.position.x, transform.position.y, be.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, endPos, 0.1f);
            yield return new WaitForFixedUpdate();
        }
        yield return SuckInCorpse(be);
    }


}
