using UnityEngine;

public class SinWaveMovement : MonoBehaviour
{
    public bool moveVertical;
    public bool moveHorizontal;
    public float magnitude = 1;
    public float speed = 1;

    void Update()
    {
        Vector3 newPosition = Vector3.zero;

        if (moveVertical)
        {
            newPosition.y = Mathf.Sin(Time.time * speed) * magnitude;
        }
        if (moveHorizontal)
        {
            newPosition.x = Mathf.Sin(Time.time * speed) * magnitude;
        }

        transform.position += newPosition;
    }
}