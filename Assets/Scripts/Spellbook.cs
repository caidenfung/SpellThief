using UnityEngine;
using System.Collections.Generic;

public class Spellbook : MonoBehaviour
{
    public List<Spell> spellList;
    public int castsPerTurn = 1;
    private int castsThisTurn = 0;

    private int emptySpellSlots = 0;

    // should be able to send out cast requests to spells
    // and receive add/remove spell requests to book

    // probably should manage spells that run out of uses here

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Spell spell in spellList)
        {
            if (spell == null)
            {
                emptySpellSlots++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
    
    public int GetRemainingCasts()
    {
        return castsPerTurn - castsThisTurn;
    }

    public void ResetCastsThisTurn()
    {
        castsThisTurn = 0;
    }

    public void StealSpell(Spell spellToSteal)
    {
        bool matched = false;

        foreach (Spell spell in spellList)
        {
            if (spell != null && spell.name == spellToSteal.name)
            {
                spell.CombineCasts(spellToSteal.GetRemainingCasts());
                matched = true;
                break;
            }
        }

        if (!matched)
        {
            spellList[spellList.Count - emptySpellSlots] = spellToSteal;
            emptySpellSlots--;
        }
    }

    public void IncrementCastsThisTurn(int casts)
    {
        castsThisTurn += casts;
    }

    public int GetEmptySlots()
    {
        return emptySpellSlots;
    }
}
