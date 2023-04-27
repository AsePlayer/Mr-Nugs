using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteFade : MonoBehaviour
{
    // Start is called before the first frame update
    public int fadeValue = 1; 
    [SerializeField] public Slider fader; 

    private Material materialToChange;
    void Start()
    {
        materialToChange = gameObject.GetComponent<Renderer>().material;
        StartCoroutine(LerpFunction(fadeValue, 5));

    }

    IEnumerator LerpFunction(int endValue, float duration)
    {
        float time = 0;
        // int startValue = 0;
        while (time < duration)
        {
            // materialToChange.Fade = Mathf.Lerp(startValue, endValue, time / duration);
            // Fade the materialToChange

            time += Time.deltaTime;
            yield return null;
        }
        // materialToChange.Fade = endValue;
        // https://answers.unity.com/questions/968423/material-that-can-fade-from-opaque-to-transparent.html
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
