
using UnityEngine;

public class FireballShotCouldown : MonoBehaviour
{
    public float fireballShotCouldownK = 0.25f;
    private GameObject playerSword;

    private void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().fireballShotCouldown -= playerSword.GetComponent<Sword>().fireballShotCouldown * fireballShotCouldownK;
    }
}
