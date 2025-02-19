using System.Collections;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // probably make a stage var, and also move this to gamemanager perhaps
    private GameObject enemyFolder;

    private int spellIndex = 0;
    private int targetIndex = 0;

    private int enemyIndex = 0;
    private int selectionIndex = 0;

    private Spellbook playerSpellbook;

    private Spell spellToCast;
    private Spell spellToSteal;

    private bool selectedSpell = false;
    private bool selectedSteal = false;

    private bool inCombat = true;

    public float cooldownBetweenActions = 1.0f;
    private bool onCooldown = false;

    private bool paused = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSpellbook = GetComponent<Spellbook>();

        GameManager.playerInput = this;

        // TODO: change where this is?
        enemyFolder = GameManager.instance.GetEnemyFolder();
    }

    // Update is called once per frame
    void Update()
    {
        if (!onCooldown && !paused)
        {
            HandlePlayerInput();
        } 
        else
        {
            Debug.Log("onCooldown is " + onCooldown);
            Debug.Log("paused is" + paused);
        }
    }

    public string GetSpell()
    {
        if (spellToCast == null || !selectedSpell)
        {
            return playerSpellbook.spellList[spellIndex].name;
        }
        return spellToCast.name;
    }

    public Spell GetSpellToCast()
    {
        return spellToCast;
    }

    public Spell GetSpellToSteal()
    {
        if (spellToSteal == null || !selectedSteal)
        {
            return enemyFolder.transform.GetChild(enemyIndex).gameObject.GetComponent<Spellbook>().spellList[selectionIndex];
        }
        return spellToSteal;
    }

    public Transform GetTargetLocation()
    {
        return enemyFolder.transform.GetChild(targetIndex);
    }

    public int GetSpellIndex()
    {
        return spellIndex;
    }

    public bool GetSelectedSpell() 
    { 
        return selectedSpell; 
    }

    public string GetTarget()
    {
        if (!selectedSpell)
        {
            return "";
        }

        if (spellToCast.targetType == "Self" || spellToCast.targetType == "All Enemies")
        {
            return spellToCast.targetType;
        }

        return enemyFolder.transform.GetChild(targetIndex).name;
    }

    public void ToggleInCombat()
    {
        inCombat = !inCombat;

        // TODO: if a spell expires we need to make sure to move everything down (sort method)
        // TODO: dont need to set all of these each time
        spellIndex = 0;
        targetIndex = 0;

        enemyIndex = 0;
        selectionIndex = 0;

        selectedSpell = false;
        selectedSteal = false;
}

    public bool CheckInCombat()
    {
        return inCombat;
    }

    public string GetEnemyName()
    {
        if (enemyFolder != null)
        {
            return enemyFolder.transform.GetChild(enemyIndex).name;
        }
        else
        {
            return "";
        }
    }

    public void Pause(bool setting)
    {
        paused = setting;
    }

    // Stages of selection in combat: Choosing a spell, then choosing target
    // Stages of selection after combat: Choosing a spell/campfire option
    // Other than those 2 things as well as esc/game over menu, player input should be disabled
    void HandlePlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!inCombat)
            {
                if (selectionIndex == 0)
                {
                    if (enemyIndex > 0)
                    {
                        enemyIndex--;
                        selectionIndex = enemyFolder.transform.GetChild(enemyIndex).gameObject.GetComponent<Spellbook>().spellList.Count - 1;
                    }
                }
                else
                {
                    selectionIndex--;
                }
            }
            else if (!selectedSpell) {
                if (spellIndex > 0)
                {
                    spellIndex--;
                }
            }
            else if (spellToCast.targetType == "Single Target")
            {
                Debug.Log("Single Target A");
                int moveToIndex = targetIndex - 1;
                while (moveToIndex >= 0 && !enemyFolder.transform.GetChild(moveToIndex).GetComponent<HasHealth>().GetStatus())
                {
                    moveToIndex--;
                }
                if (moveToIndex >= 0)
                {
                    targetIndex = moveToIndex;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (!inCombat)
            {
                if (selectionIndex == enemyFolder.transform.GetChild(enemyIndex).gameObject.GetComponent<Spellbook>().spellList.Count - 1)
                {
                    if (enemyIndex < enemyFolder.transform.childCount - 1)
                    {
                        enemyIndex++;
                        selectionIndex = 0;
                    }
                }
                else
                {
                    selectionIndex++;
                }
            }
            else if (!selectedSpell)
            {
                if (spellIndex < playerSpellbook.spellList.Count && playerSpellbook.spellList[spellIndex + 1] != null)
                {
                    spellIndex++;
                }
            }
            else if (spellToCast.targetType == "Single Target")
            {
                Debug.Log("Single Target D");
                int moveToIndex = targetIndex + 1;
                Debug.Log("target index: " + targetIndex);
                Debug.Log("moveto index start: " + moveToIndex);
                while (moveToIndex < enemyFolder.transform.childCount && !enemyFolder.transform.GetChild(moveToIndex).GetComponent<HasHealth>().GetStatus())
                {
                    moveToIndex++;
                }
                Debug.Log("moveto index: " + moveToIndex);
                if (moveToIndex < enemyFolder.transform.childCount)
                {
                    targetIndex = moveToIndex;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!inCombat)
            {
                if (!selectedSteal)
                {
                    // select a spell to steal
                    spellToSteal = enemyFolder.transform.GetChild(enemyIndex).gameObject.GetComponent<Spellbook>().spellList[selectionIndex];
                    selectedSteal = true;
                }

                // TODO: if no available slots, must select a spell to replace (do later)
                if (playerSpellbook.GetEmptySlots() > 0)
                {
                    playerSpellbook.StealSpell(enemyFolder.transform.GetChild(enemyIndex).gameObject.GetComponent<Spellbook>().spellList[selectionIndex]);
                    selectedSteal = false;
                    // call stage manager
                    GameManager.instance.StageManager();
                }
                else
                {

                }
            }
            else if (!selectedSpell)
            {
                spellToCast = playerSpellbook.spellList[spellIndex];
                selectedSpell = true;
                // TODO: change where this is?
                enemyFolder = GameManager.instance.GetEnemyFolder();
                Debug.Log("There are " + enemyFolder.transform.childCount + " Enemies");
            }
            else
            {
                StartCoroutine(HandlePlayerAction());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            selectedSpell = false;
        }
    }

    IEnumerator HandlePlayerAction()
    {
        onCooldown = true;
        selectedSpell = false;

        Debug.Log("Player casts " + spellToCast.name + "!");
        yield return StartCoroutine(spellToCast.CastSpell(gameObject, enemyFolder.transform.GetChild(targetIndex).gameObject));

        float initialTime = Time.time;
        while (Time.time - initialTime < cooldownBetweenActions)
        {
            yield return null;
        }

        playerSpellbook.UpdateCastThisTurn();
        onCooldown = false;
    }
}
