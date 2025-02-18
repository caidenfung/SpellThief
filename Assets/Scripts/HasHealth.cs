using UnityEngine;

public class HasHealth : MonoBehaviour
{
    public int max_hp = 15;
    private int current_hp;

    private int current_protection;

    private bool alive;

    void Start()
    {
        alive = true;
        current_hp = max_hp;
        current_protection = 0;
    }

    public void UpdateHealth(int val)
    {
        if (current_hp + val > max_hp)
        {
            current_hp = max_hp;
        }
        else
        {
            current_hp += val;
        }

        if (current_hp <= 0)
        { 
            alive = false;
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
