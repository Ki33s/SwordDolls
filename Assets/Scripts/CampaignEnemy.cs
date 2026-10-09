using UnityEngine;
[DefaultExecutionOrder(50)]
public sealed class CampaignEnemy : MonoBehaviour
{
 public enum Style { Fighter, Skirmisher, Heavy, Spearman, Mage, Guard, Charger, Healer, Bomber, FrostMage, Plague, Storm, Assassin, Commander, Spectral }
 public Style style;public string displayName,skillDescription;
 public bool boss,elite;
 public float projectileCooldown=3,skillCooldown=5,attackReach=1.5f;
 public int skillDamage=8;
 public int SkillUses {get;private set;}
 public bool Captured {get;private set;}
 public int Phase {get;private set;}=1;
 public bool CanCapture=>!(boss||elite)||!GetComponent<MainHealth>()||GetComponent<MainHealth>().health<=GetComponent<MainHealth>().maxHealth*.25f;
 Body body;Sword weapon;SwordAI ai;float timer,shotTimer,specialTimer,chargeTimer;int action;GameObject player;Vector3 chargeDirection;
 void Awake(){body=GetComponent<Body>();specialTimer=2;}
 void FixedUpdate()
 {
  if(Captured||!body||!body.sword)return;
  weapon=body.sword.GetComponent<Sword>();if(!weapon)return;
  if(weapon.player){Capture();return;}
  if(!ai)ai=weapon.GetComponent<SwordAI>();if(ai){ai.enabled=false;ai.playerSword=GameObject.FindGameObjectWithTag("Player");}
  player=GameObject.FindGameObjectWithTag("Player");if(!player)return;
  if(body.stunned||weapon.stunned){weapon.moveVector=Vector3.zero;return;}
  var health=GetComponent<MainHealth>();Phase=boss&&health?(elite&&health.health<health.maxHealth*.3f?3:health.health<health.maxHealth*.6f?2:1):1;
  // Keep unrelated combat out of the isolated mechanics verification; phase logic still runs.
  if(SwordInput.TestEnabled){weapon.moveVector=Vector3.zero;return;}
  Vector3 delta=Vector3.ProjectOnPlane(player.transform.position-weapon.transform.position,Vector3.up);
  float distance=delta.magnitude;Vector3 direction=delta.normalized;
  if(distance>22){weapon.moveVector=Vector3.zero;return;}
  timer-=Time.fixedDeltaTime;shotTimer-=Time.fixedDeltaTime;specialTimer-=Time.fixedDeltaTime;
  if(style==Style.Assassin&&specialTimer<=0&&distance<10){var arena=FindFirstObjectByType<CampaignArena>();Vector3 point=player.transform.position-direction*2;point.y=.15f;if(arena&&arena.Inside(point)&&!Physics.CheckSphere(point+Vector3.up*1.1f,.5f,~0,QueryTriggerInteraction.Ignore)){transform.position+=Vector3.ProjectOnPlane(point-body.head.transform.position,Vector3.up);SkillUses++;Effect(Color.magenta,1);}specialTimer=skillCooldown;}
  if(style==Style.Commander&&specialTimer<=0){foreach(var ally in FindObjectsByType<CampaignEnemy>(FindObjectsSortMode.None)){if(ally.Captured||ally==this||Vector3.Distance(transform.position,ally.transform.position)>7)continue;var shield=ally.GetComponent<BodyShield>();if(!shield)shield=ally.gameObject.AddComponent<BodyShield>();shield.Configure(12,true);}SkillUses++;Effect(Color.yellow,3);specialTimer=skillCooldown;}
  if(style==Style.Storm&&specialTimer<=0&&distance<6){Slam(6);specialTimer=skillCooldown;}
  if(style==Style.Healer&&specialTimer<=0){HealAllies();specialTimer=skillCooldown;}
  if(style==Style.Charger&&distance<10)
  {
   if(chargeTimer>0){chargeTimer-=Time.fixedDeltaTime;weapon.moveVector=chargeTimer>.65f?-chargeDirection*.15f:chargeDirection*2.5f;return;}
   if(specialTimer<=0){chargeDirection=direction;chargeTimer=1.15f;specialTimer=skillCooldown/Phase;SkillUses++;Effect(new Color(1,.35f,.12f),2);}
  }
  if((style==Style.Heavy||style==Style.Bomber)&&specialTimer<=0&&distance<4.5f)
  {Slam(style==Style.Bomber?4.5f:3.2f);specialTimer=skillCooldown/Phase;}
  if(style==Style.Mage||style==Style.Bomber||style==Style.Healer||style==Style.FrostMage||style==Style.Plague||style==Style.Spectral)
  {
   weapon.moveVector=distance<4?-direction:distance>7?direction*.6f:Vector3.Cross(Vector3.up,direction)*.35f;
   if(shotTimer<=0&&distance<14&&style!=Style.Healer){Shoot(direction);if(boss||style==Style.Spectral){Shoot(Quaternion.Euler(0,18,0)*direction);Shoot(Quaternion.Euler(0,-18,0)*direction);}shotTimer=projectileCooldown/Phase;}
   return;
  }
  if(distance>attackReach+1){weapon.moveVector=direction*(style==Style.Skirmisher?1.2f:.8f);action=0;timer=0;return;}
  if(timer<=0)
  {
   action=(action+1)%3;
   timer=action==1?(style==Style.Heavy?.65f:.4f)/Phase:action==2?(style==Style.Skirmisher?.3f:.6f):(style==Style.Heavy?.9f:.35f)/Phase;
   if(action==1&&boss&&Phase>1&&shotTimer<=0){Shoot(direction);shotTimer=projectileCooldown;}
  }
  weapon.moveVector=action==0?-direction*.12f:action==1?direction*(style==Style.Heavy?1.3f:1.1f):style==Style.Skirmisher?-direction*.85f:Vector3.Cross(Vector3.up,direction)*.35f;
 }
 public float IncomingDamage(float damage,Vector3 direction)
 {
  if(Captured||style!=Style.Guard||!body||!body.head)return damage;
  Vector3 facing=Vector3.ProjectOnPlane(body.head.transform.forward,Vector3.up).normalized;
  return Vector3.Dot(facing,-direction.normalized)>.3f?damage*.4f:damage;
 }
 void HealAllies()
 {
  bool healed=false;
  foreach(var ally in FindObjectsByType<CampaignEnemy>(FindObjectsSortMode.None))
  {
   var hp=ally.GetComponent<MainHealth>();if(!hp||ally.Captured||ally==this||Vector3.Distance(transform.position,ally.transform.position)>8)continue;
   if(hp.health<hp.maxHealth){hp.health=Mathf.Min(hp.maxHealth,hp.health+12);healed=true;}
  }
  if(healed){SkillUses++;Effect(Color.green,8);}
 }
 void Slam(float radius)
 {
  SkillUses++;Effect(style==Style.Bomber?Color.magenta:Color.red,radius);
  if(!player)return;var sword=player.GetComponent<Sword>();if(!sword||!sword.head)return;
  var target=sword.head.transform.root.GetComponent<MainHealth>();if(!target||Vector3.Distance(body.head.transform.position,sword.head.transform.position)>radius)return;
  target.Damage(skillDamage*Phase,sword.head.GetComponent<Rigidbody>(),600,(sword.head.transform.position-body.head.transform.position).normalized);
 }
 void Effect(Color color,float radius)
 {
  var go=new GameObject("Enemy skill pulse");go.transform.position=body.head.transform.position-Vector3.up*.7f;
  var particles=go.AddComponent<ParticleSystem>();particles.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);var main=particles.main;main.startColor=color;main.startSize=.16f;main.startSpeed=radius*2;main.startLifetime=.4f;main.loop=false;main.duration=.5f;
  var emission=particles.emission;emission.rateOverTime=0;emission.SetBursts(new[]{new ParticleSystem.Burst(0,40)});
  var shape=particles.shape;shape.shapeType=ParticleSystemShapeType.Circle;shape.radius=.2f;shape.rotation=new Vector3(90,0,0);
  var fx=GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>().fireball;
  if(fx){var source=fx.GetComponentInChildren<ParticleSystemRenderer>();if(source)particles.GetComponent<ParticleSystemRenderer>().sharedMaterial=source.sharedMaterial;}
  particles.Play();Destroy(go,1.5f);
 }
 void Shoot(Vector3 direction)
 {
  var global=GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>();if(!global.fireball)return;
  SkillUses++;var shot=Instantiate(global.fireball,body.head.transform.position+direction*.6f,Quaternion.FromToRotation(Vector3.up,direction));
  var hit=shot.GetComponent<BulletTrigger>();if(hit){hit.dollId=body.dollId;hit.minDamage=skillDamage;hit.maxDamage=Mathf.RoundToInt(skillDamage*1.5f);hit.forceBack=250;}
  if(style==Style.Plague||style==Style.FrostMage){var status=shot.AddComponent<EnemyProjectileStatus>();status.poison=style==Style.Plague?2:0;status.frost=style==Style.FrostMage?1:0;CombatFx.Pulse(shot.transform.position,style==Style.Plague?Color.green:Color.cyan,.5f);}
  if(!shot.GetComponent<Bullet>())shot.AddComponent<Bullet>();Destroy(shot,6);
 }
 public void Capture(){Captured=true;if(ai)ai.enabled=false;}
}
