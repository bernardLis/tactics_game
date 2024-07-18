using UnityEngine;
using DG.Tweening;

namespace Lis.Map
{
    public class PlayerController : MonoBehaviour
    {
        public NodeController CurrentNode;


        public void MoveTo(NodeController nodeController)
        {
            CurrentNode.Deactivate();

            CurrentNode = nodeController;
            transform.DOMove(nodeController.transform.position, 2f)
                .OnComplete(() => { CurrentNode.Activate(); });
        }
    }
}