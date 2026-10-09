
using UnityEngine;

public class SkillSlashHit : MonoBehaviour
{
    public float slashPlusPerc = 0.05f;
    public GameObject[] runesAdded;
    private GameObject playerSword;

    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;

        playerSword.GetComponent<ImpactTrigger>().slashChance += slashPlusPerc;

        if (playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().slashChance += slashPlusPerc;

        if (GetComponent<AllSkill>().newRune)
        {
            if (!playerSword.GetComponent<Sword>().slashLearned)
            {
                playerSword.GetComponent<Sword>().slashLearned = true;

                GameObject globalPars = GameObject.Find("GlobalParameters");

                for (int i = 0; i < runesAdded.Length; i++)
                {
                    globalPars.GetComponent<GlobalParameters>().runes.Add(runesAdded[i]);
                }
            }
        }
    }
}
