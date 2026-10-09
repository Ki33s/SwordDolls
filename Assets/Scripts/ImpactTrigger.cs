
using UnityEngine;

public class ImpactTrigger : MonoBehaviour
{
    private float forceBack = 0;
    public int minDamage = 5;
    public int maxDamage = 10;
    public float forceK = 1f;
    public float couldown = 0.2f;
    public int dollId = 0;
    public bool hasDoll = false;
    public float bashChance = 0f;
    public float bashMinTime = 0.5f;
    public float bashMaxTime = 1f;
    public GameObject bashFx;

    public float stunChance = 0f;
    public float stunMinTime = 0.5f;
    public float stunMaxTime = 1f;
    public GameObject stunFx;

    public float critChance = 0f;
    public float critDamageK = 2f;
    public float slashChance = 0f;
    public float slashRadius = 1f;
    public float prickChance = 0f;
    public float duplicateCloneChance = 0f;
    public bool isEnabled = true;

    public GameObject impactSound;

    public GameObject damageScore;

    private float timer = 0f;

    private Rigidbody rig;

    private Vector3 forceDir;

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if(forceBack > 0)
                rig.AddForce(forceDir * 100 * forceBack * Time.deltaTime);
        }
            
    }

    void OnTriggerEnter(Collider myTrigger)
    {
        //if(timer <= 0)
        //{
        if(isEnabled)
            if (myTrigger.GetComponent<Impact>())
            {
                if (myTrigger.GetComponent<Impact>().dollId != dollId)
                {
                    forceBack = myTrigger.GetComponent<Impact>().forceBack * forceK;
                    Vector3 hitPos = myTrigger.ClosestPoint(transform.position);

                    if (forceBack > 0)
                    {
                            //forceDir = new Vector3(-velocity.x, 0, -velocity.z);
                            forceDir = (transform.position - myTrigger.transform.position).normalized;
                            forceDir = new Vector3(forceDir.x, 0, forceDir.z);
                        timer = couldown;
                        //timer = Random.Range(-couldown/2,couldown + couldown/2);

                        if (hasDoll)
                        {
                            if(transform.root.GetComponent<MainHealth>())
                                transform.root.GetComponent<MainHealth>().Pushed(Mathf.RoundToInt(forceBack), forceDir);

                            if(transform.root.GetComponent<Body>())
                            {
                                transform.root.GetComponent<Body>().FootPushed(forceDir, hitPos);
                            }
                        }
                    }

                        

                        
                        GameObject hitEffect = Instantiate(myTrigger.GetComponent<Impact>().effect, hitPos, Quaternion.LookRotation(transform.position - myTrigger.transform.position, -transform.up));
                        
               
                
                    
                        Destroy(hitEffect, 3);

                    if (impactSound)
                    {
                        GameObject nowSound = Instantiate(impactSound, transform.position, Quaternion.identity);
                        nowSound.GetComponent<ImpactSoundEffect>().impactType = myTrigger.GetComponent<Impact>().impactType;
                    }



                    if (myTrigger.GetComponent<Health>())
                    {
                        if(hasDoll)
                        {
                            int damage = Random.Range(minDamage, maxDamage);
                            int healthForce = GetComponent<Impact>().forceBack;

                            GameObject score = Instantiate(damageScore, hitPos, Quaternion.identity);


                            //CheckCrit
                            if (critChance>0)
                            {
                                if (Random.Range(0, 1f) < critChance)
                                {
                                    damage = Mathf.RoundToInt(damage*critDamageK);
                                    score.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                                }
                            }

                             score.GetComponent<TextMesh>().text = damage.ToString();


                            if (healthForce > 0)
                                myTrigger.GetComponent<Health>().Damage(damage, myTrigger.GetComponent<Rigidbody>(), healthForce, -forceDir);

                            HeroCombat.Hit(this,myTrigger.GetComponent<Health>(),damage);
                            if (transform.root.GetComponent<Body>())
                            {
                                transform.root.GetComponent<Body>().FootPushed(forceDir, hitPos);
                            }

                            //CheckStun
                            if (stunChance != 0)
                            {
                                if (!myTrigger.transform.root.GetComponent<Body>().sword.GetComponent<Sword>().stunned)
                                    if (Random.Range(0, 1f) < stunChance)
                                    {
                                        myTrigger.transform.root.GetComponent<Body>().sword.GetComponent<Sword>().stunTimer = Random.Range(stunMinTime, stunMaxTime);
                                        myTrigger.transform.root.GetComponent<Body>().sword.GetComponent<Sword>().stunned = true;
                                        myTrigger.transform.root.GetComponent<Body>().stunned = true;
                                
                                        GameObject newStun = Instantiate(stunFx, myTrigger.transform.root.GetComponent<Body>().head.transform.position, Quaternion.identity, myTrigger.transform.root.GetComponent<Body>().head.transform);
                                        Destroy(newStun, 3);
                                    }
                            }
                            
                    }
                                else if(!GetComponent<Sword>().followBody)
                                {
                                    var campaignEnemy = myTrigger.transform.root.GetComponent<CampaignEnemy>();
                                    if(campaignEnemy && !campaignEnemy.CanCapture) return;
                                    if(campaignEnemy) campaignEnemy.Capture();
                                    GetComponent<Sword>().MoveInBody(myTrigger.transform.root.gameObject);
                                    myTrigger.GetComponent<Health>().MoveInBody(gameObject);
                            //myTrigger.transform.root.GetComponent<Body>().CheckBodySkills();
                            GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>().nowCity.GetComponent<City>().RemoveEnemy(myTrigger.transform.root.gameObject);
                                }

                    
                }


                
                
                    if (myTrigger.GetComponent<Sword>())
                    {
                    //CheckBash
                    if (bashChance != 0)
                        if (myTrigger.transform.root.GetComponent<Body>())
                        {
                            if (!myTrigger.transform.root.GetComponent<Body>().bashedSword)
                            {
                                if (Random.Range(0, 1f) < bashChance)
                                {
                                    myTrigger.GetComponent<Sword>().BashedSword(Random.Range(bashMinTime,bashMaxTime));
                                    GameObject newBash = Instantiate(bashFx, hitPos, Quaternion.LookRotation(transform.position - myTrigger.transform.position, -transform.up));
                                    Destroy(newBash, 3);
                                }
                            }
                                
                        }

                    //CheckSlash
                    if(slashChance > 0)
                    if(Random.Range(0,1f) < slashChance)
                    {
                        if(GetComponent<Sword>())
                                {
                                    GetComponent<Sword>().Slash(GetComponent<Sword>().slashFX);
                                }
                        else
                                {
                                    transform.parent.GetComponent<Sword>().Slash(GetComponent<Sword>().slashFX);
                                }
                    }


                    //CheckPrick
                    if (prickChance > 0)
                        if (Random.Range(0, 1f) < prickChance)
                        {
                                if (GetComponent<Sword>())
                                {
                                    GetComponent<Sword>().Slash(GetComponent<Sword>().prickFX);
                                }
                                else
                                {
                                    transform.parent.GetComponent<Sword>().Slash(GetComponent<Sword>().prickFX);
                                }
                            }

                    if(duplicateCloneChance > 0)
                        {
                            if(Random.Range(0f,1f) < duplicateCloneChance)
                            {
                                if (GetComponent<Sword>())
                                {
                                    GetComponent<Sword>().DuplicateClone();
                                }
                                else
                                {
                                    transform.parent.GetComponent<Sword>().DuplicateClone();
                                }
                            }
                        }

                }

            }

               
            }
    }

}
