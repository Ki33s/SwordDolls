
using UnityEngine;

public class DuplicateClone : MonoBehaviour
{
    public GameObject targetSword;
    private float deltaX;
    private float deltaZ;
    // Start is called before the first frame update
    void Start()
    {
        if(!targetSword){Destroy(gameObject);return;}
        int angle = Random.Range(0, 360);
        deltaX = 2 * Mathf.Cos(angle);
        deltaZ = 2 * Mathf.Sin(angle);

        if(!targetSword){Destroy(gameObject);return;}
        transform.position = new Vector3(targetSword.transform.position.x + deltaX, targetSword.transform.position.y, targetSword.transform.position.z + deltaZ);
        GameObject nowDuplicateCloneFx = Instantiate(GetComponent<BulletTrigger>().impactEffect, new Vector3(targetSword.transform.position.x + deltaX, targetSword.transform.position.y, targetSword.transform.position.z + deltaZ), transform.rotation);
        Destroy(nowDuplicateCloneFx, 3);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!targetSword){Destroy(gameObject);return;}
        transform.position = new Vector3(targetSword.transform.position.x + deltaX, targetSword.transform.position.y, targetSword.transform.position.z + deltaZ);
        transform.rotation = targetSword.transform.rotation;
    }
}
