using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UIElements;

public class ArcMovementElement : VisualElement
{
    IVisualElementScheduledItem _arcMovement;

    Vector3 _startPosition;
    Vector3 _endPosition;

    float _percent;

    public event Action OnArcMovementFinished;
    public ArcMovementElement(VisualElement child, Vector3 startPosition, Vector3 endPosition)
    {
        style.position = Position.Absolute;
        transform.position = startPosition;
        _startPosition = startPosition;
        _endPosition = endPosition;

        Add(child);

        _arcMovement = schedule.Execute(ArcMovement).Every(5);
    }

    void ArcMovement()
    {
        if (_percent >= 1)
            ArcMovementFinished();

        Vector3 p0 = _startPosition;
        float newX = _startPosition.x + (_endPosition.x - _startPosition.x) * 0.5f;
        float newY = _startPosition.y - 200f;

        Vector3 p1 = new Vector3(newX, newY);
        Vector3 p2 = _endPosition;

        // https://www.reddit.com/r/Unity3D/comments/5pyi43/custom_dotween_easetypeeasefunction_based_on_four/
        Vector3 i1 = Vector3.Lerp(p0, p1, _percent); // p1 is the shared handle
        Vector3 i2 = Vector3.Lerp(p1, p2, _percent);
        Vector3 result = Vector3.Lerp(i1, i2, _percent); // lerp between the 2 for the result

        transform.position = result;

        _percent += 0.01f;
    }

    void ArcMovementFinished()
    {
        _arcMovement.Pause();
        OnArcMovementFinished?.Invoke();
    }

}
