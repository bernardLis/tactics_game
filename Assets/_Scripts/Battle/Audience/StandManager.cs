using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Battle.Audience
{
    public class StandManager : MonoBehaviour
    {
        [SerializeField] GameObject _memberPrefab;
        [SerializeField] GameObject _standGfx;
        BoxCollider _standCollider;

        void Start()
        {
            _standCollider = _standGfx.GetComponent<BoxCollider>();
            PopulateAudience();
        }

        void PopulateAudience()
        {
            for (int i = 0; i < Random.Range(20, 40); i++)
            {
                Vector3 pos = Helpers.RandomPointInBounds(_standCollider.bounds);
                pos.y = 1.5f + transform.position.y;

                GameObject member = Instantiate(_memberPrefab, transform);
                member.transform.position = pos;
                member.GetComponent<MemberController>().Initialize();
            }
        }
    }
}