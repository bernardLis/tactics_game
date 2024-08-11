using DG.Tweening;
using UnityEngine;

namespace Lis.Map
{
    public class ChestController : MonoBehaviour
    {
        [SerializeField] Transform _lid;
        [SerializeField] GameObject _particleSystem;

        public void OpenChest()
        {
            _lid.DOLocalRotate(new(-70, 0, 0), 0.5f).SetEase(Ease.OutBack)
                .OnComplete(() => { _particleSystem.SetActive(true); });
        }
    }
}