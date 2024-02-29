using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lis.Core;
using UnityEngine;

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
        [SerializeField] List<PortalElement> _portalElements = new();
        [SerializeField] GameObject _blackPortal;

        GameObject _portal;

        float _delay;
        bool _portalShown;
        AudioSource _portalHumSource;

        public List<BattleEntity> SpawnedEntities = new();

        public event Action<BattleEntity> OnSpawnComplete;

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

        public void SpawnEntity(Entity entity, BattleEntity battleEntity, int team)
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
            Vector3 pos = transform.position;

            battleEntity.transform.position = pos;
            battleEntity.gameObject.SetActive(true);
            entity.InitializeBattle(team);
            battleEntity.InitializeEntity(entity, team);
            SpawnedEntities.Add(battleEntity);

            _audioManager.PlaySFX(_portalPopEntitySound, pos);

            Vector3 scale = battleEntity.transform.localScale;
            Vector3 jumpPos = pos + transform.forward * scale.z;
            jumpPos.y = scale.y;

            yield return battleEntity.transform.DOJump(jumpPos, 1f, 1, 0.5f)
                .WaitForCompletion();
            OnSpawnComplete?.Invoke(battleEntity);

            yield return new WaitForSeconds(1f);
            DisableSelf();
        }

        void DisableSelf()
        {
            _audioManager.PlaySFX(_portalCloseSound, transform.position);
            if (_portalHumSource != null)
                _portalHumSource.Stop();

            transform.DOScale(0, 0.5f).SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    transform.localScale = Vector3.one;
                    _portal.SetActive(false);
                    gameObject.SetActive(false);
                });
        }
    }

    [Serializable]
    public struct PortalElement
    {
        public ElementName ElementName;
        public GameObject Portal;
    }
}