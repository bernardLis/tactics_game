using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RushUI : MonoBehaviour
{
	Camera cam;

	// UI
	UIDocument UIDocument;
	VisualElement rushUIContainer;
	VisualElement rushUI;
	VisualElement rushUIArrow;

	void Awake()
	{
		// TODO: Supposedly, this is an expensive call
		cam = Camera.main;

		UIDocument = GetComponent<UIDocument>();
		// getting ui elements
		var root = UIDocument.rootVisualElement;

		rushUIContainer = root.Q<VisualElement>("rushUIContainer");
		rushUI = root.Q<VisualElement>("rushUI");
		rushUIArrow = root.Q<VisualElement>("rushUIArrow");
	}

	public void DrawRushUI(Vector3 pos, Vector2 rushVector, float percentToLimit)
	{
		rushUIContainer.style.display = DisplayStyle.Flex;

		// showing over the limit color
		rushUI.style.backgroundColor = Color.Lerp(Color.white, Color.red, percentToLimit);
		rushUIArrow.style.backgroundColor = Color.Lerp(Color.white, Color.red, percentToLimit);

		// TODO: this UI sucks.
		// direction
		Vector2 dir = rushVector.normalized;
		float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg - 90;
		rushUIContainer.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		Vector2 startPosition = RuntimePanelUtils.CameraTransformWorldToPanel(rushUIContainer.panel, pos, cam);
		rushUIContainer.transform.position = startPosition;

		// length
		Vector3 endPos = new Vector3(pos.x + rushVector.x, pos.y + rushVector.y, pos.z);
		Vector2 endPosition = RuntimePanelUtils.CameraTransformWorldToPanel(rushUIContainer.panel, endPos, cam);

		float distance = Vector2.Distance(startPosition, endPosition);
		rushUI.style.width = distance;
	}

	public void HideRushUI()
	{
		rushUIContainer.style.display = DisplayStyle.None;
	}
}
