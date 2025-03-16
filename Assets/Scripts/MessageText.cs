using TMPro;
using UnityEngine;

public class MessageText : MonoBehaviour
{
    TextMeshProUGUI text;

    Subscription<GameEnded> game_over_subscription;
    bool inGame = true;
    Subscription<CombatStateChanged> combat_state_subscription;
    bool combatState = true;
    Subscription<ReplacingSpell> replace_spell_subscription;
    bool replaceSpell = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        game_over_subscription = EventBus.Subscribe<GameEnded>(_OnGameOver);
        combat_state_subscription = EventBus.Subscribe<CombatStateChanged>(_OnPostCombatSpellSelection);
        replace_spell_subscription = EventBus.Subscribe<ReplacingSpell>(_OnSpellReplacement);

        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inGame || !combatState || replaceSpell)
        {
            text.enabled = true;
        }
        else
        {
            text.enabled = false;
        }
    }

    void _OnGameOver(GameEnded game_over_subscription)
    {
        inGame = false;

        text.text = game_over_subscription.message + "\nPress Space to Play Again";
    }

    void _OnPostCombatSpellSelection(CombatStateChanged combat_state_subscription)
    {
        combatState = combat_state_subscription.inCombat;

        text.text = "Good work winning the fight! You may now select an enemy and take one of their spells.";
    }

    void _OnSpellReplacement(ReplacingSpell replace_spell_subscription)
    {
        replaceSpell = replace_spell_subscription.replaceSpell;

        if (replaceSpell)
        {
            text.text = "We have too many spells! Select one to discard.";
        }
        else if (!combatState)
        {
            text.text = "Good work winning the fight! You may now select an enemy and take one of their spells.";
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(game_over_subscription);
        EventBus.Unsubscribe(combat_state_subscription);
        EventBus.Unsubscribe(replace_spell_subscription);
    }
}
