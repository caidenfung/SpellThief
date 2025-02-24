using TMPro;
using UnityEngine;

public class SpellDisplayer : MonoBehaviour
{
    public int spellbookPage;
    public Spellbook spellbook;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spellbook.spellList[spellbookPage] == null) 
        {
            gameObject.GetComponent<TextMeshProUGUI>().text = "Spellbook Page " + spellbookPage + ": Empty";
        }
        else
        {
            gameObject.GetComponent<TextMeshProUGUI>().text = "Spellbook Page " + spellbookPage + ": " 
                + spellbook.spellList[spellbookPage].name + " " + spellbook.spellList[spellbookPage].GetRemainingCasts() 
                + "/" + spellbook.spellList[spellbookPage].maxCasts;
        }
    }
}
