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

        void Awake()
        {
            GetComponent<Controller>().OnTileEnabled += OnTileEnabled;
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
                instance.SetActive(true);
            }
        }
    }
}