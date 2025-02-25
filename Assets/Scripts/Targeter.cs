using UnityEngine;

public class Targeter : MonoBehaviour
{
    public PlayerInput player;
    public float offsetMagnitude = 2f;
    public int targeterID;

    Vector3 disappearPosition = new Vector2(50f, 50f);

    // Update is called once per frame
    void Update()
    {
        // TODO: Change this to until the player has casted the spell
        if (player.GetSelectedSpell())
        {
            if (player.GetSpellToCast().targetType == "Self")
            {
                gameObject.transform.position = player.gameObject.transform.position;
            }
            else if (player.GetSpellToCast().targetType == "All Enemies")
            {
                // For simplicity I created 3 targeters, one for each enemy position, and assign them to each enemy accordingly
                GameObject enemyFolder = GameManager.instance.GetEnemyFolder();
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

            gameObject.transform.position += Vector3.up * offsetMagnitude;
        }
        else
        {
            gameObject.transform.position = disappearPosition;
        }
    }
}
