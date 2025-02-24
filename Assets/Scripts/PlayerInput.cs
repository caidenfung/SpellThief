using Mono.Cecil.Cil;
using System.Collections;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // probably make a stage var, and also move this to gamemanager perhaps
    private GameObject enemyFolder;

    private int spellIndex = 0;
    private int targetIndex = 0;

    private int selectionIndex = 0;

    private Spellbook playerSpellbook;

    private Spell spellToCast;
    private Spell spellToSteal;

    // In combat: We select a spell, then select an enemy
    private bool selectedSpell = false;
    // Out of combat: We select an enemy, then select a spell
    private bool selectedEnemy = false;

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
    }

    public void ToggleInCombat()
    {
        inCombat = !inCombat;

        selectedSpell = false;
        selectedEnemy = false;

        // TODO: if a spell expires we need to make sure to move everything down (sort method)
        // or we need to make sure the spell is pointed to a valid index at start of round

        // TODO: dont need to set all of these each time, since we toggleCombat twice
        spellIndex = 0;
        if (inCombat)
        {
            spellIndex = -1;
            ChangeSelection(1, "spell");
        }
        targetIndex = 0;
        
        selectionIndex = 0;
}

    public bool CheckInCombat()
    {
        return inCombat;
    }

    public string GetEnemyName()
    {
        if (enemyFolder != null)
        {
            return enemyFolder.transform.GetChild(targetIndex).name;
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
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!selectedSpell || selectedEnemy)
            {
                ChangeSelection(-1, "spell");
            }
            else
            {
                ChangeSelection(-1, "target");
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (!selectedSpell || selectedEnemy)
            {
                ChangeSelection(1, "spell");
            }
            else
            {
                ChangeSelection(1, "target");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            MakeSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (selectedEnemy)
            {
                selectedEnemy = false;
            }
            else
            {
                selectedSpell = false;
            }
        }
    }

    // TODO: Can probably make this more elegant
        // Selecting a spell: based on what we've selected, use either playerspellbook or enemyspellbook
        // Selecting a target: should overlap either way, but if we're out of combat we can select defeated enemies
    // moveAmount should only be either -1 or 1
    public void ChangeSelection(int moveAmount, string selectionType)
    {
        bool isValid = false;

        // Selecting a spell
        if (selectionType == "spell")
        {
            Spellbook selectSpellbook;

            // Selecting spell we want to steal from
            if (!inCombat)
            {
                selectSpellbook = enemyFolder.transform.GetChild(targetIndex).gameObject.GetComponent<Spellbook>();
            }
            // Selecting a spell we want to cast
            else
            {
                selectSpellbook = playerSpellbook;
            }

            while (!isValid)
            {
                spellIndex += moveAmount;
                if (spellIndex == selectSpellbook.spellList.Count)
                {
                    spellIndex = 0;
                }
                else if (spellIndex < 0)
                {
                    spellIndex = selectSpellbook.spellList.Count - 1;
                }

                if (selectSpellbook.spellList[spellIndex] != null)
                {
                    isValid = true;
                }
            }
        }
        else if (selectionType == "target")
        //else if (spellToCast.targetType == "Single Target")
        {
            // keep iterating until we find an enemy that is alive
            while (!isValid)
            {
                targetIndex += moveAmount;
                if (targetIndex == enemyFolder.transform.childCount)
                {
                    targetIndex = 0;
                }
                else if (targetIndex < 0)
                {
                    targetIndex = enemyFolder.transform.childCount - 1;
                }

                // in combat, enemies must be alive, but outside of combat it doesn't matter because we're selecting rewards
                if (!inCombat || enemyFolder.transform.GetChild(targetIndex).GetComponent<HasHealth>().GetStatus())
                {
                    isValid = true;
                }
            }
        }
        // TODO: else add a debug warning that input is not valid
    }

    // TODO: make this more elegant
    void MakeSelection()
    {
        if (!inCombat)
        {
            if (!selectedEnemy)
            {
                selectedEnemy = true;
            }
            else
            {
                // select a spell to steal
                spellToSteal = enemyFolder.transform.GetChild(targetIndex).gameObject.GetComponent<Spellbook>().spellList[selectionIndex];

                // TODO: if no available slots, must select a spell to replace (do later)
                if (playerSpellbook.GetEmptySlots() > 0)
                {
                    playerSpellbook.StealSpell(enemyFolder.transform.GetChild(targetIndex).gameObject.GetComponent<Spellbook>().spellList[selectionIndex]);
                    selectedEnemy = false;
                    // call stage manager
                    GameManager.instance.StageManager();
                }
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

    IEnumerator HandlePlayerAction()
    {
        onCooldown = true;

        Debug.Log("Player casts " + spellToCast.name + "!");
        yield return StartCoroutine(spellToCast.CastSpell(gameObject, enemyFolder.transform.GetChild(targetIndex).gameObject));

        float initialTime = Time.time;
        while (Time.time - initialTime < cooldownBetweenActions)
        {
            yield return null;
        }

        playerSpellbook.UpdateCastThisTurn();
        // TODO: make sure this is ok
        selectedSpell = false;
        onCooldown = false;
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
        if (spellToSteal == null || !selectedEnemy)
        {
            return enemyFolder.transform.GetChild(targetIndex).gameObject.GetComponent<Spellbook>().spellList[selectionIndex];
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
}
