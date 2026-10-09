
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 10f;
    public int destroyTime = 3;

    private void Start()
    {
        Destroy(gameObject, 3);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
       transform.position += transform.up *moveSpeed * Time.deltaTime;
    }
}
