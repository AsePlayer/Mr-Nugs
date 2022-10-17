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

    public TMP_Text dialogueText;

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
        
        playerUnit = playerPrefab.GetComponent<Unit>();
        enemyUnit = enemyPrefab.GetComponent<Unit>();

        dialogueText.text = "A wild " + enemyUnit.nameOfUnit + " appeared!";
        
        if(playerPrefab.GetComponent<BattleHUD>()) {
            playerUnit.GetComponent<BattleHUD>().SetHUD(playerUnit);
        }
        if(enemyPrefab.GetComponent<BattleHUD>()) {
            enemyUnit.GetComponent<BattleHUD>().SetHUD(enemyUnit);
        }

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    IEnumerator PlayerAttack() {
        // Damage the enemy
        enemyUnit.GetComponent<Health>().TakeDamage(35, false);
        state = BattleState.ENEMYTURN;
        yield return new WaitForSeconds(2f);

        StartCoroutine(EnemyTurn());

    }
    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.nameOfUnit + " attacks!";

        //playerUnit.TakeDamage(enemyUnit.damage);
        playerUnit.GetComponent<Health>().TakeDamage(enemyUnit.damage, false);
        yield return new WaitForSeconds(1f);

        if(playerUnit.lives <= 0) {
            state = BattleState.LOST;
            //EndBattle();
        }
        else {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }
}
