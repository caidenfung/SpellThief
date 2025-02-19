using System.Collections;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public int maxCasts;
    private int remainingCasts;

    public string targetType;
    public string effectType;
    public int effectValue;
    public string animateType;
    public float animateDuration;
    public string description;
    //public int turnsActive;

    private GameObject enemyFolder;

    private SpellAnimator animator;

    void Start()
    {
        remainingCasts = maxCasts;
        animator = GetComponent<SpellAnimator>();
    }

    public IEnumerator CastSpell(GameObject caster, GameObject target)
    {
        enemyFolder = GameManager.instance.GetEnemyFolder();
        animator = GetComponent<SpellAnimator>();
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

        remainingCasts--;
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

    public void CombineCasts(int numCasts)
    {
        remainingCasts += numCasts;
    }

    public int GetRemainingCasts()
    {
        return remainingCasts;
    }
}
