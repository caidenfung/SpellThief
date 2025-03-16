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
        if (instance != this && instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Set Scene Resolution
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
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

        // Set up combat
        EventBus.Publish<CurrentTurn>(new CurrentTurn(playerTurn));
        StageManager();
    }

    // Creates new enemies in the stage, destroys the previous level of enemies, then reactivates combat
    public void StageManager()
    {
        if (playerStage != 0)
        {
            foreach (Transform child in enemyFolder.transform)
            {
                Destroy(child.gameObject);
            }
        }

        SetEnemiesForStage();

        playerInput.ToggleInCombat();

        EventBus.Publish<StageNumber>(new StageNumber(playerStage));
    }

    void SetEnemiesForStage()
    {
        instance.enemyFolder = enemyList.transform.GetChild(playerStage).gameObject;

        Debug.Log("Create index " + (playerStage));
        instance.enemiesToClear = enemyFolder.transform.childCount;
        Debug.Log("Current stage: " + instance.enemyFolder.name);

        Vector3 spawnLocation = new Vector2(5f, -3f);
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
    }

    // if not player turn, playerInput.enabled = false and HandleEnemyTurns
    // otherwise, playerInput.enabled = true
    public static void UpdateTurnOrder()
    {
        playerTurn = !playerTurn;
        EventBus.Publish<CurrentTurn>(new CurrentTurn(playerTurn));
        EventBus.Publish<EnemyTurnIndex>(new EnemyTurnIndex(-1));

        if (!playerTurn)
        {
            playerInput.enabled = false;
            instance.StartCoroutine(HandleEnemyTurns());
        }
        else
        {
            playerInput.enabled = true;
            Debug.Log("It is now the Player's turn!");

            // reset player protection
            playerInput.gameObject.GetComponent<HasHealth>().SetProtection(0);
        }
    }

    static IEnumerator HandleEnemyTurns()
    {
        // for enemy in stage, call taketurn() from their enemybehavior script
        for (int i = 0; i < instance.enemyFolder.transform.childCount; i++)
        {
            EventBus.Publish<EnemyTurnIndex>(new EnemyTurnIndex(i));

            Transform enemy = instance.enemyFolder.transform.GetChild(i);
            // TODO: do something if enemy is out of spells to cast
            if (enemy.GetComponent<HasHealth>().GetStatus() && enemy.GetComponent<Spellbook>().GetEmptySlots() < enemy.GetComponent<Spellbook>().spellList.Count)
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
        // make sure at least 1 enemy is alive or we will crash
        else
        {
            playerInput.ChangeSelection(1, "target");
        }
    }

    // TODO: make gamemanager call gameover
    public IEnumerator GameOver(string message)
    {
        Debug.Log("Game lost");

        playerInput.Pause(true);
        EventBus.Publish<GameEnded>(new GameEnded(message));

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            yield return null;
        }
    }

    IEnumerator Victory()
    {
        Debug.Log("Game won");
        playerWon = true;

        playerInput.Pause(true);
        EventBus.Publish<GameEnded>(new GameEnded("Congratulations! You beat the game!"));

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            yield return null;
        }
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

public class CurrentTurn
{
    public bool isPlayerTurn;
    public CurrentTurn(bool _isPlayerTurn)
    {
        isPlayerTurn = _isPlayerTurn;
    }
}

public class EnemyTurnIndex
{
    public int enemyIndex;
    public EnemyTurnIndex(int _enemyIndex)
    {
        enemyIndex = _enemyIndex;
    }
}

public class StageNumber
{
    public int stageNum;
    public StageNumber(int _stageNum)
    {
        stageNum = _stageNum;
    }
}

public class GameEnded
{
    public string message;
    public GameEnded(string _message)
    {
        message = _message;
    }
}
