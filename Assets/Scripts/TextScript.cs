using UnityEngine;
using TMPro;

public class TextScript : MonoBehaviour
{
    public PlayerInput player;
    public HasHealth health;
    public Spellbook spellbook;

    private TextMeshProUGUI displayText;

    private Vector3 hiddenPosition = new Vector3(1200, 1000);
    private Vector3 visiblePosition = new Vector3(1200, 500);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        displayText = GetComponent<TextMeshProUGUI>();

        if (player != null && gameObject.name == "CurrentSpell")
        {
            displayText.text = "Current Spell: " + player.GetSpell();
        }
        else if (health != null)
        {
            displayText.text = "Health: " + health.GetHealth().ToString() + ", Protection: " + health.GetProtection().ToString();
        }
        else if (spellbook != null && gameObject.name == "PlayerCastsRemaining")
        {
            displayText.text = "Spells to cast this turn: " + spellbook.GetRemainingCasts().ToString();
        }
        else if (gameObject.name == "ResultText")
        {
            displayText.text = "Game Over";
        }
        else if (gameObject.name == "CurrentTarget")
        {
            displayText.text = "Current Target: " + player.GetTarget();
        }
        else if (gameObject.name == "SpellDescriptor")
        {
            if (player.CheckInCombat())
            {
                displayText.text = player.GetSpell() + "\n" + spellbook.spellList[player.GetSpellIndex()].description;
            }
            else
            {
                displayText.text = player.GetSpellToSteal().name + "\n" + player.GetSpellToSteal().description;
            }
        }
        else if (gameObject.name == "Instructions")
        {
            if (player.CheckInCombat())
            {
                if (!player.GetSelectedSpell())
                {
                    displayText.text = "Press A or D to switch between spells.\nPress Space to select a spell to cast.";
                }
                else
                {
                    displayText.text = "Press A or D to switch between targets.\nPress Space to select a target.\nPress Backspace to deselect a spell.";
                }
            }
            else
            {
                displayText.text = "Press A or D to switch between spells.\nPress Space to select a spell to add to your Spellbook.";
            }
        }
        else
        {
            if (GameManager.instance.IsPlayerTurn())
            {
                displayText.text = "Player Turn";
            }
            else
            {
                displayText.text = "Enemy Turn";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && gameObject.name == "CurrentSpell")
        {
            displayText.text = "Current Spell: " + player.GetSpell();
        }
        else if (health != null)
        {
            displayText.text = "Health: " + health.GetHealth().ToString() + ", Protection: " + health.GetProtection().ToString();
        }
        else if (spellbook != null && gameObject.name == "PlayerCastsRemaining")
        {
            if (player.CheckInCombat())
            {
                if (GameManager.instance.IsPlayerTurn())
                {
                    displayText.text = "Spell casts remaining this turn: " + spellbook.GetRemainingCasts().ToString();
                }
                else
                {
                    displayText.text = "Enemy is casting a spell...";
                }
            }
            else
            {
                displayText.text = "Used by: " + player.GetEnemyName();
            }
            
        }
        else if (gameObject.name == "CurrentTarget")
        {
            displayText.text = "Current Target: " + player.GetTarget();
        }
        else if (gameObject.name == "ResultText")
        {
            if (GameManager.instance.HasPlayerWon())
            {
                displayText.text = "You win! Thanks for playing";
            }
        }
        else if (gameObject.name == "SpellDescriptor")
        {
            if (player.CheckInCombat())
            {
                displayText.text = player.GetSpell() + "\nTargets: " + spellbook.spellList[player.GetSpellIndex()].targetType +  "\n" + spellbook.spellList[player.GetSpellIndex()].description;
            }
            else
            {
                displayText.text = player.GetSpellToSteal().name + "\nTargets: " + player.GetSpellToSteal().targetType + "\n" + player.GetSpellToSteal().description;
            }
        }
        else if (gameObject.name == "Instructions")
        {
            if (player.CheckInCombat())
            {
                if (!player.GetSelectedSpell())
                {
                    displayText.text = "Press A or D to switch between spells.\nPress Space to select a spell to cast.";
                }
                else
                {
                    displayText.text = "Press A or D to switch between targets.\nPress Space to select a target.\nPress Backspace to deselect a spell.";
                }
            }
            else
            {
                displayText.text = "Press A or D to switch between spells.\nPress Space to select a spell to add to your Spellbook.";
            }
        }
        else
        {
            if (GameManager.instance.IsPlayerTurn())
            {
                displayText.text = "Player Turn";
            }
            else
            {
                displayText.text = "Enemy Turn";
            }
        }
    }
    
    public void ToggleTextLocation()
    {
        if (gameObject.transform.position != visiblePosition)
        {
            gameObject.transform.position = visiblePosition;
        }
        else
        {
            gameObject.transform.position = hiddenPosition;
        }
    }
}
