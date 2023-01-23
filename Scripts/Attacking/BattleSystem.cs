using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    Unit playerUnit;
    Unit enemyUnit;

    public List<Unit> enemyUnits = new List<Unit>();

    public TMP_Text dialogueText;

    private GameObject currentMove;
    private int currentMoveIndex = 0;

    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        //Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        //Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        state = BattleState.PLAYERTURN;
        
        // playerUnit = playerPrefab.GetComponent<Unit>();
        // enemyUnit = enemyPrefab.GetComponent<Unit>();

        // dialogueText.text = "A wild " + enemyUnit.nameOfUnit + " appeared!";

        // Find all objects with component battlehud
        Unit[] units = FindObjectsOfType<Unit>();

        foreach (Unit unit in units)
        {
            unit.GetComponent<BattleHUD>().SetHUD(unit);

            // if tagged player
            if (unit.tag == "Player")
            {
                playerUnit = unit;
            }
            else
            {
                enemyUnits.Add(unit);
            }

            currentMoveIndex = unit.moves.Count * 5000;
        }
        
        // if(playerPrefab.GetComponent<BattleHUD>()) {
        //     playerUnit.GetComponent<BattleHUD>().SetHUD(playerUnit);
        // }
        // if(enemyPrefab.GetComponent<BattleHUD>()) {
        //     enemyUnit.GetComponent<BattleHUD>().SetHUD(enemyUnit);
        // }

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
        // Create movement circle
        playerUnit.GetComponent<MovementCircle>().SetupCircle(playerUnit.speed);    // Setup Movement Circle

        // Start with the first move
        currentMove = Instantiate(playerUnit.moves[currentMoveIndex%playerUnit.moves.Count]);
        currentMove.transform.parent = gameObject.transform;
        currentMove.GetComponent<Move>().execute(playerUnit.gameObject);

    }

    public void NewMovementCircle()
    {
        // print("new circle yo");
        playerUnit.GetComponent<MovementCircle>().DestroyCircle();
        playerUnit.GetComponent<MovementCircle>().SetupCircle(playerUnit.speed);
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerAttack() {
        // Damage the enemy
        // enemyUnit.GetComponent<Health>().TakeDamage(35, false);
        // Knockback the enemy with rigidbody2d
        // enemyUnit.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 0), ForceMode2D.Impulse);

        state = BattleState.ENEMYTURN;
        playerUnit.GetComponent<MovementCircle>().DestroyCircle();                  // Destroy Movement Circle
        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());

    }
    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.nameOfUnit + " attacks!";

        enemyUnit.GetComponent<MovementCircle>().SetupCircle(enemyUnit.speed);      // Setup Movement Circle

        //playerUnit.TakeDamage(enemyUnit.damage);
        playerUnit.GetComponent<Health>().TakeDamage(enemyUnit.damage, false);

        yield return new WaitForSeconds(1f);
        enemyUnit.GetComponent<MovementCircle>().DestroyCircle();                 // Destroy Movement Circle
        if(playerUnit.lives <= 0) {
            state = BattleState.LOST;
            //EndBattle();
        }
        else {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    private void Update()
    {
        if (Input.mouseScrollDelta.y == 1 || Input.GetKeyDown(KeyCode.E))
        {
            currentMove.GetComponent<Move>().cancel();
            currentMoveIndex++;
            currentMove = Instantiate(playerUnit.moves[currentMoveIndex % playerUnit.moves.Count]);
            currentMove.transform.parent = gameObject.transform;
            currentMove.GetComponent<Move>().execute(playerUnit.gameObject);
        }
        else if (Input.mouseScrollDelta.y == -1 || Input.GetKeyDown(KeyCode.Q))
        {
            currentMove.GetComponent<Move>().cancel();
            currentMoveIndex--;
            currentMove = Instantiate(playerUnit.moves[currentMoveIndex % playerUnit.moves.Count]);
            currentMove.transform.parent = gameObject.transform;
            currentMove.GetComponent<Move>().execute(playerUnit.gameObject);
        }

        // look for all enemies. if none are found, spawn one
        Unit[] units = FindObjectsOfType<Unit>();
        
        if(units.Length == 1) {
            Unit newEnemySpawned = Instantiate(enemyPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Unit>();
            newEnemySpawned.GetComponent<BattleHUD>().SetHUD(newEnemySpawned);
        }

    }
}
