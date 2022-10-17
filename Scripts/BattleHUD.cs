using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public List<Slider> morsels = new List<Slider>();

    public Slider morselPrefab;
    public Canvas canvas;
    
    public void SetHUD(Unit unit)
    {
        // Make HUD parent for GameObject
        var parent = new GameObject();
        parent.name = unit.name + " HUD";

        // Set parent's position to unit GameObject
        parent.transform.position = unit.gameObject.transform.position;

        // Make HUD a child of Canvas so it appears on screen!
        parent.transform.SetParent(canvas.transform, false);
        
        for (int i = 0; i < unit.lives; i++)
        {
            // Instantiate a new morsel
            morsels.Add(Instantiate(morselPrefab, transform));
            // Set morsels to the HUD parent
            morsels[i].transform.SetParent(parent.transform, false);
            // Place morsels in the correct position
            morsels[i].transform.position = parent.transform.position;
            // Balance morsel position
            SetMorselPositioning();

            // Set the morsel's value to the unit's health
            morsels[i].maxValue = unit.health;
            morsels[i].minValue = 0;
            morsels[i].value = unit.health;
        }
    }

    public void SetMorselPositioning()
    {
        Vector3 offset = new Vector3(0, 2, 0);
        for (int i = 0; i < morsels.Count; i++)
        {
            if(morsels.Count == 1) {
                offset = new Vector3(0, 2, 0);
            }
            else {
                offset += new Vector3(1, 0, 0);
            }
            morsels[i].transform.position = transform.root.position + offset - new Vector3((float)morsels.Count/2.0f + 0.5f, 0, 0);
        }

    }

    public void Update()
    {
        SetMorselPositioning();

        // Update morsel values
        for (int i = 0; i < morsels.Count; i++)
        {
            morsels[i].value = transform.root.GetComponent<Health>().morsels[i];
        }
    }
}
