using UnityEngine;
using DG.Tweening;
using Lis.Core.Utilities;

namespace Lis.Map
{
    public class PlayerController : Singleton<PlayerController>
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