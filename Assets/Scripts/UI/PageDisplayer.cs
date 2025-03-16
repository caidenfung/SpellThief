using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageDisplayer : MonoBehaviour
{
    public string displayType;

    private GameObject parent;
    private int page;
    private Spellbook spellbook;

    private Spell targetSpell;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        parent = gameObject.transform.parent.gameObject;
        PageHolder parentHolder = parent.GetComponent<PageHolder>();

        page = parentHolder.spellbookPage;
        spellbook = parentHolder.spellbook;

        if (page >= spellbook.spellList.Count)
        {
            return;
        }
        targetSpell = spellbook.spellList[page];
    }

    // Update is called once per frame
    void Update()
    {
        if (page >= spellbook.spellList.Count)
        {
            return;
        }
        targetSpell = spellbook.spellList[page];

        if (displayType == "remainingCasts")
        {
            if (targetSpell == null)
            {
                gameObject.GetComponent<TextMeshProUGUI>().enabled = false;
            }
            else
            {
                gameObject.GetComponent<TextMeshProUGUI>().enabled = true;
                gameObject.GetComponent<TextMeshProUGUI>().text = "Uses: " + targetSpell.GetRemainingCasts().ToString();
            }
        }
        else if (displayType == "sprite")
        {
            if (targetSpell == null)
            {
                gameObject.GetComponent<Image>().enabled = false;
            }
            else
            {
                gameObject.GetComponent<Image>().enabled = true;
                gameObject.GetComponent<Image>().sprite = targetSpell.gameObject.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }
}
