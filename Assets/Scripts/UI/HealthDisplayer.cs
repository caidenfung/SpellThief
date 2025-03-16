using UnityEngine;
using TMPro;

public class HealthDisplayer : MonoBehaviour
{
    public GameObject textPrefab;
    public float offsetMagnitude = 2f;

    private GameObject text;
    private TextMeshPro textContent;
    private HasHealth health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = Instantiate(textPrefab, gameObject.transform.position + (Vector3.up * offsetMagnitude), Quaternion.identity);

        text.transform.SetParent(transform);

        textContent = text.GetComponent<TextMeshPro>();

        health = gameObject.GetComponent<HasHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health.GetStatus())
        {
            textContent.text = "Health: " + health.GetHealth().ToString() + "\nProtection: " + health.GetProtection().ToString();
        }
        else
        {
            textContent.text = "";
        }
    }
}
