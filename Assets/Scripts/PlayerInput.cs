using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    private GameObject enemyFolder;

    private int spellIndex = 0;
    private int targetIndex = 0;

    private Spellbook playerSpellbook;

    private Spell spellToCast;
    private Spell spellToSteal;

    // In combat: We select a spell, then select an enemy
    private bool selectedSpell = false;
    // Out of combat: We select an enemy, then select a spell
    private bool selectedEnemy = false;

    private bool replaceSpell = false;

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

        replaceSpell = false;

        // if a spell expires we need to make sure the spell is pointed to a valid index afterwards
        spellIndex = 0;
        if (inCombat)
        {
            spellIndex = -1;
            ChangeSelection(1, "spell");
        }
        targetIndex = 0;

        // heal player for next round
        gameObject.GetComponent<HasHealth>().UpdateHealth(20);

        // publish eventBus to represent changed Combat state and changed spellIndex/targetIndex
        EventBus.Publish<SelectedSpell>(new SelectedSpell(selectedSpell));
        EventBus.Publish<SelectedEnemy>(new SelectedEnemy(selectedEnemy));

        EventBus.Publish<CurrentSpellIndex>(new CurrentSpellIndex(spellIndex));
        EventBus.Publish<CurrentEnemyIndex>(new CurrentEnemyIndex(targetIndex));
        EventBus.Publish<ReplacingSpell>(new ReplacingSpell(replaceSpell));
        EventBus.Publish<CombatStateChanged>(new CombatStateChanged(inCombat));
    }

    public void Pause(bool setting)
    {
        paused = setting;
    }

    // TODO: Separate ChangeSelection into changeSpell and changeTarget
    // Stages of selection in combat: Choosing a spell, then choosing target
    // Stages of selection after combat: Choosing a spell/campfire option
    // Other than those 2 things as well as esc/game over menu, player input should be disabled
    void HandlePlayerInput()
    {
        // Determine whether A/D or left/right should change the spellIndex or targetIndex
        string inputIndex;
        if ((!selectedSpell && inCombat) || (selectedEnemy && !inCombat) || replaceSpell)
        {
            inputIndex = "spell";
        }
        else
        {
            inputIndex = "target";
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeSelection(-1, inputIndex);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeSelection(1, inputIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            MakeSelection();
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (replaceSpell)
            {
                replaceSpell = false;
                EventBus.Publish<ReplacingSpell>(new ReplacingSpell(replaceSpell));
                playerSpellbook.spellList.RemoveAt(playerSpellbook.spellList.Count - 1);
            }
            else if (selectedEnemy)
            {
                selectedEnemy = false;
                EventBus.Publish<SelectedEnemy>(new SelectedEnemy(selectedEnemy));
            }
            else
            {   
                selectedSpell = false;
                EventBus.Publish<SelectedSpell>(new SelectedSpell(selectedSpell));
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

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
            if (!inCombat && !replaceSpell)
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

            EventBus.Publish<CurrentSpellIndex>(new CurrentSpellIndex(spellIndex));
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

            EventBus.Publish<CurrentEnemyIndex>(new CurrentEnemyIndex(targetIndex));
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
                EventBus.Publish<SelectedEnemy>(new SelectedEnemy(selectedEnemy));
                Debug.Log("Selected Enemy: " + selectedEnemy);
            }
            else
            {
                // TODO: lots of code duplication here
                if (!replaceSpell)
                {
                    // select a spell to steal
                    spellToSteal = enemyFolder.transform.GetChild(targetIndex).gameObject.GetComponent<Spellbook>().spellList[spellIndex];

                    // see if we already have the spell first
                    if (playerSpellbook.CheckForMatch(spellToSteal))
                    {
                        selectedEnemy = false;
                        EventBus.Publish<SelectedEnemy>(new SelectedEnemy(selectedEnemy));
                        // call stage manager
                        GameManager.instance.StageManager();
                    }
                    // if no available slots, must select a spell to replace
                    else if (playerSpellbook.GetEmptySlots() == 0)
                    {
                        playerSpellbook.spellList.Add(spellToSteal);
                        replaceSpell = true;
                        EventBus.Publish<ReplacingSpell>(new ReplacingSpell(replaceSpell));
                    }
                    else
                    {
                        playerSpellbook.StealSpell(spellToSteal);
                        selectedEnemy = false;
                        EventBus.Publish<SelectedEnemy>(new SelectedEnemy(selectedEnemy));
                        // call stage manager
                        GameManager.instance.StageManager();
                    }
                }
                else
                {
                    playerSpellbook.spellList[spellIndex] = playerSpellbook.spellList[playerSpellbook.spellList.Count - 1];
                    playerSpellbook.spellList.RemoveAt(playerSpellbook.spellList.Count - 1);
                    selectedEnemy = false;
                    EventBus.Publish<SelectedEnemy>(new SelectedEnemy(selectedEnemy));
                    // call stage manager
                    GameManager.instance.StageManager();
                }
            }
        }
        else if (!selectedSpell)
        {
            spellToCast = playerSpellbook.spellList[spellIndex];
            selectedSpell = true;
            EventBus.Publish<SelectedSpell>(new SelectedSpell(selectedSpell));

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
        selectedSpell = false;
        EventBus.Publish<SelectedSpell>(new SelectedSpell(selectedSpell));

        onCooldown = false;
    }

    public string GetSpell()
    {
        if (playerSpellbook.spellList[spellIndex] == null)
        {
            return "None";
        }
        return playerSpellbook.spellList[spellIndex].name;
    }

    public Spell GetSpellToCast()
    {
        return spellToCast;
    }

    public Spell GetSpellToSteal()
    {
        if (enemyFolder.transform.GetChild(targetIndex).gameObject.GetComponent<Spellbook>().spellList[spellIndex] == null)
        {
            return new Spell();
        }
        return enemyFolder.transform.GetChild(targetIndex).gameObject.GetComponent<Spellbook>().spellList[spellIndex];
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

public class CombatStateChanged
{
    public bool inCombat;
    public CombatStateChanged(bool _inCombat)
    {
        inCombat = _inCombat;
        Debug.Log("Published inCombat");
    }
}

public class ReplacingSpell
{
    public bool replaceSpell;
    public ReplacingSpell(bool _replaceSpell)
    {
        replaceSpell = _replaceSpell;
    }
}

public class SelectedSpell
{
    public bool selectedSpell;
    public SelectedSpell(bool _selectedSpell)
    {
        selectedSpell = _selectedSpell;
    }
}

public class SelectedEnemy
{
    public bool selectedEnemy;
    public SelectedEnemy(bool _selectedEnemy)
    {
        selectedEnemy = _selectedEnemy;
    }
}

public class CurrentSpellIndex
{
    public int spellIndex;
    public CurrentSpellIndex(int _spellIndex)
    {
        spellIndex = _spellIndex;
    }
}

public class CurrentEnemyIndex
{
    public int enemyIndex;
    public CurrentEnemyIndex(int _enemyIndex)
    {
        enemyIndex = _enemyIndex;
    }
}