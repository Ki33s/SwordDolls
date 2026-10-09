
using UnityEngine;

public class DamageScore : MonoBehaviour
{
    public float speefY = 1f;
    public float lifeTime = 3f;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + speefY * Time.deltaTime, transform.position.z);

        float colorAlpha = GetComponent<TextMesh>().color.a;

        colorAlpha -= Time.deltaTime / lifeTime;

        if (colorAlpha > 0)
        {
            GetComponent<TextMesh>().color = new Color(GetComponent<TextMesh>().color.r, GetComponent<TextMesh>().color.g, GetComponent<TextMesh>().color.b, colorAlpha);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
