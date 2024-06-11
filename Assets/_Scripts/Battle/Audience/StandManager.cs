using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Battle.Audience
{
    public class StandManager : MonoBehaviour
    {
        [SerializeField] private GameObject _memberPrefab;
        private BoxCollider _spawnCollider;

        private void Start()
        {
            _spawnCollider = GetComponentInChildren<BoxCollider>();
            PopulateAudience();
        }

        private void PopulateAudience()
        {
            for (int i = 0; i < Random.Range(5, 10); i++)
            {
                Vector3 pos = Helpers.RandomPointInBounds(_spawnCollider.bounds);
                pos.y = 5.3f + transform.position.y;

                GameObject member = Instantiate(_memberPrefab, transform);
                member.transform.position = pos;
                member.GetComponent<MemberController>().Initialize();
            }
        }
    }
}