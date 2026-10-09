
using UnityEngine;

public class SkillCrit : MonoBehaviour
{
    public float critChance = 0.2f;
    private GameObject playerSword;

    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<ImpactTrigger>().critChance += critChance;

        if (playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().critChance += critChance;
    }
}
