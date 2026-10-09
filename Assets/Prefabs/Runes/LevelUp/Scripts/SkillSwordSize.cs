
using UnityEngine;

public class SkillSwordSize : MonoBehaviour
{
    public float sizePlusK = 0.1f;
    private GameObject playerSword;
    // Start is called before the first frame update
    void Start()
    {
        playerSword = GetComponent<AllSkill>().playerSword;
        playerSword.transform.localScale = new Vector3(playerSword.transform.localScale.x + sizePlusK, playerSword.transform.localScale.y + sizePlusK, playerSword.transform.localScale.z + sizePlusK);
    }
}
