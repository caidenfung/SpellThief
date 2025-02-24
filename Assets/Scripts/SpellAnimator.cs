using System.Collections;
using UnityEngine;

public class SpellAnimator : MonoBehaviour
{
    public IEnumerator AnimateSpell(float duration, GameObject caster, GameObject target, string animationType, string targetType) 
    {
        Vector2 startPosition = caster.transform.position;
        Vector2 destination = target.transform.position;

        Vector2 playerPosition = new Vector2(-4.5f, -3.5f);
        Vector2 enemyPosition = new Vector2(4.5f, -3.5f);

        // instantiate sprite
        GameObject spellSprite = GameObject.Instantiate(gameObject, startPosition, Quaternion.identity);
        if (targetType == "All Enemies")
        {
            if (caster.CompareTag("Player"))
            {
                spellSprite.transform.position = enemyPosition;
            }
            else
            {
                spellSprite.transform.position = playerPosition;
            }
        }

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
        else if (targetType == "Single Target")
        {
            spellSprite.transform.position = target.transform.position;
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
