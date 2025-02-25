using System.Collections;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private int numFlickers = 3;
    private float intervalTime = 0.1f;
    private SpriteRenderer characterSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
