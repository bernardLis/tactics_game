using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

public class DamageUI : MonoBehaviour
{
    Camera cam;

    UIDocument UIDocument;
    float offsetY = 0.5f;

    VisualElement healthChangeDisplayContainer;
    Label healthChangeDisplayLabel;

    void Awake()
    {
        // TODO: Supposedly, this is an expensive call
        cam = Camera.main;

        UIDocument = GetComponent<UIDocument>();
        // getting ui elements
        var rootVisualElement = UIDocument.rootVisualElement;
        healthChangeDisplayContainer = rootVisualElement.Q<VisualElement>("HealthChangeDisplayContainer");
        healthChangeDisplayLabel = rootVisualElement.Q<Label>("HealthChangeDisplayLabel");
    }

    public void DisplayDamage(int damage)
    {
        healthChangeDisplayLabel.text = "" + damage;
        healthChangeDisplayLabel.style.fontSize = 24;
        healthChangeDisplayLabel.style.color = new Color(1f, 0.42f, 0.42f, 1f);
        healthChangeDisplayContainer.style.display = DisplayStyle.Flex;

        StartCoroutine(DisplayHealthChangeCoroutine());
    }

    public void DisplayHeal(int healthGain)
    {
        healthChangeDisplayLabel.text = "" + healthGain;
        healthChangeDisplayLabel.style.fontSize = 24;
        healthChangeDisplayLabel.style.color = new Color(0.42f, 1f, 0.42f, 1f);
        healthChangeDisplayContainer.style.display = DisplayStyle.Flex;

        StartCoroutine(DisplayHealthChangeCoroutine());
    }

    public void DisplayText(string _txt)
    {
        healthChangeDisplayLabel.text = _txt;
        healthChangeDisplayLabel.style.fontSize = 12;
        healthChangeDisplayLabel.style.color = new Color(1f, 1f, 1f, 1f);
        healthChangeDisplayContainer.style.display = DisplayStyle.Flex;

        StartCoroutine(DisplayHealthChangeCoroutine());
    }

    IEnumerator DisplayHealthChangeCoroutine()
    {
        // set position of the element 
        Vector3 middleOfTheTile = new Vector3(transform.position.x, transform.position.y + offsetY, transform.position.z);
        Vector2 newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(healthChangeDisplayContainer.panel, middleOfTheTile, cam);

        float x = newPosition.x; //  - healthChangeDisplayContainer.resolvedStyle.width / 2; <- 0 on the first frame
        float y = newPosition.y; // - healthChangeDisplayContainer.layout.height; <- height is 0, coz it does not calculate in the first frame after element creation

        healthChangeDisplayContainer.transform.position = new Vector3(x, y, transform.position.z);

        // move up
        float waitTime = 0;
        float fadeTime = 1f;
        float i = 0.1f;
        while (waitTime < 1f)
        {
            middleOfTheTile = new Vector3(transform.position.x, transform.position.y + offsetY, transform.position.z);
            newPosition = RuntimePanelUtils.CameraTransformWorldToPanel(healthChangeDisplayContainer.panel, middleOfTheTile, cam);

            // TODO: i am calculating x so many times is because size won't be computed on the first frame after creating an element
            // https://forum.unity.com/threads/how-to-get-the-actual-width-and-height-of-an-uielement.820266/
            x = newPosition.x - healthChangeDisplayContainer.resolvedStyle.width / 2;
            y = newPosition.y - i;
            i += 0.1f;

            healthChangeDisplayContainer.transform.position = new Vector3(x, y, transform.position.z);

            yield return null;
            waitTime += Time.deltaTime / fadeTime;
        }

        healthChangeDisplayContainer.style.display = DisplayStyle.None;
    }

}
