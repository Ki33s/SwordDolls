
using UnityEngine;

public class Tree : MonoBehaviour
{
    public float minScale = 1f;
    public float maxScale = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        float newScale = Random.Range(minScale, maxScale);
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}
