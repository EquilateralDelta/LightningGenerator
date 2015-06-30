using UnityEngine;
using System.Collections;

public class FadeScript : MonoBehaviour 
{
    public float fadeSpeed;
    public float delay;

    float visibility;
    Color baseColor;
    float delayTimer;

	void Start () 
    {
        visibility = 1;
        if (light != null)
            baseColor = light.color;
        else
            baseColor = renderer.material.color;

        delayTimer = Time.time + delay;
	}

	void Update () 
    {
        if (Time.time > delayTimer)
            visibility -= fadeSpeed;

        if (light != null)
            light.color = baseColor * visibility;
        else
            renderer.material.color = baseColor * visibility;

        if (visibility <= 0)
            Destroy(gameObject);
	}
}
