using Lis.Units.Enemy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lis.Other
{
    public class EnemyRankingCanvas : MonoBehaviour
    {
        [SerializeField] TMP_Text _name;
        [SerializeField] TMP_Text _scariness;
        [SerializeField] Image _nature;

        public void Initialize(Enemy e, Vector3 pos)
        {
            _name.text = e.name;
            _scariness.text = $"Scariness: {e.ScarinessRank}";
            _nature.sprite = e.Nature.Icon;
            transform.position = pos;
        }
    }
}