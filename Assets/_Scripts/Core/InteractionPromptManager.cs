using DG.Tweening;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.Core
{
    public class InteractionPromptManager : Singleton<InteractionPromptManager>
    {
        readonly string _interactionPromptTweenID = "_interactionPromptTweenID";
        VisualElement _interactionPromptContainer;
        VisualElement _interactionPromptButton;
        Label _interactionPromptLabel;

        // Start is called before the first frame update
        void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            _interactionPromptContainer = root.Q<VisualElement>("interactionPromptContainer");
            _interactionPromptButton = root.Q<VisualElement>("interactionPromptButton");
            _interactionPromptLabel = root.Q<Label>("interactionPromptLabel");
        }


        /* INTERACTION PROMPT */
        public void ShowInteractionPrompt(string txt)
        {
            _interactionPromptLabel.text = txt;
            _interactionPromptContainer.style.display = DisplayStyle.Flex;
            _interactionPromptContainer.style.opacity = 0;
            DOTween.Kill(_interactionPromptTweenID);
            DOTween.To(x => _interactionPromptContainer.style.opacity = x, 0, 1, 0.5f)
                .SetEase(Ease.InOutSine);

            DOTween.To(x => _interactionPromptButton.transform.scale = x * Vector3.one,
                    1, 1.1f, 0.5f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine).SetId(_interactionPromptTweenID);
        }

        public void HideInteractionPrompt()
        {
            DOTween.Kill(_interactionPromptTweenID);
            DOTween.To(x => _interactionPromptContainer.style.opacity = x, 1, 0, 0.5f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => _interactionPromptContainer.style.display = DisplayStyle.None)
                .SetId(_interactionPromptTweenID);
        }
    }
}