using System.Collections;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public int maxCasts;
    private int remainingCasts;

    // replace this with proper targeting
    public GameObject target;

    public string targetType;
    public string effectType;
    public int effectValue;
    public string animateType;
    public float animateDuration;
    //public int turnsActive;

    private Animator animator;

    void Start()
    {
        remainingCasts = maxCasts;
        animator = GetComponent<Animator>();
    }

    public IEnumerator CastSpell(GameObject caster)
    {
        // player input set to false, set back to true at the end

        // animate spell
        yield return StartCoroutine(animator.AnimateSpell(animateDuration, caster, target, animateType));

        // targets receive spell effects and have an animation
            // maybe have a separate coroutine for this, that we can activate on separate targets?
        if (effectType == "Damage")
        {
            target.GetComponent<HasHealth>().UpdateProtection(-effectValue);
        }
        if (effectType == "Protection")
        {
            target.GetComponent<HasHealth>().UpdateProtection(effectValue);
        }

        remainingCasts--;
    }
}
