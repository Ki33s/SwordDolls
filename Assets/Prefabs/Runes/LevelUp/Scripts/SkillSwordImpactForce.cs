
using UnityEngine;

public class SkillSwordImpactForce : MonoBehaviour
{
    public int forcePlus = 50;
    public float forceResist = 0.05f;
    private GameObject playerSword;

    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;

        playerSword.GetComponent<Impact>().forceBack += forcePlus;
        playerSword.GetComponent<ImpactTrigger>().forceK -= forceResist;

        if (playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
        {
            playerSword.GetComponent<Sword>().tip.GetComponent<Impact>().forceBack += forcePlus;
            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().forceK -= forceResist;
        }
    }
}
