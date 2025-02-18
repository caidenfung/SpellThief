using UnityEngine;
using System.Collections.Generic;

public class Spellbook : MonoBehaviour
{
    public List<Spell> spellList;
    public int castsPerTurn = 1;
    private int castsThisTurn = 0;

    // should be able to send out cast requests to spells
    // and receive add/remove spell requests to book

    // probably should manage spells that run out of uses here

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
            GameManager.UpdateTurnOrder();
        }
    }
    
    public int GetRemainingCasts()
    {
        return castsPerTurn - castsThisTurn;
    }
}
