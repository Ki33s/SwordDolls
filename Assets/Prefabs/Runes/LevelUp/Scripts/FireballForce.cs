
using UnityEngine;

public class FireballForce : MonoBehaviour
{
    public float fireballForceKPlus = 0.25f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().fireballForceK += fireballForceKPlus;
    }
}
