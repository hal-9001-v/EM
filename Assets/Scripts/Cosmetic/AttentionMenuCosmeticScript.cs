using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttentionMenuCosmeticScript : MonoBehaviour
{
    float amplitude = 0.2f;
    Vector3  scale;
    void Update ()
    {
        for (int i = 0; i < 2; ++i) scale[i] = amplitude * Mathf.Sin(Time.time) + 1;
        transform.localScale = scale;
    }

}
