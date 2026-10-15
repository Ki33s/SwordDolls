using UnityEngine;
using System.Collections.Generic;

public class SwordHero : MonoBehaviour
{
    public GameObject[] evoSwords;
    //public List<GameObject> runes = new List<GameObject>();

    public int experience = 0;
    public int level = 1;
    public int levelUpRunes = 0;

    private GameObject expBar;

    public int minDamage;
    public int maxDamage;
    public int tipminDamage;
    public int tipmaxDamage;

    private VariableJoystick variableJoystick;
    // Start is called before the first frame update
    void Start()
    {
        if(!GetComponent<HeroCombat>())gameObject.AddComponent<HeroCombat>();
        Camera.main.GetComponent<CameraMove>().GetPlayer(gameObject);
        expBar = Camera.main.GetComponent<CameraMove>().expBar;
        GetComponent<Sword>().FirstEffects();
        GetStartDamage();
        //variableJoystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<VariableJoystick>();
    }

    void GetStartDamage()
    {
       minDamage = GetComponent<ImpactTrigger>().minDamage;
       maxDamage = GetComponent<ImpactTrigger>().maxDamage;

        if (GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
        {
            tipminDamage = GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().minDamage;
            tipmaxDamage = GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().maxDamage;
        }
    }

    public void CheckLevelUp()
    {
        if(GetComponent<Sword>().InUpgradeMenu) return;
        if(levelUpRunes > 0)
        {
            levelUpRunes--;
            GetComponent<Sword>().LevelUp();
        }
        else
        {
            //dungeon.GetComponent<Dungeon>().ProceedNextLevel();
        }
    }

    public void NewCityLevel()
    {
        GetComponent<Sword>().newLevelTimer = 0;
    }

    public void ExpGain(int exp)
    {
        experience += exp;
        while(experience >= level * 10)
        {
            experience -= level * 10;
            level++;
            levelUpRunes++;
        }

        UpdateExpBar();
    }

    void UpdateExpBar()
    {
        float sliderVal = experience / (level * 10f);
        expBar.GetComponent<UiBar2d>().ChangeBar(sliderVal, level);
    }

    public void ChooseEvo(GameObject evoSword)
    {
        /*for(int i = 0; i< evoSword.GetComponent<SwordEvo>().accuredRunes.Count; i++)
        {
            AddSkillBody(evoSword.GetComponent<SwordEvo>().accuredRunes[i]);
        }*/

        GetComponent<Sword>().ChooseEvo(evoSword);
    }

    public void AddSkillBody(GameObject skill)
    {
        //runes.Add(skill);

        bool bodySkill = skill.GetComponent<Rune>().useRune.GetComponent<AllSkill>().bodySkill;

        if (bodySkill)
        {
            GameObject nowSkill = Instantiate(skill.GetComponent<Rune>().useRune, transform.position, Quaternion.identity);
            nowSkill.GetComponent<AllSkill>().playerSword = gameObject;
        }
    }

    public void AddSkillSword(GameObject skill)
    {
        //runes.Add(skill);

        bool bodySkill = skill.GetComponent<Rune>().useRune.GetComponent<AllSkill>().bodySkill;

        if (!bodySkill)
        {
            GameObject nowSkill = Instantiate(skill.GetComponent<Rune>().useRune, transform.position, Quaternion.identity);
            nowSkill.GetComponent<AllSkill>().playerSword = gameObject;
        }
    }

    public void MoveSkills(GameObject newSword)
    {
        newSword.GetComponent<SwordHero>().minDamage = newSword.GetComponent<ImpactTrigger>().minDamage;
        newSword.GetComponent<SwordHero>().maxDamage = newSword.GetComponent<ImpactTrigger>().maxDamage;

        if(newSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>())
        {
            newSword.GetComponent<SwordHero>().tipminDamage = newSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().minDamage;
            newSword.GetComponent<SwordHero>().tipmaxDamage = newSword.GetComponent<Sword>().tip.GetComponent<ImpactTrigger>().maxDamage;
        }

        GameObject globalPars = GameObject.Find("GlobalParameters");

        for(int i = 0; i < globalPars.GetComponent<GlobalParameters>().accuredRunes.Count; i++)
        {
            GameObject nowSkill = Instantiate(globalPars.GetComponent<GlobalParameters>().accuredRunes[i], transform.position, Quaternion.identity);
            nowSkill.GetComponent<AllSkill>().playerSword = newSword;
            nowSkill.GetComponent<AllSkill>().newRune = false;
        }

        

        /*(for (int i = 0; i < runes.Count; i++)
        {
            newSword.GetComponent<SwordHero>().AddSkillSword(runes[i]);
        }*/
    }

    public void UseBodySkills()
    {
        /*for(int i = 0; i<runes.Count; i++)
        {
            if (runes[i].GetComponent<Rune>().useRune.GetComponent<AllSkill>().bodySkill)
            {
                GameObject nowSkill = Instantiate(runes[i].GetComponent<Rune>().useRune, transform.position, Quaternion.identity);
                nowSkill.GetComponent<AllSkill>().playerSword = gameObject;
            }
        }*/
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        Controls();
        //AndroidControls();
    }

    void Controls()
    {
        float axisHorizontal = SwordInput.Read().x;
        float axisVertical = SwordInput.Read().y;
        Vector3 moveVector = Vector3.zero;

        if (axisHorizontal != 0 || axisVertical != 0)
        {
            moveVector = Vector3.ClampMagnitude(new Vector3(axisHorizontal, 0, axisVertical), 1f);
        }

        GetComponent<Sword>().moveVector = moveVector;
    }

    void AndroidControls()
    {
        float axisHorizontal = variableJoystick.Direction.x;
        float axisVertical = variableJoystick.Direction.y;
        Vector3 moveVector = Vector3.zero;
        //Debug.Log(Input.GetAxis("Vertical"));
        if (axisHorizontal != 0 || axisVertical != 0)
        {
            moveVector = Vector3.ClampMagnitude(new Vector3(axisHorizontal, 0, axisVertical), 1f);
        }

        GetComponent<Sword>().moveVector = moveVector;
    }

}
