using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
public sealed class CampaignVerification : MonoBehaviour
{
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
 static void Initialize(){if(Array.IndexOf(Environment.GetCommandLineArgs(),"-verifyCampaign")>=0)new GameObject("Campaign verification").AddComponent<CampaignVerification>();}
 string runtimeError;float started;
 void OnEnable(){Application.logMessageReceived+=Log;started=Time.realtimeSinceStartup;}
 void OnDisable(){Application.logMessageReceived-=Log;}
 void Log(string message,string stack,LogType type){if(type==LogType.Exception||type==LogType.Error)runtimeError=message+" "+stack;}
 void Check(bool value,string message){if(!value)throw new Exception(message);}
 GlobalParameters Global=>GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>();
 SwordHero Hero=>GameObject.FindGameObjectWithTag("Player").GetComponent<SwordHero>();
 CampaignStage Stage=>Global.nowCity.GetComponent<CampaignStage>();
 IEnumerator Start()
 {
  SwordInput.TestEnabled=true;SwordInput.TestAxes=Vector2.zero;
  var routine=Verify();
  while(true)
  {
   object current=null;bool next=false;string failure=null;
   try{if(runtimeError!=null)throw new Exception(runtimeError);if(Time.realtimeSinceStartup-started>120)throw new Exception("Verification timeout");next=routine.MoveNext();if(next)current=routine.Current;}
   catch(Exception error){failure=error.ToString();}
   if(failure!=null){Debug.Log("CAMPAIGN_VERIFY_FAIL "+failure);Application.Quit(1);yield break;}
   if(!next)break;yield return current;
  }
  Debug.Log("CAMPAIGN_VERIFY_PASS: expanded unique card families, full possession heal, bonus shield absorption, echo blade, breakable scenery, stationary upgrade choice, real exit crystal proximity across five stages, two forges, progress replay and victory");Application.Quit();
 }
 IEnumerator Frames(int count){for(int i=0;i<count;i++)yield return null;}
 void VerifyGround()
 {
  var city=Global.nowCity.GetComponent<City>();
  var visuals=city.GetComponentInChildren<TerrainVisuals>();
  Check(visuals&&visuals.GetComponentsInChildren<Renderer>().Length>=city.tilesNum*city.tilesNum,"Ground artwork missing");
  Check(!visuals.GetComponentsInChildren<Collider>(true).Any(c=>c.enabled),"Terrain/grass collision must be removed");
  Check(!visuals.GetComponentInChildren<BreakableProp>(),"Scenery preparation must skip ground artwork");
  var floor=city.transform.Find("GroundCollider(Clone)").GetComponent<BoxCollider>();
  Check(floor.enabled&&!floor.isTrigger&&Mathf.Abs(floor.bounds.max.y-city.transform.position.y)<.001f,"Flat support surface missing or wrong height");
  float radius=city.tilesNum*city.tilesLength*.5f-1;
  for(int x=-4;x<=4;x++)for(int z=-4;z<=4;z++)
  {
   var origin=city.transform.position+new Vector3(x*radius/4,3,z*radius/4);
   Check(floor.Raycast(new Ray(origin,Vector3.down),out var hit,4)&&Mathf.Abs(hit.point.y-city.transform.position.y)<.001f,"Ground must be flat across the whole arena");
  }
  Debug.Log("FLAT_GROUND_PASS stage="+Stage.number+" grid=81 artworkRenderers="+visuals.GetComponentsInChildren<Renderer>().Length);
 }
 IEnumerator DrainCards()
 {
  int guard=0;
  while(Hero.levelUpRunes>0||FindFirstObjectByType<LevelUpUI>())
  {
   if(FindFirstObjectByType<Anvill>() || (CampaignHUD.Active&&CampaignHUD.Active.Victory))yield break;
   if(++guard>10000)throw new Exception("Card queue stuck");var ui=FindFirstObjectByType<LevelUpUI>();if(ui&&ui.Options!=null)ui.Select(0);yield return null;yield return null;
  }
 }
 IEnumerator ChooseAxes(Func<int> selected,Func<bool> exists,int count=3)
 {
  SwordInput.TestAxes=Vector2.zero;yield return null;yield return null;
  int before=selected();SwordInput.TestAxes=Vector2.right;yield return null;yield return null;
  Check(selected()==(before+1)%count,"Horizontal axis should move focus exactly once");
  SwordInput.TestAxes=Vector2.zero;yield return null;yield return null;
  SwordInput.TestAxes=Vector2.up;yield return null;yield return null;SwordInput.TestAxes=Vector2.zero;
  Check(!exists(),"Vertical axis should confirm the focused option");
 }
 IEnumerator VerifyEnemySkills()
 {
  var pool=Global.city[3].GetComponent<CampaignStage>().enemyPool;
  Check(pool.Length>=15 && pool.Distinct().Count()==pool.Length,"Nine ordinary enemy variants required");
  var actors=new List<GameObject>();
  foreach(var style in new[]{CampaignEnemy.Style.Fighter,CampaignEnemy.Style.Healer,CampaignEnemy.Style.Guard,CampaignEnemy.Style.Mage,CampaignEnemy.Style.Heavy})
  {
   var prefab=pool.First(x=>x.GetComponent<CampaignEnemy>().style==style);
   var actor=Instantiate(prefab,new Vector3(60+actors.Count*3,3,60),Quaternion.identity);actors.Add(actor);
   actor.GetComponent<CampaignEnemy>().enabled=false;
   foreach(var rb in actor.GetComponentsInChildren<Rigidbody>())rb.isKinematic=true;
  }
  yield return Frames(5);
  foreach(var actor in actors){var ai=actor.GetComponent<Body>().sword.GetComponent<SwordAI>();if(ai)ai.enabled=false;}
  var fighter=actors[0].GetComponent<MainHealth>();fighter.health=10;
  var healer=actors[1].GetComponent<CampaignEnemy>();healer.SendMessage("HealAllies");Check(fighter.health==22,"Healer must restore nearby ally health");
  actors[0].GetComponent<CampaignEnemy>().Capture();healer.SendMessage("HealAllies");Check(fighter.health==22,"Healer must not heal captured player body");
  var guard=actors[2].GetComponent<CampaignEnemy>();Vector3 facing=actors[2].GetComponent<Body>().head.transform.forward;
  Check(Mathf.Approximately(guard.IncomingDamage(100,-facing),40),"Guard front protection missing");
  Check(Mathf.Approximately(guard.IncomingDamage(100,facing),100),"Guard rear must remain vulnerable");guard.Capture();Check(guard.IncomingDamage(100,-facing)==100,"Enemy shield passive must stop on possession");
  var mage=actors[3].GetComponent<CampaignEnemy>();mage.SendMessage("Shoot",Vector3.forward);Check(mage.SkillUses==1,"Mage projectile skill failed");
  var heavy=actors[4].GetComponent<CampaignEnemy>();heavy.SendMessage("Slam",3.2f);Check(heavy.SkillUses==1,"Heavy radial skill failed");
  foreach(var actor in actors)Destroy(actor);yield return Frames(2);
  Debug.Log("ENEMY_SKILLS_PASS fifteen variants, ally healing, captured ally exclusion, front/rear shield, projectile and radial FX");
 }
 IEnumerator Verify()
 {
  yield return new WaitForSecondsRealtime(2);
  Check(!FindFirstObjectByType<LevelUpUI>()&&Hero.level==1&&Hero.levelUpRunes==0&&Global.accuredRunes.Count==0,"A new run must not start with a free skill");Check(Stage.number==1,"Campaign must start at stage 1");Check(Global.city.Length==4,"Exactly four onward stage references");Check(Global.runes.Count>=20,"Expanded card deck missing");
  Check(Stage.authoredEncounters && Stage.PlannedCount>=2 && Stage.PlannedCount<=4,"First round must contain 2–4 enemies");
  float rosterDeadline=Time.realtimeSinceStartup+10;while(!Stage.RosterSpawned){yield return DrainCards();yield return null;Check(Time.realtimeSinceStartup<rosterDeadline,"First roster spawn stuck");}
  Check(Stage.SpawnedCount==Stage.PlannedCount,"First roster not complete");
  yield return VerifyEnemySkills();
  var seen=new HashSet<GameObject>();
  for(int n=0;n<1000;n++){var draw=CardDraw.Draw(Global.runes,3);Check(draw.Count==3&&draw.Select(c=>c.GetComponent<LootRune>().useRune).Distinct().Count()==3 && draw.Select(CardDraw.Family).Distinct().Count()==3,"Duplicate upgrade card in draw");foreach(var c in draw)seen.Add(c);}
  Check(seen.Count>=18,"Weighted deck does not expose enough variety");Debug.Log("CAMPAIGN_DRAW 1000 unique hands; seen="+seen.Count);
  yield return DrainCards();
  var hero=Hero;int acquired=Global.accuredRunes.Count;hero.ExpGain(hero.level*10-hero.experience);
  yield return Frames(3);var menu=FindFirstObjectByType<LevelUpUI>();Check(menu&&Time.timeScale==0,"Hero level must pause and open cards");
  int blockedCount=Global.accuredRunes.Count;menu.Select(0);Check(menu&&Global.accuredRunes.Count==blockedCount,"Card selection must be blocked during first second");SwordInput.TestAxes=Vector2.up;yield return new WaitForSecondsRealtime(1.05f);yield return Frames(2);Check(menu,"Held combat axis must not select a card after delay");SwordInput.TestAxes=Vector2.zero;Check(menu.Options.Select(x=>x.GetComponent<LootRune>().useRune).Distinct().Count()==3,"Visible card choices repeat");
  Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Application.dataPath,"../../../Review")));
  ScreenCapture.CaptureScreenshot(Path.GetFullPath(Path.Combine(Application.dataPath,"../../../Review/card-menu.png")));yield return Frames(3);
  Vector3 menuPosition=Hero.transform.position;yield return Frames(3);Check(Vector3.Distance(menuPosition,Hero.transform.position)<.01f,"Sword must stay in place during level choice");
  yield return ChooseAxes(()=>menu.Selected,()=>menu!=null);
  yield return Frames(3);Check(Global.accuredRunes.Count==acquired+1,"Chosen card not saved");Check(Time.timeScale==1,"Game must resume after card choice");
  int xp=Hero.experience;CampaignExperience.Spawn(Hero.transform.position,3);yield return new WaitForSecondsRealtime(.3f);Check(Hero.experience==xp+3,"Experience pickup not credited");
  // Exercise the original collision callback with a live enemy collider, then
  // let the actual rigidbodies/hand joint complete the possession.
  var fixture=Instantiate(Resources.Load<GameObject>("CampaignTestEnemy"),Hero.transform.position+Vector3.forward,Quaternion.identity);
  Global.nowCity.GetComponent<City>().AddEnemy(fixture);yield return new WaitForSecondsRealtime(.25f);
  var part=fixture.GetComponentsInChildren<Health>().First(h=>h.GetComponent<Collider>());
  fixture.GetComponent<MainHealth>().health=5;Hero.GetComponent<Sword>().bodyHealthBonus=.2f;Hero.GetComponent<HeroCombat>().shieldBonus=.25f;
  Hero.GetComponent<ImpactTrigger>().SendMessage("OnTriggerEnter",part.GetComponent<Collider>(),SendMessageOptions.RequireReceiver);
  var capturedHealth=fixture.GetComponent<MainHealth>();Check(capturedHealth.health==capturedHealth.maxHealth,"Possession must restore all HP immediately");
  var capturedShield=fixture.GetComponent<BodyShield>();Check(capturedShield&&Mathf.Approximately(capturedShield.Capacity,capturedHealth.maxHealth*.45f),"Health bonuses and shield card must create shield");
  capturedHealth.maxHealth=100000;capturedHealth.health=100000;
  yield return new WaitForSecondsRealtime(2);
  Check(fixture.GetComponent<CampaignEnemy>().Captured,"Original possession callback failed");
  Check(Hero.GetComponent<Sword>().head==fixture.GetComponent<Body>().head,"Player must inherit captured body");
  capturedShield.Refill();float hpBefore=capturedHealth.health;capturedHealth.Damage(5,fixture.GetComponent<Body>().head.GetComponent<Rigidbody>(),0,Vector3.zero);Check(capturedHealth.health==hpBefore&&capturedShield.Current<capturedShield.Capacity,"Shield must absorb incoming damage");
  var combat=Hero.GetComponent<HeroCombat>();combat.echoes=1;yield return new WaitForSecondsRealtime(.2f);var echo=FindFirstObjectByType<EchoBlade>();Check(echo&&echo.owner==combat,"Echo blade must appear");
  var cube=GameObject.CreatePrimitive(PrimitiveType.Cube);cube.transform.position=new Vector3(12,1,12);BreakableProp.Setup(cube);var prop=cube.GetComponent<BreakableProp>();float beforeHp=prop.hp;prop.SendMessage("OnTriggerEnter",Hero.GetComponent<Collider>());Check(prop.hp<beforeHp,"Weapon contact must damage scenery");prop.Damage(100,Hero.transform.position);yield return Frames(2);Check(!prop,"Scenery must break into physical debris");
  fixture.GetComponent<MainHealth>().maxHealth=100000;fixture.GetComponent<MainHealth>().health=100000;
  yield return DrainCards();
  int forges=0;
  for(int number=1;number<=5;number++)
  {
   Check(Hero.GetComponent<Sword>().newLevelTimer==0,"Transition must not launch sword upwards");
   Check(Stage.number==number,"Incorrect round sequence");Debug.Log("CAMPAIGN_STAGE "+number+" "+Stage.title);
   yield return new WaitForSecondsRealtime(2.2f);
   VerifyGround();
   yield return DrainCards();
   float populationDeadline=Time.realtimeSinceStartup+10;while(Stage.SpawnedCount<Stage.PlannedCount){yield return DrainCards();yield return null;Check(Time.realtimeSinceStartup<populationDeadline,"Roster spawn stuck "+Stage.SpawnedCount+"/"+Stage.PlannedCount);}
   if(Stage.bossPrefab)
   {
    float spawnDeadline=Time.realtimeSinceStartup+10; while(!Stage.Boss){yield return DrainCards();yield return null;Check(Time.realtimeSinceStartup<spawnDeadline,"Boss spawn stuck");}
    Check(Stage.Boss&&!Stage.BossDefeated,"Boss did not spawn");var health=Stage.Boss.GetComponent<MainHealth>();Check(!Stage.Boss.CanCapture,"Healthy boss must resist instant possession");
    health.Damage(health.maxHealth*.5f,Stage.Boss.GetComponent<Body>().head.GetComponent<Rigidbody>(),0,Vector3.zero);yield return new WaitForSecondsRealtime(.1f);Check(Stage.Boss.Phase>=2,"Boss phase transition missing");
    health.Damage(health.maxHealth,Stage.Boss.GetComponent<Body>().head.GetComponent<Rigidbody>(),0,Vector3.zero);yield return null;
    foreach(var actor in Global.nowCity.GetComponent<City>().enemies.ToArray()){var hp=actor?actor.GetComponent<MainHealth>():null;if(hp && actor.GetComponent<CampaignEnemy>().elite)hp.Damage(hp.maxHealth*3,actor.GetComponent<Body>().head.GetComponent<Rigidbody>(),0,Vector3.zero);}
    yield return null;Check(Stage.BossDefeated,"Boss/mini-boss gate did not clear");
   }
   yield return DrainCards();
   Check(Stage.SpawnedCount==Stage.PlannedCount,"Encounter count mismatch "+Stage.SpawnedCount+"/"+Stage.PlannedCount);
   int expected=new[]{0,0,5,7,9,12}[number];Check(number==1||Stage.PlannedCount==expected,"Round population must grow");
   int deckCount=Global.runes.Count,skillCount=Global.accuredRunes.Count;var city=Global.nowCity.GetComponent<City>();
   foreach(var actor in city.enemies.ToArray()){var hp=actor?actor.GetComponent<MainHealth>():null;if(hp&&hp.health>0)hp.Damage(hp.maxHealth*4,actor.GetComponent<Body>().head.GetComponent<Rigidbody>(),0,Vector3.zero);}
   yield return DrainCards();float readyDeadline=Time.realtimeSinceStartup+6;while(!city.crystalDestroyed&&(!city.CurrentCrystal||!city.CurrentCrystal.Ready)){yield return DrainCards();yield return null;Check(Time.realtimeSinceStartup<readyDeadline,"Actual exit crystal did not unlock: neutralized="+city.NeutralizedCount+" planned="+Stage.PlannedCount+" alive="+city.enemies.Count+" bossesClear="+Stage.BossDefeated); }
   if(!city.crystalDestroyed){
   var arena=city.GetComponent<CampaignArena>();Check(arena&&arena.Inside(city.CurrentCrystal.transform.position),"Exit must be inside arena");
   Vector3 exit=city.CurrentCrystal.transform.position;var sword=Hero.GetComponent<Sword>();var root=sword.head?sword.head.transform.root:Hero.transform;Vector3 delta=Vector3.ProjectOnPlane(exit-Hero.transform.position,Vector3.up);root.position+=delta;if(Hero.transform.root!=root)Hero.transform.position+=delta;
   float exitDeadline=Time.realtimeSinceStartup+6;while(city&&!city.crystalDestroyed){yield return DrainCards();yield return null;Check(Time.realtimeSinceStartup<exitDeadline,"Real exit proximity failed to finish round");}
   }
   yield return Frames(2);
   if(number==1||number==3)
   {
    var forge=FindFirstObjectByType<Anvill>();Check(forge&&forge.Options.Count==2,"Forge must offer two distinct weapons");
    Check(forge.Options.Select(x=>x.GetComponent<SwordEvo>().newSword).Distinct().Count()==2,"Duplicate forge weapon");
    ScreenCapture.CaptureScreenshot(Path.GetFullPath(Path.Combine(Application.dataPath,"../../../Review/forge-menu.png")));yield return Frames(3);
    deckCount=Global.runes.Count;skillCount=Global.accuredRunes.Count;
    var previous=Hero.gameObject;int level=Hero.level,experience=Hero.experience;
    yield return ChooseAxes(()=>forge.Selected,()=>forge!=null,2);yield return Frames(5);
    Check(Hero.gameObject!=previous,"Evolution did not replace weapon");Check(Hero.GetComponent<WeaponEvolution>()&&Hero.GetComponent<WeaponEvolution>().tier==forges+1,"Evolution must advance exactly one tier");Check(Hero.level*(Hero.level-1)*5+Hero.experience>=level*(level-1)*5+experience,"Evolution lost total hero experience");
    Check(Global.accuredRunes.Count==skillCount&&Global.runes.Count==deckCount,"Replaying skills altered the upgrade deck");forges++;
    float equipDeadline=Time.realtimeSinceStartup+7;while(!Hero.GetComponent<Sword>().hasDoll){yield return DrainCards();yield return null;Check(Time.realtimeSinceStartup<equipDeadline,"New weapon failed to reconnect to carried body");}
    var carried=Hero.GetComponent<Sword>().head.transform.root.GetComponent<Body>();foreach(var joint in carried.handlerR.GetComponents<FixedJoint>())Check(joint.connectedBody,"Old hand joint still anchors body to world");
   }
   else Check(!FindFirstObjectByType<Anvill>(),"Forge appeared on the wrong round");
   if(number<5)
   {
    float deadline=Time.realtimeSinceStartup+10;
    while(Stage.number==number){yield return DrainCards();yield return null;Check(Time.realtimeSinceStartup<deadline,"Round transition stuck");}
   }
  }
  Check(forges==2,"Exactly two forges expected");Check(CampaignHUD.Active.Victory&&Time.timeScale==0,"Fifth round must end in victory");
 }
}
