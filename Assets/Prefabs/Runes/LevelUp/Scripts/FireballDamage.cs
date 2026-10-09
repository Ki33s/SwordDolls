
using UnityEngine;

public class FireballDamage : MonoBehaviour
{
    public float fireballDamageKPlus = 0.25f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().fireballDamageK += fireballDamageKPlus;
    }
}
