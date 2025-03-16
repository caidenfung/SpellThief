using UnityEngine;

public class PlayerTargeter : MonoBehaviour
{
    public PlayerInput player;
    public float offsetMagnitude = 2f;
    public int targeterID;

    Vector3 disappearPosition = new Vector2(50f, 50f);

    Subscription<CombatStateChanged> combat_state_subscription;
    bool combatState = true;
    Subscription<SelectedEnemy> selected_enemy_subscription;
    bool selectedEnemy = false;
    Subscription<CurrentEnemyIndex> enemy_index_subscription;
    int enemyIndex = 0;
    

    void Start()
    {
        combat_state_subscription = EventBus.Subscribe<CombatStateChanged>(_OnPostCombatSpellSelection);
        selected_enemy_subscription = EventBus.Subscribe<SelectedEnemy>(_OnEnemySelection);
        enemy_index_subscription = EventBus.Subscribe<CurrentEnemyIndex>(_OnEnemyIndexChange);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCombatTargeter();
    }

    void UpdateCombatTargeter()
    {
        GameObject enemyFolder = GameManager.instance.GetEnemyFolder();

        // The Targeter will appear when selecting an enemy for a spell
        if (!combatState && !selectedEnemy)
        {
            GameObject enemy = enemyFolder.transform.GetChild(enemyIndex).gameObject;
            gameObject.transform.position = enemy.transform.position;
        }
        // TODO: Change this to disappear once the player has casted the spell?
        else if (player.GetSelectedSpell())
        {
            if (player.GetSpellToCast().targetType == "Self")
            {
                gameObject.transform.position = player.gameObject.transform.position;
            }
            else if (player.GetSpellToCast().targetType == "All Enemies")
            {
                // For simplicity I created 3 targeters, one for each enemy position, and assign them to each enemy accordingly

                if (targeterID < enemyFolder.transform.childCount)
                {
                    GameObject enemy = enemyFolder.transform.GetChild(targeterID).gameObject;
                    if (enemy.GetComponent<HasHealth>().GetStatus())
                    {
                        gameObject.transform.position = enemy.transform.position;
                    }
                }
            }
            else
            {
                gameObject.transform.position = player.GetTargetLocation().position;
            }
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
        Debug.Log("Combat state updated to " + combatState);
    }

    void _OnEnemySelection(SelectedEnemy selected_enemy_subscription)
    {
        selectedEnemy = selected_enemy_subscription.selectedEnemy;
    }

    void _OnEnemyIndexChange(CurrentEnemyIndex enemy_index_subscription)
    {
        enemyIndex = enemy_index_subscription.enemyIndex;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(combat_state_subscription);
        EventBus.Unsubscribe(selected_enemy_subscription);
        EventBus.Unsubscribe(enemy_index_subscription);
    }
}
