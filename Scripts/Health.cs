using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int lives;
    public List<int> morsels = new List<int>();
    [SerializeField] GameObject dmgNum;

    // Start is called before the first frame update
    void Start()
    {
        // Initial health setup
        if(GetComponent<Unit>() != null)
        {
            health = GetComponent<Unit>().health;
            lives = GetComponent<Unit>().lives;
            for (int i = 0; i < lives; i++)
            {
                morsels.Add(health);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(morsels.Count > 0)
            health = morsels[morsels.Count - 1];

    }

    // if clicked take damage (test)
    private void OnMouseDown()
    {
        TakeDamage(33, true);
    }

    public void TakeDamage(int damage, bool showDamage)
    {
        // Take damage
        morsels[morsels.Count - 1] -= damage;

        // Display damage numbers
        if (showDamage)
        {
            Debug.Log(gameObject.transform.position.x + " " + gameObject.transform.position.y);
            GameObject dmg = Instantiate(dmgNum);//, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y), Quaternion.identity);
            dmg.transform.SetParent(GameObject.Find("Canvas").transform);
            dmg.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            dmg.GetComponent<DmgNum>().displaydmg(-damage);
        }
        if (lives >= 0)
        {
            lives--;
            health = morsels[morsels.Count - 1];
        }
        else
        {
            // Die
            Destroy(gameObject);
        }
    }

}
