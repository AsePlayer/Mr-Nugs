using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    private Health health; // Health script of the unit
    public List<Slider> morselSlider = new List<Slider>(); // Data structure to hold all the sliders
    public Slider morselPrefab; // Prefab of the sliders
    [SerializeField] GameObject personalCanvas;

    // Start is called before the first frame update
    void Start() 
    {
        // Instantiate new canvas
        Canvas canvas = new GameObject("Canvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.transform.localScale = new Vector3(0.02f, 0.02f, 0);

        // Set the canvas to be a child of the unit
        personalCanvas = canvas.gameObject;
        personalCanvas.transform.SetParent(transform);
        personalCanvas.transform.position = transform.position + new Vector3(0, 2, 0);


        if(gameObject.TryGetComponent(out Health h))
        {
            // If something has a BattleHUD, there should be a Health script attached to it!
            health = h;
        }
    }
    public void SetHUD(Unit unit)
    {
        foreach (Slider m in morselSlider)
        {
            Destroy(m.gameObject);
        }
        morselSlider.Clear();
        for (int i = 0; i < health.getActualLives(); i++)
        {
            // Instantiate a new morsel
            morselSlider.Add(Instantiate(morselPrefab, transform));
            // Set morsels to the HUD parent
            morselSlider[i].transform.SetParent(personalCanvas.transform, false);
            // Place morsels in the correct position
            morselSlider[i].transform.position = personalCanvas.transform.position;
            
            // Set the morsel's value to the unit's health
            morselSlider[i].maxValue = unit.health;
            morselSlider[i].minValue = 0;
            morselSlider[i].value = health.GetMorsels()[i];
        }
        SetMorselPositioning();
    }

    private void SetMorselPositioning()
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
