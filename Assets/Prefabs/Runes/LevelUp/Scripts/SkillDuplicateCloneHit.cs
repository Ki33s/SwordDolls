
using UnityEngine;

public class SkillDuplicateCloneHit : MonoBehaviour
{
    public float duplicateCloneHitPerc = 0.05f;
    public GameObject[] runesAdded;
    private GameObject playerSword;

    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;

        playerSword.GetComponent<ImpactTrigger>().duplicateCloneChance += duplicateCloneHitPerc;

        if (playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
            playerSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().duplicateCloneChance += duplicateCloneHitPerc;

        if (GetComponent<AllSkill>().newRune)
        {
            if (!playerSword.GetComponent<Sword>().duplicateLearned)
            {
                playerSword.GetComponent<Sword>().duplicateLearned = true;

                GameObject globalPars = GameObject.Find("GlobalParameters");

                for (int i = 0; i < runesAdded.Length; i++)
                {
                    globalPars.GetComponent<GlobalParameters>().runes.Add(runesAdded[i]);
                }
            }
        }
    }
}
