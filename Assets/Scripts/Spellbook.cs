using UnityEngine;
using System.Collections.Generic;
using UnityEditor.VersionControl;

public class Spellbook : MonoBehaviour
{
    public List<Spell> spellList;
    public int castsPerTurn = 1;
    private int castsThisTurn = 0;

    private int emptySpellSlots = 0;

    // TODO:
    // should be able to send out cast requests to spells
    // and receive add/remove spell requests to book

    // probably should manage spells that run out of uses here

    void Start()
    {
        foreach (Spell spell in spellList)
        {
            if (spell == null)
            {
                emptySpellSlots++;
            }
            else
            {
                spell.SetSpellbook(this);
            }
        }
    }

    // TODO: Move updateturnorder around?
    // Create a update function that checks if the current person moving is out of casts
    public void UpdateCastThisTurn()
    {
        castsThisTurn++;

        if (castsThisTurn == castsPerTurn)
        {
            if (gameObject.CompareTag("Player"))
            {
                ResetCastsThisTurn();
                GameManager.UpdateTurnOrder();
            }
        }
    }

    public void StealSpell(Spell spellToSteal)
    {
        bool matched = false;

        // Check if we already have this spell, if so we add the casts together
        foreach (Spell spell in spellList)
        {
            if (spell != null && spell.name == spellToSteal.name)
            {
                spell.UpdateRemainingCasts(spellToSteal.GetRemainingCasts());
                matched = true;
                break;
            }
        }

        // Otherwise we add the spell
        // TODO: Add a menu to replace spells
        if (!matched)
        {
            spellList[spellList.Count - emptySpellSlots] = spellToSteal;
            spellList[spellList.Count - emptySpellSlots].SetSpellbook(this);
            emptySpellSlots--;
        }
    }

    public void SpellExpires(Spell expiredSpell)
    {
        for (int i = 0; i < spellList.Count; i++)
        {
            if (spellList[i] == expiredSpell)
            {
                spellList[i] = null;
                emptySpellSlots++;
            }
        }

        // TODO: is there a better way to do this
        if (gameObject.CompareTag("Player"))
        {
            gameObject.GetComponent<PlayerInput>().ChangeSelection(1);
        }
    }

    // When a Spell casts, increment number of casts this turn
    public void IncrementCastsThisTurn(int casts)
    {
        castsThisTurn += casts;
    }

    public int GetEmptySlots()
    {
        return emptySpellSlots;
    }

    public int GetRemainingCasts()
    {
        return castsPerTurn - castsThisTurn;
    }

    public void ResetCastsThisTurn()
    {
        castsThisTurn = 0;
    }
}
