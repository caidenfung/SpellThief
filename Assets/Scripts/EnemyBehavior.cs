using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    Spellbook characterSpellbook;
    public float cooldownBetweenActions = 1.0f;

    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameObject.GetComponent<Spellbook>() != null)
        {
            characterSpellbook = gameObject.GetComponent<Spellbook>();
        }
        else
        {
            Debug.Log("No Spellbook found on " + gameObject.name);
        }
    }

    public IEnumerator TakeTurn()
    {
        characterSpellbook.ResetCastsThisTurn();

        while (characterSpellbook.GetRemainingCasts() > 0 && player.GetComponent<HasHealth>().GetStatus())
        {
            float initialTime = Time.time;
            while (Time.time - initialTime < cooldownBetweenActions)
            {
                yield return null;
            }

            // pick a random spell from the spellbook and cast it
            int spellIndex = Random.Range(0, characterSpellbook.spellList.Count);
            Debug.Log(gameObject.name + " casts " + characterSpellbook.spellList[spellIndex].name + "!");
            yield return characterSpellbook.spellList[spellIndex].CastSpell(gameObject, player);

            characterSpellbook.UpdateCastThisTurn();
        }
    }
}
