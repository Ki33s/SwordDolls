
using UnityEngine;

public class AllSkill : MonoBehaviour
{
    public GameObject runeAdded;
    public bool bodySkill = false;
    public bool permanent = false;
    public GameObject playerSword;
    public int runeId;
    public bool newRune = true;

    private void Start()
    {
        if(!newRune) return;
        GameObject globalPars = GameObject.Find("GlobalParameters");
        if(runeId >= 0 && runeId < globalPars.GetComponent<GlobalParameters>().runes.Count) globalPars.GetComponent<GlobalParameters>().runes.RemoveAt(runeId);

        if(runeAdded != null)
            globalPars.GetComponent<GlobalParameters>().runes.Add(runeAdded);
    }
}
