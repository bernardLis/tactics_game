using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lis.Battle.Tiles;
using Lis.Upgrades;
using UnityEngine;

namespace Lis
{
    public class VisualController : MonoBehaviour
    {
        [SerializeField] protected GameObject GfxHolder;
        [SerializeField] GameObject[] _objectPrefabs;
        [SerializeField] Vector2Int _objectCountRange;
        [SerializeField] int _xRotation;
        [SerializeField] float _yPosition;


        float _objectsScale = 1;
        List<GameObject> _objects = new();

        void Awake()
        {
            GetComponent<Controller>().OnTileEnabled += OnTileEnabled;
            GetComponent<Controller>().OnTileUnlocked += OnTileUnlocked;
        }


        void OnTileEnabled(UpgradeTile upgrade)
        {
            PlaceObjects();
        }

        void PlaceObjects()
        {
            int numberOfObjects = Random.Range(_objectCountRange.x, _objectCountRange.y);
            for (int i = 0; i < numberOfObjects; i++)
            {
                Vector3 position = new(Random.Range(-20f, 20f), _yPosition,
                    Random.Range(-20f, 20f));
                GameObject instance = Instantiate(_objectPrefabs[Random.Range(0, _objectPrefabs.Length)],
                    GfxHolder.transform);
                instance.transform.localPosition = position;
                instance.transform.localRotation =
                    Quaternion.Euler(_xRotation, Random.Range(0, 360), 0);
                _objectsScale = instance.transform.localScale.x;
                instance.transform.localScale = Vector3.zero;
                instance.SetActive(true);
                _objects.Add(instance);
            }
        }

        void OnTileUnlocked(Controller obj)
        {
            StartCoroutine(TileUnlockedCoroutine());
        }

        private IEnumerator TileUnlockedCoroutine()
        {
            yield return new WaitForSeconds(3f);
            foreach (GameObject o in _objects)
            {
                o.transform.DOScale(_objectsScale, 0.5f).SetEase(Ease.OutBounce);
                yield return new WaitForSeconds(Random.Range(0, 0.2f));
            }
        }
    }
}