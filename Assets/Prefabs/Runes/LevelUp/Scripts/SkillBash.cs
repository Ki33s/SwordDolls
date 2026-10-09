
using UnityEngine;

public class SkillBash : MonoBehaviour
{
    public float bashPlusPerc = 0.02f;
    public GameObject[] runesAdded;
    private GameObject playerSword;

    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;

        playerSword.GetComponent<ImpactTrigger>().bashChance += bashPlusPerc;

        if (playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().bashChance += bashPlusPerc;

        if (GetComponent<AllSkill>().newRune)
        {
            if (!playerSword.GetComponent<Sword>().bashLearned)
            {
                playerSword.GetComponent<Sword>().bashLearned = true;

                GameObject globalPars = GameObject.Find("GlobalParameters");

                for (int i = 0; i < runesAdded.Length; i++)
                {
                    globalPars.GetComponent<GlobalParameters>().runes.Add(runesAdded[i]);
                }
            }
        }
    }
}
