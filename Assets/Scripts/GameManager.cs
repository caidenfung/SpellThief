using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private static bool playerTurn = true;

    public static PlayerInput playerInput { get; set; }

    public static GameManager instance { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Set screen resolution here?
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Increment turn index (1 for player, then 1 more for every other unit
    // Make this an event?
    // Right now this only accounts for 1 enemy at a time
    public static void UpdateTurnOrder()
    {
        if (playerTurn)
        {
            playerInput.enabled = false;
        }

        playerTurn = !playerTurn;

        if (!playerTurn)
        {
            HandleEnemyTurns();
        }
    }

    static void HandleEnemyTurns()
    {
        // for enemy in stage, call taketurn() from their enemybehavior script
    }
}
