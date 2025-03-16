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

    Subscription<CombatStateChanged> combat_state_subscription;
    bool combatState = true;
    Subscription<CurrentTurn> current_turn_subscription;
    bool isPlayerTurn = true;
    Subscription<ReplacingSpell> replace_spell_subscription;
    bool replaceSpell = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combat_state_subscription = EventBus.Subscribe<CombatStateChanged>(_OnPostCombatSpellSelection);
        current_turn_subscription = EventBus.Subscribe<CurrentTurn>(_OnTurnEnd);
        replace_spell_subscription = EventBus.Subscribe<ReplacingSpell>(_OnSpellReplacement);

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
        if (spellbook.spellList[playerInput.GetSpellIndex()] == null)
        {
            return;
        }

        Spell displaySpell = spellbook.spellList[playerInput.GetSpellIndex()];

        //if (playerInput.CheckInCombat() && spellbook.GetEmptySlots() != spellbook.spellList.Count)
        //{
        //    displaySpell = spellbook.spellList[playerInput.GetSpellIndex()];
        //}

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

    void PanelMovement()
    {
        if ((combatState && isPlayerTurn) || replaceSpell)
        {
            EventBus.Publish<PanelEvent>(new PanelEvent(true, "player"));
        }
        else
        {
            EventBus.Publish<PanelEvent>(new PanelEvent(false, "player"));
        }
    }

    void _OnPostCombatSpellSelection(CombatStateChanged combat_state_subscription)
    {
        combatState = combat_state_subscription.inCombat;
        PanelMovement();
    }

    void _OnTurnEnd(CurrentTurn current_turn_subscription)
    {
        isPlayerTurn = current_turn_subscription.isPlayerTurn;
        PanelMovement();
    }

    void _OnSpellReplacement(ReplacingSpell replace_spell_subscription)
    {
        replaceSpell = replace_spell_subscription.replaceSpell;
        PanelMovement();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(combat_state_subscription);
        EventBus.Unsubscribe(current_turn_subscription);
        EventBus.Unsubscribe(replace_spell_subscription);
    }
}

public class PanelEvent
{
    public bool moveToCenter;
    public string senderParty;
    public bool initial = false;
    public PanelEvent(bool _moveToCenter, string _senderParty)
    {
        moveToCenter = _moveToCenter;
        senderParty = _senderParty;
    }

    public PanelEvent(bool _moveToCenter, string _senderParty, bool _initial)
    {
        moveToCenter = _moveToCenter;
        senderParty = _senderParty;
        initial = _initial;
    }
}