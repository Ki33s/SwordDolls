
using UnityEngine;

public class SkillDuplicateShot : MonoBehaviour
{
    public float duplicateShotCouldown = 5f;
    public GameObject[] runesAdded;
    public GameObject[] evoRunes;
    private GameObject playerSword;

    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().duplicateShot = true;
        playerSword.GetComponent<Sword>().duplicateShotCouldown = duplicateShotCouldown;

        if (GetComponent<AllSkill>().newRune)
        {
            GameObject globalPars = GameObject.Find("GlobalParameters");
            globalPars.GetComponent<GlobalParameters>().runes.RemoveAt(GetComponent<AllSkill>().runeId);

            for (int i = 0; i < evoRunes.Length; i++)
            {
                globalPars.GetComponent<GlobalParameters>().runes.Add(evoRunes[i]);
            }

            if (!playerSword.GetComponent<Sword>().duplicateLearned)
            {
                playerSword.GetComponent<Sword>().duplicateLearned = true;

                for (int i = 0; i < runesAdded.Length; i++)
                {
                    globalPars.GetComponent<GlobalParameters>().runes.Add(runesAdded[i]);
                }
            }
        }
    }
}
