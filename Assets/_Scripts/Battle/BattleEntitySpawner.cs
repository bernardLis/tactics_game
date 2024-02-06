using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lis
{
    public class BattleEntitySpawner : MonoBehaviour
    {
        AudioManager _audioManager;

        [SerializeField] Sound _portalOpenSound;
        [SerializeField] Sound _portalCloseSound;
        [SerializeField] Sound _portalHumSound;
        [SerializeField] Sound _portalPopEntitySound;

        Element _portalElement;
        List<Entity> _entities = new();
        [SerializeField] List<PortalElement> _portalElements = new();
        [SerializeField] GameObject _blackPortal;

        GameObject _portal;

        float _delay;
        bool _portalShown;
        AudioSource _portalHumSource;

        public List<BattleEntity> SpawnedEntities = new();

        public event Action<List<BattleEntity>> OnSpawnComplete;

        void Awake()
        {
            _audioManager = AudioManager.Instance;
        }

        public void ShowPortal(Element element, Vector3 scale = default)
        {
            if (_portalShown) return;
            _portalShown = true;

            Vector3 position = transform.position;
            _audioManager.PlaySFX(_portalOpenSound, position);
            _portalHumSource = _audioManager.PlaySFX(_portalHumSound, position, true);
            _portal = _blackPortal;
            if (element != null)
                _portal = _portalElements.Find(x => x.ElementName == element.ElementName).Portal;

            if (scale == default) scale = Vector3.one * 2f;

            _portal.transform.localScale = Vector3.zero;
            _portal.SetActive(true);
            _portal.transform.DOScale(scale, 0.5f)
                .SetEase(Ease.OutBack);
        }

        public void NewSpawnEntity(Entity entity, BattleEntity battleEntity, int team)
        {
            SpawnedEntities = new();
            _portalShown = false;
            gameObject.SetActive(true);
            ShowPortal(entity.Element);

            StartCoroutine(SpawnCoroutine(entity, battleEntity, team));
        }

        IEnumerator SpawnCoroutine(Entity entity, BattleEntity battleEntity, int team)
        {
            yield return new WaitForSeconds(0.6f);

            battleEntity.gameObject.SetActive(true);
            entity.InitializeBattle(team);
            battleEntity.InitializeEntity(entity, team);
            SpawnedEntities.Add(battleEntity);

            Vector3 position = transform.position;
            _audioManager.PlaySFX(_portalPopEntitySound, position);

            Vector3 jumpPos = position + transform.forward * battleEntity.transform.localScale.z;
            if (_entities.Count > 1)
                jumpPos += Vector3.left * Random.Range(-2, 2);
            jumpPos.y = battleEntity.transform.localScale.y;

            yield return battleEntity.transform.DOJump(jumpPos, 1f, 1, 0.5f)
                .WaitForCompletion();
            OnSpawnComplete?.Invoke(SpawnedEntities);

            yield return new WaitForSeconds(1f);
            DisableSelf();
        }

        public void SpawnEntities(List<Entity> entities, Element portalElement = null,
            float duration = 2f, int team = 0)
        {
            _entities = new(entities);
            _portalElement = portalElement;
            _delay = duration / _entities.Count;

            StartCoroutine(SpawnShow(team));
        }

        IEnumerator SpawnShow(int team)
        {
            ShowPortal(_portalElement);

            foreach (Entity t in _entities)
            {
                SpawnEntity(t, team);
                yield return new WaitForSeconds(_delay);
            }

            OnSpawnComplete?.Invoke(SpawnedEntities);
            Invoke(nameof(DestroySelf), 1f);
        }

        void SpawnEntity(Entity entity, int team)
        {
            Vector3 position = transform.position;
            _audioManager.PlaySFX(_portalPopEntitySound, position);

            entity.InitializeBattle(team);

            GameObject instance = Instantiate(entity.Prefab, position, transform.localRotation);
            BattleEntity be = instance.GetComponent<BattleEntity>();
            be.InitializeEntity(entity, team);
            SpawnedEntities.Add(be);

            Vector3 jumpPos = position +
                              transform.forward * (Random.Range(2, 5) * instance.transform.localScale.z);
            if (_entities.Count > 1)
                jumpPos += Vector3.left * Random.Range(-2, 2);
            jumpPos.y = instance.transform.localScale.y;

            instance.transform.DOJump(jumpPos, 1f, 1, 0.5f);
        }

        public void ClearSpawnedEntities()
        {
            SpawnedEntities.Clear();
        }

        void DisableSelf()
        {
            _audioManager.PlaySFX(_portalCloseSound, transform.position);
            if (_portalHumSource != null)
                _portalHumSource.Stop();

            transform.DOScale(0, 0.5f).SetEase(Ease.InBack)
                .OnComplete(() => gameObject.SetActive(false));
        }

        public void DestroySelf()
        {
            _audioManager.PlaySFX(_portalCloseSound, transform.position);
            if (_portalHumSource != null)
                _portalHumSource.Stop();

            transform.DOScale(0, 0.5f).SetEase(Ease.InBack);
            Destroy(gameObject, 1f);
        }
    }

    [Serializable]
    public struct PortalElement
    {
        public ElementName ElementName;
        public GameObject Portal;
    }
}