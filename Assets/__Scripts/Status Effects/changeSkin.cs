using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeSkin : MonoBehaviour
{
    public int skinNum;
    public Skins[] skins;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SkinChoice(); 
    }

    void Update()
    {
        if (skinNum > skins.Length - 1) skinNum = 0;
        else if (skinNum < 0) skinNum = skins.Length - 1;
    }

    void SkinChoice()
    {
        if (spriteRenderer.sprite.name.Contains("NuggetMain"))
        {
            string spriteName = spriteRenderer.sprite.name;
            spriteName = spriteName.Replace("NuggetMain_", "");
            int spriteNum = int.Parse(spriteName);

            spriteRenderer.sprite = skins[skinNum].sprites[spriteNum]; 
        }
    }
}

[System.Serializable]

public struct Skins
{
    public Sprite[] sprites; 
}