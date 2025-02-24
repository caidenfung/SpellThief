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

        // animate spell
        yield return StartCoroutine(animator.AnimateSpell(animateDuration, caster, target, animateType, targetType));

        // targets receive spell effects and should do a taken damage animation
        if (targetType == "All Enemies" && caster.CompareTag("Player"))
        {
            for (int i = 0; i < enemyFolder.transform.childCount; i++)
            {
                SpellEffect(enemyFolder.transform.GetChild(i).gameObject);
            }
        }
        else if (targetType == "Self")
        {
            SpellEffect(caster);
        }
        else
        {
            SpellEffect(target);
        }

        UpdateRemainingCasts(-1);
    }

    void SpellEffect(GameObject target)
    {
        if (effectType == "Damage")
        {
            target.GetComponent<HasHealth>().UpdateProtection(-effectValue);
        }
        if (effectType == "Protection")
        {
            target.GetComponent<HasHealth>().UpdateProtection(effectValue);
        }
        if (effectType == "Debuff Cast")
        {
            target.GetComponent<Spellbook>().IncrementCastsThisTurn(effectValue);
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
