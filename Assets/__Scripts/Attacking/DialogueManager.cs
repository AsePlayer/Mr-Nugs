using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI battleDialogue;

    public void insertBattleDialogue(string text)
    {
        battleDialogue.text = text;
    }

    public void clearBattleDialogue()
    {
        battleDialogue.text = "";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
