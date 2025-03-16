using System.Collections;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public Sprite turnSprite;

    private int numFlickers = 3;
    private float intervalTime = 0.1f;
    private SpriteRenderer characterSprite;
    private Sprite defaultSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterSprite = GetComponent<SpriteRenderer>();
        defaultSprite = characterSprite.sprite;
    }

    public void ToggleSprite()
    {
        if (characterSprite.sprite == defaultSprite)
        {
            characterSprite.sprite = turnSprite;
        }
        else
        {
            characterSprite.sprite = defaultSprite;
        }
    }

    public IEnumerator OnDamageTaken()
    {
        for (int i = 0; i < numFlickers * 2; i++)
        {
            float startTime = Time.time;
            characterSprite.enabled = !characterSprite.enabled;

            while (Time.time - startTime < intervalTime)
            {
                yield return null;
            }
        }

        // Sanity check to make sure sprite remains enabled
        characterSprite.enabled = true;
    }
}
