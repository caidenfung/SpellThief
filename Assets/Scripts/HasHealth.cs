using UnityEngine;

public class HasHealth : MonoBehaviour
{
    public int max_hp = 15;
    private int current_hp;

    private int current_protection;

    private bool alive;

    private CharacterAnimator characterAnimator;

    void Start()
    {
        alive = true;
        current_hp = max_hp;
        current_protection = 0;
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    public void UpdateHealth(int val)
    {
        // If the player takes damage, start a damage animation
        if (val < 0 && GetStatus())
        {
            StartCoroutine(characterAnimator.OnDamageTaken());
        }

        if (current_hp + val > max_hp)
        {
            current_hp = max_hp;
        }
        else
        {
            current_hp += val;
        }

        if (current_hp <= 0 && alive)
        {
            current_hp = 0; // could remove this depending on possible spells we add
            alive = false;

            if (gameObject.CompareTag("Enemy"))
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 270);
                gameObject.transform.position += Vector3.down;
                GameManager.instance.EnemyDefeated();
            }
            else if (gameObject.CompareTag("Player"))
            {
                gameObject.transform.rotation = Quaternion.Euler(0, 0, 90);
                gameObject.transform.position += Vector3.down;
                StartCoroutine(GameManager.instance.GameOver());
            }
        }
    }

    // should be able to add protection, or remove it
    public void UpdateProtection(int val)
    {
        current_protection += val;
        if (current_protection < 0)
        {
            // leftover damage is dealt to the player
            UpdateHealth(current_protection);
            current_protection = 0;
        }
    }

    public bool GetStatus()
    {
        return alive;
    }

    public int GetHealth()
    {
        return current_hp;
    }

    public int GetProtection()
    {
        return current_protection;
    }
}
