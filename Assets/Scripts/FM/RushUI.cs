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

		/*
		// position
		Vector3 posAdjusted = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);

		if (angle >= -135 && angle < -45)
		{
			posAdjusted = new Vector3(pos.x, pos.y + 0.5f, 0f);
		}
		else if (angle >= -45 && angle < 45)
		{
			posAdjusted = new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0f);
		}
		else if ((angle >= 45 && angle < 91) || angle < -225)
		{
			posAdjusted = new Vector3(pos.x, pos.y, 0f);
		}
		else if (angle >= -225 && angle < -135)
		{
			posAdjusted = new Vector3(pos.x - 0.5f, pos.y, 0f);
		}

		// HERE: calc
		Vector2 newPos = currentPos + rushStrength;
		rushStrength += inputVector * rushChargingSpeed * Time.fixedDeltaTime;
		*/

		print(rushVector);

		Vector2 startPosition = RuntimePanelUtils.CameraTransformWorldToPanel(rushUIContainer.panel, pos, cam);
		rushUIContainer.transform.position = startPosition;

		// length
		// TODO: I need to know how much len is the rush vector in the real world
		Vector2 endPosition = new Vector2(pos.x + rushVector.x, pos.y + rushVector.y);


		float len = (Mathf.Abs(rushVector.x) + Mathf.Abs(rushVector.y)) * 50; // TODO: this is incorrect
		rushUI.style.width = len;
	}

	public void HideRushUI()
	{
		rushUIContainer.style.display = DisplayStyle.None;
	}


}
