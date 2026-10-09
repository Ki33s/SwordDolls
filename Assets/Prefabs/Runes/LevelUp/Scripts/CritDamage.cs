
using UnityEngine;

public class CritDamage : MonoBehaviour
{
    public float critDamageK = 0.2f;
    private GameObject playerSword;

    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;

        playerSword.GetComponent<ImpactTrigger>().critDamageK += critDamageK;

        if (playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().critDamageK += critDamageK;
    }
}
