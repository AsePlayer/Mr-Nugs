using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    private Health health; // Health script of the unit
    public List<Slider> morselSlider = new List<Slider>(); // Data structure to hold all the sliders
    public Slider morselPrefab; // Prefab of the sliders
    public Canvas canvas; // Canvas to hold the sliders

    // Start is called before the first frame update
    void Start() 
    {
        if(GetComponent<Health>() != null)
        {
            // If something has a BattleHUD, there should be a Health script attached to it!
            health = GetComponent<Health>();
        }
    }
    public void SetHUD(Unit unit)
    {
        // Make HUD parent for GameObject
        var parent = new GameObject();
        parent.name = unit.name + " HUD";

        // Set parent's position to unit GameObject
        parent.transform.position = unit.gameObject.transform.position;

        // Make HUD a child of Canvas so it appears on screen! TODO: Give units a personal canvas!
        parent.transform.SetParent(canvas.transform, false);
        
        for (int i = 0; i < unit.lives; i++)
        {
            // Instantiate a new morsel
            morselSlider.Add(Instantiate(morselPrefab, transform));
            // Set morsels to the HUD parent
            morselSlider[i].transform.SetParent(parent.transform, false);
            // Place morsels in the correct position
            morselSlider[i].transform.position = parent.transform.position;
            // Balance morsel position
            SetMorselPositioning();

            // Set the morsel's value to the unit's health
            morselSlider[i].maxValue = unit.health;
            morselSlider[i].minValue = 0;
            morselSlider[i].value = unit.health;
        }
    }

    public void SetMorselPositioning()
    {       
        Vector3 offset = new Vector3(0, 2, 0); // Initial offset for the morsel's y-axis. TODO: Base this off of the Unit's height?
        for (int i = 0; i < morselSlider.Count; i++)
        {
            offset += new Vector3(1, 0, 0); // Offset to stop lives from overlapping on the x-axis
            morselSlider[i].transform.position = transform.position + offset - new Vector3((float)morselSlider.Count / 2.0f + 0.5f, 0, 0);
        }
    }

    public void Update()
    {
        // Update morsel values
        for (int i = 0; i < health.GetMorsels().Count; i++)
        {
            morselSlider[i].value = health.GetMorsels()[i];
        }
    
        // If the BattleHUD morsel count is greater than the Health Component morsel count, a morsel was lost! 
        // Update the HUD and remove it from the game scene.
        if(morselSlider.Count > health.GetMorsels().Count)
        {
            Slider emptyMorsel = morselSlider[morselSlider.Count-1]; // Cache morsel to delete
            morselSlider.RemoveAt(morselSlider.Count - 1); // Remove morsel from list
            Destroy(emptyMorsel.gameObject); // Remove morsel from game scene
        }

        if(morselSlider.Count > 0)
            SetMorselPositioning(); // Update morsel positions. TODO: When Canvas becomes a child of the unit, this will be unnecessary!
        
        
    }

    public void Cleanup() 
    {
        // Destroy all morsels when unit dies!
        foreach (Slider morsel in morselSlider)
        {
            Destroy(morsel.gameObject);
        }
    }
}
