using UnityEngine;
using UnityEngine.UI;

public class PageHolder : MonoBehaviour
{
    public int spellbookPage;
    public Spellbook spellbook;

    private bool visibility = true;

    private void Update()
    {
        if (spellbook != null)
        {
            if (spellbookPage >= spellbook.spellList.Count)
            {
                visibility = false;
            }
            else
            {
                visibility = true;
            }
        }
        
        ToggleVisibility(visibility);
    }

    void ToggleVisibility(bool visibility)
    {
        GetComponent<Image>().enabled = visibility;

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(visibility);
        }
    }
}
