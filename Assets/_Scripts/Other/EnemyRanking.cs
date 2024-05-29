using System.Collections.Generic;
using DG.Tweening;
using Lis.Units.Enemy;
using UnityEngine;

namespace Lis
{
    public class EnemyRanking : MonoBehaviour
    {
        Camera _cam;
        [SerializeField] Canvas _canvasPrefab;
        [SerializeField] List<Enemy> _enemies = new();

        void Start()
        {
            _cam = Camera.main;
            PlaceEnemies();
        }

        void PlaceEnemies()
        {
            _enemies.Sort((e1, e2) => e2.ScarinessRank.CompareTo(e1.ScarinessRank));
            _enemies.Reverse();
            for (int i = 0; i < _enemies.Count; i++)
            {
                Vector3 pos = new(2 + i * 5, 0, 0);
                GameObject enemyObj = Instantiate(_enemies[i].Prefab, pos, Quaternion.identity);
                enemyObj.transform.DOLookAt(_cam.transform.position, 0.5f, AxisConstraint.Y);
                Canvas canvas = Instantiate(_canvasPrefab, pos, Quaternion.Euler(new Vector3(0, 30, 0)));
                canvas.GetComponent<EnemyRankingCanvas>().Initialize(_enemies[i], pos + Vector3.up);
            }
        }
    }
}