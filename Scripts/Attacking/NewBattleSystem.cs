using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBattleSystem : MonoBehaviour
{
    // Battle Start Variables
    private List<GameObject> friendlies;        // List of all active friendly units
    private List<GameObject> enemies;           // List of all active enemy units

    // Battle State Variables
    private bool lockControl = true;           // true = player cannot do actions, other than the move they are currently executing
    private bool battleStillGoing = false;      // Battle will end when false
    private bool playerTurn = false;            // true = player's turn, false = enemy's turn

    // Control Variables
    private int currentMoveIndex;               // The current move selected by the player (move list index)
    private GameObject selectedCharacter;       // The current character being controlled by the player
    private Unit selectedCharacterUnit;         // The unit script of the character being controlled by the character
    private GameObject currentMove;             // The gameobject of the move being displayed
    enum Actions { Move };                      // All possible actions for a player turn.
    private Actions currentAction;              // Current action being executed by the player. Keeps track of what the player controls should be doing.

    // UI
    private DialogueManager dialogueManager;    // UI controller



    // Outside scripts will call this function to begin a battle
    public void StartBattle(List<GameObject> toFriendlies, List<GameObject> toEnemies, bool playerFirst)
    {
        // fill lists
        friendlies = toFriendlies;
        enemies = toEnemies;

        // Set up health bars
        setHUDS();

        // Set starting params
        dialogueManager = GetComponentInChildren<DialogueManager>();
        currentMoveIndex = 10000; // Change this to remember last move used or something later
        playerTurn = playerFirst;
        battleStillGoing = true;
        selectedCharacter = friendlies[0];
        selectedCharacterUnit = selectedCharacter.GetComponent<Unit>();

        StartCoroutine(BattleLoop());
    }

    // Main loop for every battle
    IEnumerator BattleLoop()
    {
        while (battleStillGoing)
        {
            if (playerTurn)
            {
                yield return StartCoroutine(PlayerTurn());
            }
            if (!playerTurn)
            {
                yield return StartCoroutine(EnemyTurn());
            }
        }
        
    }

    // Sets up the player turn
    IEnumerator PlayerTurn()
    {
        lockControl = true;
        dialogueManager.insertBattleDialogue("It is team Nugs's turn.");
        yield return new WaitForSeconds(2);

        bool selectedPlayer = false;

        // Give each living friendly 1 action, and choose a default selected character + set up their actions
        foreach (GameObject friend in friendlies)
        {
            if (friend.TryGetComponent(out Unit u))
            {
                if (!u.dead)
                {
                    u.usedTurn = false;
                    if (!selectedPlayer)
                    {
                        selectedCharacter = friend;
                        selectedCharacterUnit = selectedCharacter.GetComponent<Unit>();
                        selectedPlayer = true;
                        u.usedTurn = false;

                        // Once more actions like items are added, the action menu would be brought up here
                        currentMove = Instantiate(u.moves[currentMoveIndex % u.moves.Count]);
                        currentMove.transform.parent = gameObject.transform;
                        currentMove.GetComponent<Move>().execute(selectedCharacter);
                        // Debug.Log("Default selected character: " + selectedCharacter.name + "\nDefault selected move: " + currentMove.name);

                        // There will be more enums once more actions are added. For now, there's just moves.
                        currentAction = Actions.Move;
                    }
                }
            }
            else
            {
                Debug.Log("Why doesn't " + friend.name + " have a Unit script?");
            }
        }

        lockControl = false;

        // When a friendly uses their turn, they will call friendlyUsedTurn(),
        // which will set playerTurn to false once all friendlies have used their turn.
        yield return new WaitUntil(() => !playerTurn);
    }

    IEnumerator EnemyTurn()
    {
        lockControl = true;
        dialogueManager.insertBattleDialogue("It is team Baddie's turn.");
        yield return new WaitForSeconds(2);

        playerTurn = true;
        // When an enemy uses their turn, they will call enemyUsedTurn(),
        // which will set playerTurn to true once all friendlies have used their turn.
        yield return new WaitUntil(() => playerTurn);
    }

    // Checks for deaths on both teams. 
    // Signals end of battle if a team has won.
    // Starts next turn if team is out of moves.
    IEnumerator CheckBattleStatus()
    {
        bool allFriendliesDead = true;
        bool allFriendlyTurnsUsed = true;

        foreach (GameObject friend in friendlies)
        {
            if (friend.TryGetComponent(out Unit u))
            {
                // Check to see if a friendly died this turn
                if (u.morsels.Count <= 0 && !u.dead)
                {
                    dialogueManager.insertBattleDialogue(u.name + " has tragically passed away...");
                    u.dead = true;
                    u.usedTurn = true;
                    yield return new WaitForSeconds(2);
                    dialogueManager.insertBattleDialogue("");
                    u.transform.Rotate(0, 0, 90);
                }
                // Check to see if at least 1 friendly is alive
                else if (u.morsels.Count > 0)
                {
                    allFriendliesDead = false;
                    if (!u.usedTurn)
                    {
                        allFriendlyTurnsUsed = false;
                    }
                }
            }
            else Debug.Log(friend.name + " has no Unit script.");
        }

        if (allFriendliesDead)
        {
            dialogueManager.insertBattleDialogue("You have lost the battle...");
            battleStillGoing = false;
            yield break;
        }

        bool allEnemiesDead = true;
        bool allEnemyTurnsUsed = true;

        foreach(GameObject enemy in enemies)
        {
            if (enemy.TryGetComponent(out Unit u))
            {
                // Check to see if an enemy died this turn
                if (u.morsels.Count <= 0 && !u.dead)
                {
                    dialogueManager.insertBattleDialogue(u.name + " has awesomely passed away...");
                    u.dead = true;
                    u.usedTurn = true;
                    yield return new WaitForSeconds(2);
                    dialogueManager.insertBattleDialogue("");
                    u.transform.Rotate(0, 0, 90);

                }
                // Check to see if at least 1 enemy is alive
                else if (u.morsels.Count > 0)
                {
                    allEnemiesDead = false;
                    if (!u.usedTurn)
                    {
                        allEnemyTurnsUsed = false;
                    }
                }
            }
            else Debug.Log(enemy.name + " has no Unit script.");
        }

        if (allEnemiesDead)
        {
            dialogueManager.insertBattleDialogue("You have won the battle!!!");
            battleStillGoing = false;
            yield break;
        }

        // Change turns
        if (allFriendlyTurnsUsed && allEnemyTurnsUsed)
        {
            playerTurn = !playerTurn;
        }
    }

    // Sets up HUDs for all enemy and friendly units
    private void setHUDS()
    {
        // Set up HUDs (friendly units)
        foreach (GameObject unit in friendlies)
        {
            if (unit.TryGetComponent(out Unit u) && u.TryGetComponent(out BattleHUD b))
            {
                b.SetHUD(u);
            }
            else Debug.Log("For some odd reason, " + unit.name + " seems to be missing either a BattleHUD or Unit script...");
        }

        // Set up HUDs (enemy units)
        foreach (GameObject unit in enemies)
        {
            if (unit.TryGetComponent(out Unit u) && u.TryGetComponent(out BattleHUD b))
            {
                b.SetHUD(u);
            }
            else Debug.Log("For some odd reason, " + unit.name + " seems to be missing either a BattleHUD or Unit script...");
        }
    }

    // When a friendly unit finishes a turn, this function is called.
    // It displays the move dialogue, and calls CheckBattleStatus.
    public IEnumerator friendlyUsedTurn(GameObject user, string text)
    {
        lockControl = true;
        setHUDS();
        dialogueManager.insertBattleDialogue(text);
        if (user.TryGetComponent(out Unit u))
        {
            u.usedTurn = true;
        }
        else Debug.Log("Could not find User script for " + user.name);

        yield return new WaitForSeconds(1);

        dialogueManager.clearBattleDialogue();

        yield return StartCoroutine(CheckBattleStatus());
    }

    // When an enemy unit finishes a turn, this function is called
    // It displays the move dialogue, and calls CheckBattleStatus.
    public IEnumerator enemyUsedTurn(GameObject user, string text)
    {
        setHUDS();
        dialogueManager.insertBattleDialogue(text);
        if (user.TryGetComponent(out Unit u))
        {
            u.usedTurn = true;
        }
        else Debug.Log("Could not find User script for " + user.name);

        yield return new WaitForSeconds(1);

        dialogueManager.clearBattleDialogue();

        yield return StartCoroutine(CheckBattleStatus());
    }

    // Update is called once per frame
    void Update()
    {
        if (!lockControl)
        {
            if (currentAction == Actions.Move)
            {
                if (Input.mouseScrollDelta.y == 1 || Input.GetKeyDown(KeyCode.E))
                {
                    currentMove.GetComponent<Move>().cancel();
                    currentMoveIndex++;
                    currentMove = Instantiate(selectedCharacterUnit.moves[currentMoveIndex % selectedCharacterUnit.moves.Count]);
                    currentMove.transform.parent = gameObject.transform;
                    currentMove.GetComponent<Move>().execute(selectedCharacterUnit.gameObject);
                }
                else if (Input.mouseScrollDelta.y == -1 || Input.GetKeyDown(KeyCode.Q))
                {
                    currentMove.GetComponent<Move>().cancel();
                    currentMoveIndex--;
                    currentMove = Instantiate(selectedCharacterUnit.moves[currentMoveIndex % selectedCharacterUnit.moves.Count]);
                    currentMove.transform.parent = gameObject.transform;
                    currentMove.GetComponent<Move>().execute(selectedCharacterUnit.gameObject);
                }
            }
        }
    }
}
