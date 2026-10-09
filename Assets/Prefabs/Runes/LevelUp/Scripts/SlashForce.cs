
using UnityEngine;

public class SlashForce : MonoBehaviour
{
    public float slashForceKPlus = 0.25f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.GetComponent<Sword>().slashForceK += slashForceKPlus;
    }
}
