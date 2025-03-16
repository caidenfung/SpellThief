using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpellDisplayer : MonoBehaviour
{
    public PlayerInput playerInput;
    public string displayType;

    private TextMeshProUGUI displayText;
    private Image displaySprite;

    Subscription<CharacterActiveSpell> enemy_spell_subscription;
    Spell enemySpell;
    Subscription<CombatStateChanged> combat_state_subscription;
    bool combatState = true;
    Subscription<SelectedEnemy> selected_enemy_subscription;
    bool selectedEnemy = false;
    Subscription<ReplacingSpell> replace_spell_subscription;
    bool replaceSpell = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy_spell_subscription = EventBus.Subscribe<CharacterActiveSpell>(_OnEnemySpellSelect);
        combat_state_subscription = EventBus.Subscribe<CombatStateChanged>(_OnPostCombatSpellSelection);
        selected_enemy_subscription = EventBus.Subscribe<SelectedEnemy>(_OnEnemySelection);
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
        DisplayEnemySpell();
    }

    void DisplayEnemySpell()
    {
        if (!combatState)
        {
            enemySpell = playerInput.GetSpellToSteal();
        }

        // safeguard for before we've defeated an enemy on the 1st round
        if (enemySpell == null)
        {
            return;
        }

        if (displayType == "sprite")
        {
            displaySprite.sprite = enemySpell.gameObject.GetComponent<SpriteRenderer>().sprite;
        }
        else if (displayType == "name")
        {
            displayText.text = enemySpell.name;
        }
        else if (displayType == "description")
        {
            displayText.text = enemySpell.targetType.ToString() + "\n" + enemySpell.description.ToString();
        }
        else if (displayType == "remainingCasts")
        {
            displayText.text = "Remaining uses: " + enemySpell.GetRemainingCasts().ToString();
        }
    }

    void PanelMovement()
    {
        if (!combatState && selectedEnemy && !replaceSpell)
        {
            EventBus.Publish<PanelEvent>(new PanelEvent(true, "enemy"));
        }
        else
        {
            EventBus.Publish<PanelEvent>(new PanelEvent(false, "enemy"));
        }
    }

    void _OnEnemySpellSelect(CharacterActiveSpell enemy_spell_subscription)
    {
        enemySpell = enemy_spell_subscription.activeSpell;
    }

    void _OnPostCombatSpellSelection(CombatStateChanged combat_state_subscription)
    {
        combatState = combat_state_subscription.inCombat;
        PanelMovement();
    }

    void _OnEnemySelection(SelectedEnemy selected_enemy_subscription)
    {
        selectedEnemy = selected_enemy_subscription.selectedEnemy;
        PanelMovement();
    }

    void _OnSpellReplacement(ReplacingSpell replace_spell_subscription)
    {
        replaceSpell = replace_spell_subscription.replaceSpell;
        PanelMovement();
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(enemy_spell_subscription);
        EventBus.Unsubscribe(combat_state_subscription);
        EventBus.Unsubscribe(selected_enemy_subscription);
        EventBus.Unsubscribe(replace_spell_subscription);
    }
}
