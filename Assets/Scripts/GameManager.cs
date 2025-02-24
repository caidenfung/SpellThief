using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static bool playerTurn = true;

    public GameObject enemyList;
    private GameObject enemyFolder;
    private int enemiesToClear;

    private static int playerStage = 0;
    private static int numStages;

    public GameObject resultText;
    private bool playerWon = false;

    public static PlayerInput playerInput { get; set; }

    public static GameManager instance { get; private set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;    
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find better way to reassign scene
        enemyList = GameObject.Find("Enemies");
        resultText = GameObject.Find("ResultText");

        playerStage = 0;
        numStages = enemyList.transform.childCount;

        // Should do this for every stage
        SetEnemiesForStage(true);

        // Set Scene Resolution?
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Creates new enemies in the stage, destroys the previous level of enemies, then reactivates combat
    public void StageManager()
    {
        SetEnemiesForStage(false);
        Destroy(enemyList.transform.GetChild(0).gameObject);
        playerInput.ToggleInCombat();
    }

    void SetEnemiesForStage(bool initial)
    {
        if (initial)
        {
            instance.enemyFolder = enemyList.transform.GetChild(0).gameObject;
        }
        else
        {
            instance.enemyFolder = enemyList.transform.GetChild(1).gameObject;
        }

        Debug.Log("Create index " + (playerStage));
        instance.enemiesToClear = enemyFolder.transform.childCount;
        Debug.Log("Current stage: " + instance.enemyFolder.name);

        Vector3 spawnLocation = new Vector2(4.5f, -2f);
        float magnitude = 2.5f;
        // TODO: this is almost all hard coded
        for (int i = 0; i < instance.enemyFolder.transform.childCount; i++)
        {
            GameObject enemy = instance.enemyFolder.transform.GetChild(i).gameObject;
            enemy.transform.position = spawnLocation;

            if (i == 0)
            {
                spawnLocation += Vector3.right * magnitude;
            }
            else if (i == 1)
            {
                spawnLocation += Vector3.left * 2 * magnitude;
            }
        }

        // TODO: We have this here but is there a better location for it?
        playerInput.gameObject.GetComponent<Spellbook>().ResetCastsThisTurn();

        // instance.enemies = enemyFolder.transform.GetComponentsInChildren<GameObject>();
    }

    // Increment turn index (1 for player, then 1 more for every other unit
    // Make this an event?
    // if not player turn, playerInput.enabled = false and HandleEnemyTurns
    // otherwise, playerInput.enabled = true
    public static void UpdateTurnOrder()
    {
        playerTurn = !playerTurn;

        if (!playerTurn)
        {
            playerInput.enabled = false;
            instance.StartCoroutine(HandleEnemyTurns());
        }
        else
        {
            playerInput.enabled = true;
            Debug.Log("It is now the Player's turn!");
        }
    }

    static IEnumerator HandleEnemyTurns()
    {
        // for enemy in stage, call taketurn() from their enemybehavior script
        for (int i = 0; i < instance.enemyFolder.transform.childCount; i++)
        {
            Transform enemy = instance.enemyFolder.transform.GetChild(i);
            if (enemy.GetComponent<HasHealth>().GetStatus())
            {
                Debug.Log(instance.enemyFolder.transform.GetChild(i).name + " is taking their turn");
                yield return instance.StartCoroutine(enemy.GetComponent<EnemyBehavior>().TakeTurn());
            }
        }

        UpdateTurnOrder();
    }

    public void EnemyDefeated()
    {
        enemiesToClear--;

        playerInput.ChangeSelection(1);

        if (enemiesToClear == 0)
        {
            playerStage++;
            if (playerStage == numStages)
            {
                StartCoroutine(Victory());
            }
            else
            {
                playerInput.ToggleInCombat();
            }
        }
    }

    // TODO: make gamemanager call gameover
    public IEnumerator GameOver()
    {
        Debug.Log("Game lost");
        instance.resultText.GetComponent<TextScript>().ToggleTextLocation();

        playerInput.Pause(true);

        float initial_time = Time.time;
        while (Time.time - initial_time < 2.0f)
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator Victory()
    {
        Debug.Log("Game won");
        playerWon = true;

        instance.resultText.GetComponent<TextScript>().ToggleTextLocation();

        playerInput.Pause(true);

        float initial_time = Time.time;
        while (Time.time - initial_time < 2.0f)
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public GameObject GetEnemyFolder()
    {
        return instance.enemyFolder;
    }

    public bool IsPlayerTurn()
    {
        return playerTurn;
    }

    public bool HasPlayerWon()
    {
        return playerWon;
    }
}
