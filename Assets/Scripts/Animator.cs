using System.Collections;
using UnityEngine;

public class Animator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator AnimateSpell(float duration, GameObject caster, GameObject target, string animationType) 
    {
        Vector2 startPosition = caster.transform.position;
        Vector2 destination = target.transform.position;

        // instantiate sprite
        GameObject spellSprite = GameObject.Instantiate(gameObject, startPosition, Quaternion.identity);

        if (animationType == "Projectile")
        {
            float distance = Vector2.Distance(startPosition, destination);
            float speed = duration / distance;

            while (Vector3.Distance(spellSprite.transform.position, destination) > 0.01f)
            {
                spellSprite.transform.position = Vector2.MoveTowards(spellSprite.transform.position, destination, speed);
                yield return null;
            }
            spellSprite.transform.position = destination;
        }
        else
        {
            float initialTime = Time.time;
            while (Time.time - initialTime < duration)
            {
                yield return null;
            }
        }
        

        Destroy(spellSprite);
    }
}
