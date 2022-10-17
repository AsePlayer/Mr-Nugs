using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DmgNum : MonoBehaviour
{
    [SerializeField] float maxTime = 2.0f; //Time that number stays on screen
    [SerializeField] float speed = 0.25f;
    float currentTime = 0f; //Time that number has been on screen

    [SerializeField] TextMeshProUGUI tmp; //The gameObject's TMP component
    [SerializeField] Rigidbody2D rb; //The gameObject's rigidbody component

    public void displaydmg(int dmg)
    {
        tmp.text = "" + dmg;
        rb.velocity = new Vector2(speed, speed);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float ratio = currentTime / maxTime;

        currentTime += Time.deltaTime;

        Color fade = tmp.color;
        fade.a = 1.0f - (ratio);
        tmp.color = fade;

        rb.velocity = new Vector2(speed - (ratio * speed), speed - (ratio * speed));

        if (currentTime >= maxTime)
        {
            Destroy(gameObject);
        }
    }
}
