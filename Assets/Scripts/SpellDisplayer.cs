using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpellDisplayer : MonoBehaviour
{
    public Spellbook spellbook;
    public PlayerInput playerInput;
    public string displayType;

    private TextMeshProUGUI displayText;
    private Image displaySprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (displayType == "sprite")
        {
            displaySprite = GetComponent<Image>();
        }
        else
        {
            displayText = GetComponent<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Spell displaySpell;

        if (playerInput.CheckInCombat() && spellbook.GetEmptySlots() != spellbook.spellList.Count)
        {
            displaySpell = spellbook.spellList[playerInput.GetSpellIndex()];
        }
        else
        {
            displaySpell = playerInput.GetSpellToSteal();
        }

        if (displayType == "sprite")
        {
            displaySprite.sprite = displaySpell.gameObject.GetComponent<SpriteRenderer>().sprite;
        }
        else if (displayType == "name")
        {
            displayText.text = displaySpell.name;
        }
        else if (displayType == "description")
        {
            displayText.text = displaySpell.targetType.ToString() + "\n" + displaySpell.description.ToString();
        }
        else if (displayType == "remainingCasts")
        {
            displayText.text = "Remaining uses: " + displaySpell.GetRemainingCasts().ToString();
        }
    }
}
