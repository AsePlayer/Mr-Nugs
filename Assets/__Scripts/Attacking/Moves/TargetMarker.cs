using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMarker : MonoBehaviour
{

    static float scaleScalelol = 1;        // Size multiplier of ring - Creates pulsating effect. Shared by all rings
    static float scaleTimer = 0;           // Creates calculation for scaleScale
    static bool counterExists = false;     // Keeps track if there is a TargetMarker script incrementing the scaleTimer

    private bool isCounter = false;        // True if this script is incrementing the scaleTimer
    public Vector2 defaultScale;           // Base scale of this ring

    // Update is called once per frame
    void Update()
    {
        // Becomes new designated counter if none exists
        if (!counterExists)
        {
            isCounter = true;
            counterExists = true;
        }

        if (isCounter)
        {
            scaleTimer += Time.deltaTime * 3;
            scaleScalelol = 1.15f + Mathf.Sin(scaleTimer)*0.15f;
        }

        transform.localScale = defaultScale * scaleScalelol;
    }

    private void OnDestroy()
    {
        if (isCounter)
        {
            isCounter = false;
            counterExists = false;
        }
    }
}
