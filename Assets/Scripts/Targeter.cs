using UnityEngine;

public class Targeter : MonoBehaviour
{
    public PlayerInput player;
    public float offsetMagnitude = 2f;

    Vector3 aoePosition = new Vector2(4.5f, -4.5f);
    Vector3 disappearPosition = new Vector2(50f, 50f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetSelectedSpell())
        {
            if (player.GetSpellToCast().targetType == "All Enemies")
            {
                gameObject.transform.localScale = new Vector3(12, 3, 1);
            }
            else
            {
                gameObject.transform.localScale = new Vector3(2, 3, 1);
            }


            if (player.GetSpellToCast().targetType == "Self")
            {
                gameObject.transform.position = player.gameObject.transform.position + (Vector3.down * offsetMagnitude);
            }
            else if (player.GetSpellToCast().targetType == "All Enemies")
            {
                gameObject.transform.position = aoePosition;
            }
            else
            {
                gameObject.transform.position = player.GetTargetLocation().position + (Vector3.down * offsetMagnitude);
            }
        }
        else
        {
            gameObject.transform.position = disappearPosition;
        }
    }
}
