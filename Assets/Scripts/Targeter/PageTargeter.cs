using UnityEngine;

public class PageTargeter : MonoBehaviour
{
    public float offsetMagnitude = 2f;

    Subscription<CombatStateChanged> combat_state_subscription;
    bool combatState = true;
    Subscription<CurrentSpellIndex> spell_index_subscription;
    int spellIndex = 0;
    Subscription<ReplacingSpell> replace_spell_subscription;
    bool replaceSpell = false;

    Vector3 disappearPosition = new Vector2(-500f, -500f);
    GameObject parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        combat_state_subscription = EventBus.Subscribe<CombatStateChanged>(_OnPostCombatSpellSelection);
        spell_index_subscription = EventBus.Subscribe<CurrentSpellIndex>(_OnSpellIndexChange);
        replace_spell_subscription = EventBus.Subscribe<ReplacingSpell>(_OnSpellReplacement);

        parent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpellPageTargeter();
    }

    void UpdateSpellPageTargeter()
    {
        // TODO: Update so it disappears when player is not selecting a spell in combat or selecting a spell to remove
            // Can make it disappear on enemy turns too potentially
        if (parent != null && (combatState || replaceSpell))
        {
            GameObject page = parent.transform.GetChild(spellIndex).gameObject;

            gameObject.transform.position = page.transform.position;
        }
        else
        {
            gameObject.transform.position = disappearPosition;
        }

        // Should be fine with the disappearPosition as well because it should be out of sight regardless
        gameObject.transform.position += Vector3.up * offsetMagnitude;
    }

    void _OnPostCombatSpellSelection(CombatStateChanged combat_state_subscription)
    {
        combatState = combat_state_subscription.inCombat;
    }

    void _OnSpellIndexChange(CurrentSpellIndex spell_index_subscription)
    {
        spellIndex = spell_index_subscription.spellIndex;
    }

    void _OnSpellReplacement(ReplacingSpell replace_spell_subscription)
    {
        replaceSpell = replace_spell_subscription.replaceSpell;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(combat_state_subscription);
        EventBus.Unsubscribe(spell_index_subscription);
        EventBus.Unsubscribe(replace_spell_subscription);
    }
}
