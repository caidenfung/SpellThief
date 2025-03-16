using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MoveToCenter : MonoBehaviour
{
    public string ownerParty;
    public Vector3 centerPosition;

    private Vector3 originalPosition;
    private float animationTime = 1.0f;

    Subscription<PanelEvent> panel_subscription;
    Subscription<GameEnded> game_over_subscription;
    bool gameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        centerPosition = new Vector3(Screen.width / 2, (Screen.height / 2), 0) + centerPosition;

        panel_subscription = EventBus.Subscribe<PanelEvent>(_OnPanelActivation);
        game_over_subscription = EventBus.Subscribe<GameEnded>(_OnGameOver);

        originalPosition = transform.position;

        if (ownerParty == "player")
        {
            TeleportPanel();
        }
        else if (ownerParty == "spacebar")
        {
            StartCoroutine(MoveTo());
        }
    }

    void Update()
    {
        if (gameOver && ownerParty != "spacebar")
        {
            TeleportToOriginal();
        }
    }

    void TeleportPanel()
    {
        if (transform.position != centerPosition) 
        {
            transform.position = centerPosition;
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    void TeleportToCenter()
    {
        transform.position = centerPosition;
    }

    void TeleportToOriginal()
    {
        transform.position = originalPosition;
    }

    public IEnumerator MoveTo()
    {
        yield return Move(centerPosition);
    }

    public IEnumerator MoveBack()
    {
        yield return Move(originalPosition);
    }

    public IEnumerator MoveToAndBack(float waitTime)
    {
        yield return MoveTo();

        yield return new WaitForSeconds(waitTime);

        yield return MoveBack();
    }

    IEnumerator Move(Vector3 dest)
    {
        Vector3 initial_loc = transform.position;
        float initial_time = Time.time;
        float progress = (Time.time - initial_time) / animationTime;

        while (progress < 1.0f)
        {
            progress = (Time.time - initial_time) / animationTime;
            transform.position = Vector3.LerpUnclamped(initial_loc, dest, progress);

            yield return null;
        }

        transform.position = dest;
    }

    void _OnPanelActivation(PanelEvent panel_subscription)
    {
        if (panel_subscription.senderParty == ownerParty)
        {
            if (!panel_subscription.initial)
            {
                if (panel_subscription.moveToCenter)
                {
                    StartCoroutine(MoveTo());
                }
                else
                {
                    StartCoroutine(MoveBack());
                }
            }
            else
            {
                if (panel_subscription.moveToCenter)
                {
                    TeleportToCenter();
                }
                else
                {
                    TeleportToOriginal();
                }
            }
        }
    }

    void _OnGameOver(GameEnded game_over_subscription)
    {
        gameOver = true;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(panel_subscription);
        EventBus.Unsubscribe(game_over_subscription);
    }
}

