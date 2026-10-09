
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public float health;
    private int startMaxHealth;
    private bool playerHealthApplied;
    public float forceK = 0.5f;
    public float couldown = 0.1f;
    public float healthForceMultiplier = 0.25f;
    public float playerHealthMultiplier = 1.5f;

    public Image healthBar;
    public Image nowHealthBar;
    public int healthBarDeltaY = 30;

    public GameObject dieSound;
    public float voiceDieProb = 0.5f;
    private List<Rigidbody> healthParts = new List<Rigidbody>();
    private Rigidbody hitPart;
    private float hitForce;
    private Vector3 hitVector;
    private float timer = 0f;
    public int bodyLevel;

    private float healthPartSpeed;
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        startMaxHealth = maxHealth;
        healthPartSpeed = GetComponent<Body>().bodySpeed;
        SpawnHealthBar();
        GetHealthParts(transform);
    }

    private void FixedUpdate()
    {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                MoveParts();   
            }

        MoveHealthBar();
    }

    void MoveHealthBar()
    {
        Vector2 camPos = Camera.main.WorldToScreenPoint(GetComponent<Body>().head.transform.position);
        nowHealthBar.GetComponent<Transform>().position = new Vector2(camPos.x, camPos.y + healthBarDeltaY);
    }

    public void IncreaseMaxHealth(float healthPlusK)
    {
        maxHealth += Mathf.RoundToInt(startMaxHealth * healthPlusK);
        health += Mathf.RoundToInt(health * healthPlusK);

        if (health > maxHealth)
            health = maxHealth;
    }

    void MoveParts()
    {
        for(int i = 0; i < healthParts.Count; i++)
        {
            healthParts[i].AddForce(hitVector * 100 * hitForce  * Time.deltaTime);
        }

        if(hitPart)
            hitPart.AddForce(hitVector * 100 * hitForce  * Time.deltaTime);
    }

    void GetHealthParts(Transform GO)
    {
        for (int i = 0; i < GO.childCount; i++)
        {
            if (GO.GetChild(i).GetComponent<HealthPart>())
            {
                GO.GetChild(i).GetComponent<HealthPart>().maxSpeed = healthPartSpeed * GO.GetChild(i).GetComponent<HealthPart>().speedK;
                healthParts.Add(GO.GetChild(i).GetComponent<Rigidbody>());
            }
                
            GetHealthParts(GO.GetChild(i));
        }
    }

    public void SetMaxSpeed()
    {
        healthPartSpeed = GetComponent<Body>().bodySpeed;

        for (int i = 0; i < healthParts.Count; i++)
        {
            healthParts[i].GetComponent<HealthPart>().maxSpeed = healthPartSpeed * healthParts[i].GetComponent<HealthPart>().speedK;
        }
    }

    void SpawnHealthBar()
    {
        nowHealthBar = Instantiate(healthBar);
        nowHealthBar.transform.SetParent(GameObject.Find("CanvasExp").transform, false);
        Vector2 camPos = Camera.main.WorldToScreenPoint(GetComponent<Body>().head.transform.position);
        nowHealthBar.GetComponent<Transform>().position = new Vector2(camPos.x, camPos.y + healthBarDeltaY);
    }

    void UpdateHealthBar()
    {
        nowHealthBar.GetComponent<HealthBar>().UpdateBar(health / maxHealth);
    }

    public void Heal(float healPerc)
    {
        if(health < maxHealth)
        {
            float newHealth = health + maxHealth * healPerc;

            if (newHealth > maxHealth)
                newHealth = maxHealth;

            health = newHealth;

            UpdateHealthBar();
            GetComponent<Body>().UpdateHealth(health / maxHealth * healthForceMultiplier + 1 - healthForceMultiplier);
        }
    }

    public void Pushed(int force, Vector3 dir)
    {
        hitPart = null;
        hitForce = force * forceK;
        hitVector = dir;
        timer = couldown;
        //timer = Random.Range(-couldown / 2, couldown + couldown / 2);
    }

    public void Damage(float damage, Rigidbody rigPart, int force, Vector3 dir)
    {
        var activeBody=GetComponent<Body>(); if(!activeBody || !activeBody.sword || !activeBody.sword.GetComponent<Sword>())return;
        if(health > 0)
        {
            var profile=GetComponent<CampaignEnemy>(); if(profile)damage=profile.IncomingDamage(damage,dir);
            var shield=GetComponent<BodyShield>();if(shield)damage=shield.Absorb(damage);
            if(damage<=0)return;
            health -= damage;
            if (health <= 0)
            {
                
                if(Random.Range(0,1f) < voiceDieProb)
                {
                    Instantiate(dieSound, transform.position, Quaternion.identity);
                }

                if (GetComponent<Body>().sword.GetComponent<Sword>().player)
                {
                    Camera.main.GetComponent<CameraMove>().LooseLife();
                }
                else
                {
                    if(!GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>().nowCity.GetComponent<City>().crystalDestroyed)
                        GetComponent<Body>().sword.GetComponent<SwordAI>().playerSword.GetComponent<SwordHero>().ExpGain(bodyLevel);

                    var player=GameObject.FindGameObjectWithTag("Player");var combat=player?player.GetComponent<HeroCombat>():null;if(combat)combat.OnKilled(transform.position);
                    CampaignExperience.Spawn(GetComponent<Body>().head.transform.position, Mathf.Max(1, bodyLevel / 2));
                    GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>().nowCity.GetComponent<City>().RemoveEnemy(gameObject);
                }
                    

                GetComponent<Body>().sword.GetComponent<Sword>().DropLeft();
                GetComponent<Body>().sword.GetComponent<Sword>().DropSword();
               
                
                PartsDead(transform);
                Destroy(GetComponent<Body>());
                Destroy(GetComponent<MainHealth>());

                Destroy(nowHealthBar.gameObject);
            }
            else
            {
                UpdateHealthBar();
                hitPart = rigPart;
                hitForce = force * forceK;
                hitVector = dir;
                GetComponent<Body>().UpdateHealth(health / maxHealth * healthForceMultiplier + 1 - healthForceMultiplier);
                timer = couldown;
                //timer = Random.Range(-couldown / 2, couldown + couldown / 2);
            }
        }  
    }

    public void MoveInBody(GameObject sword)
    {
        GetComponent<Body>().MoveInBody(sword);
    }

    public void NewSword()
    {
        if(playerHealthApplied)return; playerHealthApplied=true;
        startMaxHealth = Mathf.RoundToInt(startMaxHealth * playerHealthMultiplier); 
        maxHealth = Mathf.RoundToInt(maxHealth * playerHealthMultiplier);
        health = Mathf.RoundToInt(health * playerHealthMultiplier);
    }

    void PartsDead(Transform GO)
    {
        GO.gameObject.layer = default;

        if (GO.GetComponent<Health>())
        {
            Destroy(GO.GetComponent<Health>());
        }

        if (GO.GetComponent<ImpactTrigger>())
            Destroy(GO.GetComponent<ImpactTrigger>());

        if (GO.GetComponent<Impact>())
            Destroy(GO.GetComponent<Impact>());

        for (int i = 0; i < GO.childCount; i++)
        {
            PartsDead(GO.GetChild(i));
        }
    }
}
