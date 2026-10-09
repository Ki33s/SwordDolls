
using UnityEngine;

public class SlashScale : MonoBehaviour
{
    public float slashScalePlus = 0.5f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().slashScale += slashScalePlus;
    }
}
