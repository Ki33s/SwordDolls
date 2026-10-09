
using UnityEngine;

public class SkillStun : MonoBehaviour
{
    public float stunPlusPerc = 0.02f;
    public GameObject[] runesAdded;
    private GameObject playerSword;

    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<ImpactTrigger>().stunChance += stunPlusPerc;

        if(playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().stunChance += stunPlusPerc;

        if (GetComponent<AllSkill>().newRune)
        {
            if (!playerSword.GetComponent<Sword>().stunLearned)
            {
                playerSword.GetComponent<Sword>().stunLearned = true;

                GameObject globalPars = GameObject.Find("GlobalParameters");

                for (int i = 0; i < runesAdded.Length; i++)
                {
                    globalPars.GetComponent<GlobalParameters>().runes.Add(runesAdded[i]);
                }
            }
        }
    }
}
