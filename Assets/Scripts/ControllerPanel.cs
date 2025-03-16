using UnityEngine;

public class ControllerPanel : MonoBehaviour
{
    // listen for playerTurn, stageNum, and selectedSpell/selectedEnemy (only 1 should be active for backspace)
    Subscription<CurrentTurn> player_turn_subscription;
    bool isPlayerTurn = true;
    Subscription<StageNumber> stage_num_subscription;
    int stageNum = 0;
    Subscription<SelectedSpell> selected_spell_subscription;
    bool selectSpell = false;
    Subscription<SelectedEnemy> selected_enemy_subscription;
    bool selectEnemy = false;
    Subscription<CombatStateChanged> combat_state_subscription;
    bool combatState = true;

    bool spacebarInitial = false;
    bool arrowInitial = false;
    bool backspaceInitial = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_turn_subscription = EventBus.Subscribe<CurrentTurn>(_OnPlayerTurn);
        stage_num_subscription = EventBus.Subscribe<StageNumber>(_OnStageBegin);
        selected_spell_subscription = EventBus.Subscribe<SelectedSpell>(_OnSpellSelect);
        selected_enemy_subscription = EventBus.Subscribe<SelectedEnemy>(_OnEnemySelect);
        combat_state_subscription = EventBus.Subscribe<CombatStateChanged>(_OnPostCombatSpellSelection);
    }

    void PanelMovement()
    {
        if (stageNum >= 1 && (isPlayerTurn || !combatState) && (selectEnemy ^ selectSpell))
        {
            EventBus.Publish<PanelEvent>(new PanelEvent(true, "backspace", backspaceInitial));
            backspaceInitial = true;
        }
        else
        {
            EventBus.Publish<PanelEvent>(new PanelEvent(false, "backspace", backspaceInitial));
        }

        if (stageNum >= 1 && (isPlayerTurn || !combatState))
        {
            EventBus.Publish<PanelEvent>(new PanelEvent(true, "arrow", arrowInitial));
            arrowInitial = true;
        }
        else
        {
            EventBus.Publish<PanelEvent>(new PanelEvent(false, "arrow", arrowInitial));
        }

        if (isPlayerTurn || !combatState)
        {
            EventBus.Publish<PanelEvent>(new PanelEvent(true, "spacebar", spacebarInitial));
            spacebarInitial = true;
        }
        else
        {
            EventBus.Publish<PanelEvent>(new PanelEvent(false, "spacebar", spacebarInitial));
        }
    }

    void _OnPlayerTurn(CurrentTurn player_turn_subscription)
    {
        isPlayerTurn = player_turn_subscription.isPlayerTurn;
        PanelMovement();
    }

    void _OnStageBegin(StageNumber stage_num_subscription)
    {
        stageNum = stage_num_subscription.stageNum;
        PanelMovement();
    }

    void _OnSpellSelect(SelectedSpell selected_spell_subscription)
    {
        selectSpell = selected_spell_subscription.selectedSpell;
        Debug.Log("Spell Selected " + selectSpell);
        PanelMovement();
    }

    void _OnEnemySelect(SelectedEnemy selected_enemy_subscription)
    {
        selectEnemy = selected_enemy_subscription.selectedEnemy;
        Debug.Log("Enemy Selected " + selectEnemy);
        PanelMovement();
    }

    void _OnPostCombatSpellSelection(CombatStateChanged combat_state_subscription)
    {
        combatState = combat_state_subscription.inCombat;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(player_turn_subscription);
        EventBus.Unsubscribe(stage_num_subscription);
        EventBus.Unsubscribe(selected_spell_subscription);
        EventBus.Unsubscribe(selected_enemy_subscription);
        EventBus.Unsubscribe(combat_state_subscription);
    }
}
