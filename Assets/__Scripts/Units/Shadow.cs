using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite.color = new Color(1f, 1f, 1f, .5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
