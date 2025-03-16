using UnityEngine;
using System.Collections.Generic;

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
        for (int i = 0; i < spellList.Count; i++)
        {
            if (spellList[i] == null)
            {
                emptySpellSlots++;
            }
            else
            {
                Spell newSpell = Instantiate(spellList[i]);
                newSpell.name = newSpell.spellName;
                spellList[i] = newSpell;
                spellList[i].SetSpellbook(this);
            }
        }

        // TODO: If the player starts without any spells, raise a warning
        if (GetEmptySlots() == spellList.Count && gameObject.CompareTag("Player"))
        {
            if (gameObject.GetComponent<PlayerInput>().CheckInCombat())
            {
                StartCoroutine(GameManager.instance.GameOver("You have no spells left..."));
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
        for (int i = 0; i < spellList.Count; i++)
        {
            if (spellList[i] == null)
            {
                spellList[i] = spellToSteal;
                spellList[i].SetSpellbook(this);
                emptySpellSlots--;
                break;
            }
        }
    }

    public bool CheckForMatch(Spell spellToSteal)
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

        return matched;
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

        // If we're out of spells and we didn't win the fight, the game is over
        // Otherwise, correct the spell index to the next available spell
        if (gameObject.CompareTag("Player"))
        {
            if (GetEmptySlots() == spellList.Count)
            {
                if (gameObject.GetComponent<PlayerInput>().CheckInCombat())
                {
                    StartCoroutine(GameManager.instance.GameOver("You have no spells left..."));
                }
            }
            else
            {
                gameObject.GetComponent<PlayerInput>().ChangeSelection(1, "spell");
            }
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
