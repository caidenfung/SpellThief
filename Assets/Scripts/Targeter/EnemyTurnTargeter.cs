using UnityEngine;

public class EnemyTurnTargeter : MonoBehaviour
{
    public float offsetMagnitude = 2f;
    public GameObject player;

    Subscription<CurrentTurn> current_turn_subscription;
    bool isPlayerTurn = true;
    Subscription<EnemyTurnIndex> enemy_turn_index_subscription;
    int activeEnemyIndex = -1;
    Subscription<CharacterActiveSpell> enemy_spell_subscription;
    Spell enemySpell;

    Vector3 disappearPosition = new Vector2(50f, 50f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        current_turn_subscription = EventBus.Subscribe<CurrentTurn>(_OnTurnEnd);
        enemy_turn_index_subscription = EventBus.Subscribe<EnemyTurnIndex>(_OnEnemyCastEnd);
        enemy_spell_subscription = EventBus.Subscribe<CharacterActiveSpell>(_OnEnemySpellSelect);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemyTurnTargeter();
    }

    void UpdateEnemyTurnTargeter()
    {
        // activeEnemyIndex = -1 as a placeholder when we haven't started iterating through enemy turns yet
        if (isPlayerTurn || activeEnemyIndex < 0)
        {
            gameObject.transform.position = disappearPosition;
        }
        else
        {
            GameObject enemies = GameManager.instance.GetEnemyFolder();

            if (enemySpell.targetType == "Self")
            {
                gameObject.transform.position = enemies.transform.GetChild(activeEnemyIndex).position;
            }
            else
            {
                gameObject.transform.position = player.transform.position;
            }
        }

        // Should be fine with the disappearPosition as well because it should be out of sight regardless
        gameObject.transform.position += Vector3.up * offsetMagnitude;
    }

    void _OnTurnEnd(CurrentTurn current_turn_subscription)
    {
        isPlayerTurn = current_turn_subscription.isPlayerTurn;
    }

    void _OnEnemyCastEnd(EnemyTurnIndex enemy_turn_index_subscription)
    {
        activeEnemyIndex = enemy_turn_index_subscription.enemyIndex;
    }

    void _OnEnemySpellSelect(CharacterActiveSpell enemy_spell_subscription)
    {
        enemySpell = enemy_spell_subscription.activeSpell;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(current_turn_subscription);
        EventBus.Unsubscribe(enemy_turn_index_subscription);
        EventBus.Unsubscribe(enemy_spell_subscription);
    }
}
