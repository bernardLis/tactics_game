using DG.Tweening;
using UnityEngine;

namespace Lis.Map
{
    public class ChestController : MonoBehaviour
    {
        [SerializeField] Transform _lid;

        public void OpenChest()
        {
            _lid.DOLocalRotate(new(-70, 0, 0), 1).SetEase(Ease.OutBack);
        }
    }
}