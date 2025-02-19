using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    Spellbook characterSpellbook;
    public float cooldownBetweenActions = 1.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameObject.GetComponent<Spellbook>() != null)
        {
            characterSpellbook = gameObject.GetComponent<Spellbook>();
        }
        else {
            Debug.Log("No Spellbook found on " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TakeTurn() {
        while (GetRemainingCasts > 0) {
            // pick a random spell from the spellbook and cast it
            int spellIndex = Random.Range(0, characterSpellbook.spellList.Count);
            yield return characterSpellbook.spellList[spellIndex].CastSpell();

            float initialTime = Time.time;
            while (Time.time - initialTime < cooldownBetweenActions) {
                yield return null;
            }

            characterSpellbook.UpdateCastThisTurn();
        }
    }
}
