using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscilateScale : MonoBehaviour
{
    bool isOscilating;
    bool isScalingUp;


    // Update is called once per frame
    void Update()
    {
        if (isOscilating && Time.frameCount % 60 == 0)
            Oscilate();
    }

    public void SetOscilation(bool _isOscilating)
    {
        isOscilating = _isOscilating;
    }

    void Oscilate()
    {
        if (isScalingUp)
            transform.localScale += Vector3.one * 0.01f;
        else
            transform.localScale -= Vector3.one * 0.01f;

        if (transform.localScale.x > 1.03f)
            isScalingUp = false;
        else if (transform.localScale.x < 0.98f)
            isScalingUp = true;
    }

}
