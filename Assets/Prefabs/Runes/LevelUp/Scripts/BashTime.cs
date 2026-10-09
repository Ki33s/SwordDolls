
using UnityEngine;

public class BashTime : MonoBehaviour
{
    public float bashMinTimePlus = 0.25f;
    public float bashMaxTimePlus = 0.5f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;

        playerSword.GetComponent<ImpactTrigger>().bashMinTime += bashMinTimePlus;
        playerSword.GetComponent<ImpactTrigger>().bashMaxTime += bashMaxTimePlus;

        if (playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
        {
            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().bashMinTime += bashMinTimePlus;
            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().bashMaxTime += bashMaxTimePlus;
        }
    }
}
