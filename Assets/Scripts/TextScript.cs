using UnityEngine;
using TMPro;

public class TextScript : MonoBehaviour
{
    public PlayerInput player;
    public HasHealth health;
    public Spellbook spellbook;

    private TextMeshProUGUI displayText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        displayText = GetComponent<TextMeshProUGUI>();

        if (player != null)
        {
            displayText.text = "Current Spell: " + player.GetSpell();
        }
        else if (health != null)
        {
            displayText.text = "Health: " + health.GetHealth().ToString() + ", Protection: " + health.GetProtection().ToString();
        }
        else if (spellbook != null)
        {
            displayText.text = "Spells to cast this turn: " + spellbook.GetRemainingCasts().ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            displayText.text = "Current Spell: " + player.GetSpell();
        }
        else if (health != null)
        {
            displayText.text = "Health: " + health.GetHealth().ToString() + ", Protection: " + health.GetProtection().ToString();
        }
        else if (spellbook != null)
        {
            displayText.text = "Spells to cast this turn: " + spellbook.GetRemainingCasts().ToString();
        }
    }
}
