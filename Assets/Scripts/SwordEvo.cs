
using UnityEngine;

public class SwordEvo : MonoBehaviour
{
    public GameObject newSword;
    public GameObject anvill;
    public bool used = false;
    public GameObject textRight;
    public GameObject textLeft;

    private void OnTriggerEnter(Collider other)
    {
        if(!used)
            if (other.tag == "Player")
            {
                other.GetComponent<Sword>().ChooseEvo(gameObject);
                other.GetComponent<SwordHero>().NewCityLevel();
                GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>().nowCity.GetComponent<City>().newLevelStart = true;
            }
    }

    public void ChangeText()
    {
        textRight.SetActive(false);
        textLeft.SetActive(true);
    }
}
