using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    Spellbook characterSpellbook;
    CharacterAnimator characterAnimator;
    public float cooldownBetweenActions = 3.0f;

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
        // TODO: may have to adjust so we have an idea of who the spell is meant to target
        // TODO: make it so we aren't instantly moving panel back and forth
        while (characterSpellbook.GetRemainingCasts() > 0 && player.GetComponent<HasHealth>().GetStatus())
        {
            // reset protection to 0
            gameObject.GetComponent<HasHealth>().SetProtection(0);

            // pick a random spell from the spellbook and cast it
            int spellIndex = Random.Range(0, characterSpellbook.spellList.Count);
            EventBus.Publish<CharacterActiveSpell>(new CharacterActiveSpell(spellIndex, characterSpellbook));

            // Pop out panel
            EventBus.Publish<PanelEvent>(new PanelEvent(true, "enemy"));

            float initialTime = Time.time;
            while (Time.time - initialTime < cooldownBetweenActions)
            {
                yield return null;
            }

            // cast the selected spell
            Debug.Log(gameObject.name + " casts " + characterSpellbook.spellList[spellIndex].name + "!");
            yield return characterSpellbook.spellList[spellIndex].CastSpell(gameObject, player);

            initialTime = Time.time;
            while (Time.time - initialTime < cooldownBetweenActions)
            {
                yield return null;
            }

            // Return panel
            EventBus.Publish<PanelEvent>(new PanelEvent(false, "enemy"));

            initialTime = Time.time;
            while (Time.time - initialTime < cooldownBetweenActions)
            {
                yield return null;
            }

            characterSpellbook.UpdateCastThisTurn();
        }

        characterSpellbook.ResetCastsThisTurn();
    }

    public Spell ActiveSpell(int spellbookIndex)
    {
        return characterSpellbook.spellList[spellbookIndex];
    }
}

public class CharacterActiveSpell
{
    public Spell activeSpell;
    public CharacterActiveSpell(int _spellbookIndex, Spellbook _characterSpellbook)
    {
        activeSpell = _characterSpellbook.spellList[_spellbookIndex];
    }
}