using System.Collections;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public int maxCasts;
    private int remainingCasts;

    public string spellName;
    public string targetType;
    public string effectType;
    public int effectValue;
    public string animateType;
    public float animateDuration;
    public string description;
    //public int turnsActive;

    private GameObject enemyFolder;

    private SpellAnimator animator;
    private CharacterAnimator characterAnimator;

    private Spellbook spellbook;

    void Start()
    {
        remainingCasts = maxCasts;
        animator = GetComponent<SpellAnimator>();
    }

    public void SetSpellbook(Spellbook book)
    {
        spellbook = book;
    }

    public IEnumerator CastSpell(GameObject caster, GameObject target)
    {
        enemyFolder = GameManager.instance.GetEnemyFolder();

        characterAnimator = caster.GetComponent<CharacterAnimator>();
        characterAnimator.ToggleSprite();

        // animate spell
        yield return StartCoroutine(animator.AnimateSpell(animateDuration, caster, target, animateType, targetType));

        // targets receive spell effects and should do a taken damage animation
        if (targetType == "All Enemies" && caster.CompareTag("Player"))
        {
            for (int i = 0; i < enemyFolder.transform.childCount; i++)
            {
                SpellEffect(enemyFolder.transform.GetChild(i).gameObject, caster);
            }
        }
        else if (targetType == "Self")
        {
            SpellEffect(caster, caster);
        }
        else
        {
            SpellEffect(target, caster);
        }

        characterAnimator.ToggleSprite();
        UpdateRemainingCasts(-1);
    }

    void SpellEffect(GameObject target, GameObject caster)
    {
        // TODO: Could make it so spells have multiple effects (List) and go through all of them here
        if (effectType == "Damage")
        {
            if (spellName == "Mana Eater")
            {
                int trueValue = effectValue - GetRemainingCasts();
                target.GetComponent<HasHealth>().UpdateProtection(-trueValue);
            }
            else
            {
                target.GetComponent<HasHealth>().UpdateProtection(-effectValue);
            }
        }
        else if (effectType == "Protection")
        {
            target.GetComponent<HasHealth>().UpdateProtection(effectValue);
        }
        else if (effectType == "Healing")
        {
            target.GetComponent<HasHealth>().UpdateHealth(effectValue);
        }
        else if (effectType == "Debuff Mana")
        {
            target.GetComponent<Spellbook>().IncrementCastsThisTurn(effectValue);
        }
        else if (effectType == "Debuff Cast")
        {
            Spellbook targetSpellbook = target.GetComponent<Spellbook>();
            foreach (Spell spell in targetSpellbook.spellList)
            {
                if (spell != null)
                {
                    spell.UpdateRemainingCasts(-effectValue);
                }
            }
        }
        else if (effectType == "Buff Cast")
        {
            Spellbook targetSpellbook = target.GetComponent<Spellbook>();
            foreach (Spell spell in targetSpellbook.spellList)
            {
                if (spell != null && spell.spellName != spellName)
                {
                    spell.UpdateRemainingCasts(effectValue);
                }
            }
        }
    }

    public void UpdateRemainingCasts(int numCasts)
    {
        remainingCasts += numCasts;

        if (remainingCasts <= 0)
        {
            spellbook.SpellExpires(this);
        }
    }

    public int GetRemainingCasts()
    {
        return remainingCasts;
    }
}
