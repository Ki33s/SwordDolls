
using UnityEngine;

public class Health : MonoBehaviour
{
    private GameObject mainHealth;
    public bool dead = false;

    private void Start()
    {
        mainHealth = transform.root.gameObject;
    }

    public void Damage(float damage, Rigidbody rigPart, int force, Vector3 dir)
    {
        mainHealth.GetComponent<MainHealth>().Damage(damage, rigPart, force, dir);

        if(GetComponent<Bleeding>())
        {
            if (mainHealth.GetComponent<MainHealth>().health <= 0)
                GetComponent<Bleeding>().CheckBleeding();
        }
    }

    public void MoveInBody(GameObject sword)
    {
        mainHealth.GetComponent<MainHealth>().MoveInBody(sword);
    }
}
