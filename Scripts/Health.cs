using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    private Unit unit;
    public List<int> morsels = new List<int>();
    [SerializeField] GameObject dmgNum;


    // Start is called before the first frame update
    void Start()
    {
        // Initial health setup
        if(GetComponent<Unit>() != null)
        {
            unit = GetComponent<Unit>();
        
            for (int i = 0; i < unit.lives; i++)
            {
                morsels.Add(unit.health);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(morsels.Count > 0)
            unit.health = morsels[morsels.Count - 1];

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
            GameObject dmg = Instantiate(dmgNum);
            dmg.transform.SetParent(GameObject.Find("Canvas").transform);
            dmg.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            dmg.GetComponent<RectTransform>().transform.position = gameObject.transform.position;
            dmg.GetComponent<DmgNum>().displaydmg(-damage);
        }

        
        // Remove morsel if HP too low!
        if(morsels[morsels.Count - 1] <= 0) {
            morsels.RemoveAt(morsels.Count - 1);

            if (unit.lives > 0)
            {
                unit.lives--;
            }
            else
            {
                // Die
                Destroy(gameObject);
            }
        }
    } 

}
