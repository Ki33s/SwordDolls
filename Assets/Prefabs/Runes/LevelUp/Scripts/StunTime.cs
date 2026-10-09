
using UnityEngine;

public class StunTime : MonoBehaviour
{
    public float stunMinTimePlus = 0.25f;
    public float stunMaxTimePlus = 0.5f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;

        playerSword.GetComponent<ImpactTrigger>().stunMinTime += stunMinTimePlus;
        playerSword.GetComponent<ImpactTrigger>().stunMaxTime += stunMaxTimePlus;

        if (playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
        {
            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().stunMinTime += stunMinTimePlus;
            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().stunMaxTime += stunMaxTimePlus;
        }
    }
}
