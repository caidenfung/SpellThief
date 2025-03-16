using UnityEngine;
using UnityEngine.UI;

public class ManaUI : MonoBehaviour
{
    public int val;
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<Spellbook>().GetRemainingCasts() < val || !GameManager.instance.IsPlayerTurn() || !player.GetComponent<PlayerInput>().CheckInCombat())
        {
            gameObject.GetComponent<Image>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<Image>().enabled = true;
        }
    }
}
