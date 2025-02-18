using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // probably make a stage var, and also move this to gamemanager perhaps
    public GameObject enemyFolder;

    private int spellIndex = 0;
    private int targetIndex = 0;

    private Spellbook playerSpellbook;

    private Spell spellToCast;

    private bool selectedSpell = false;

    private bool inCombat = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerSpellbook = GetComponent<Spellbook>();
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerInput();
    }

    public string GetSpell()
    {
        if (spellToCast == null || !selectedSpell)
        {
            return spellIndex.ToString();
        }
        return spellToCast.name;
    }

    // Stages of selection in combat: Choosing a spell, then choosing target
    // Stages of selection after combat: Choosing a spell/campfire option
    // Other than those 2 things as well as esc/game over menu, player input should be disabled

    // rewrite this to handle input for targeting and stuff
    // also add a back space key
    void HandlePlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (spellIndex > 0)
            {
                spellIndex--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (spellIndex < playerSpellbook.spellList.Count && playerSpellbook.spellList[spellIndex + 1] != null)
            {
                spellIndex++;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!selectedSpell)
            {
                spellToCast = playerSpellbook.spellList[spellIndex];
                selectedSpell = true;
            }
            else
            {
                Debug.Log("Cast " + spellToCast.name);
                StartCoroutine(spellToCast.CastSpell(gameObject));
                selectedSpell = false;
            }
        }
    }
}
