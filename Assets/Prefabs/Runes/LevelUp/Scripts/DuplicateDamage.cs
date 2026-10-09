
using UnityEngine;

public class DuplicateDamage : MonoBehaviour
{
    public float duplicateDamageKPlus = 0.25f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().duplicateDamageK += duplicateDamageKPlus;
    }
}
