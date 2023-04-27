using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBattleTest : MonoBehaviour
{
    [SerializeField] List<GameObject> friendlies;
    [SerializeField] List<GameObject> enemies;

    float timer = 0;
    bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 0.1)
        {
            timer += Time.deltaTime;
        }
        else if (!done)
        {
            gameObject.GetComponent<NewBattleSystem>().StartBattle(friendlies, enemies, true);
            done = true;
        }
    }
}
