using System.Collections.Generic;
using UnityEngine;
public sealed class CampaignStage : MonoBehaviour
{
 public int number=1; public string title;
 public bool authoredEncounters;
 public GameObject[] enemyPool=new GameObject[0],miniBosses=new GameObject[0];
 public int minEnemies=3,maxEnemies=3;
 public GameObject bossPrefab;
 public CampaignEnemy Boss {get;private set;}
 public int PlannedCount {get;private set;}
 public int SpawnedCount {get;private set;}
 public bool RosterSpawned {get;private set;}
 readonly List<CampaignEnemy> elites=new List<CampaignEnemy>();
 int normalCount; bool bossSpawned;
 public bool BossDefeated
 {
  get
  {
   if(authoredEncounters&&!RosterSpawned)return false;
   if(bossPrefab&&(!bossSpawned||(Boss&&!Boss.Captured&&Boss.GetComponent<MainHealth>())))return false;
   foreach(var actor in elites)if(actor&&!actor.Captured&&actor.GetComponent<MainHealth>())return false;
   return true;
  }
 }
 void Awake()
 {
  if(!authoredEncounters)return;
  normalCount=Random.Range(minEnemies,maxEnemies+1);
  PlannedCount=normalCount+miniBosses.Length+(bossPrefab?1:0);
  GetComponent<City>().newLevelMinEnemyPower=PlannedCount;
 }
 void Start(){if(authoredEncounters)Invoke(nameof(SpawnRoster),.5f);if(bossPrefab)Invoke(nameof(SpawnBoss),2);}
 void SpawnRoster()
 {
  var available=new List<GameObject>(enemyPool);
  for(int i=0;i<normalCount;i++)
  {
   if(available.Count==0)available.AddRange(enemyPool);
   if(available.Count==0)break;
   int index=Random.Range(0,available.Count);Spawn(available[index],i);available.RemoveAt(index);
  }
  foreach(var prefab in miniBosses)elites.Add(Spawn(prefab,SpawnedCount));
  RosterSpawned=true;
 }
 CampaignEnemy Spawn(GameObject prefab,int index)
 {
  var arena=GetComponent<CampaignArena>();Vector3 point=arena?arena.FreePoint(index):new Vector3(index*2,.15f,7);Vector3 center=Vector3.zero;
  var actor=Instantiate(prefab,point,Quaternion.LookRotation(Vector3.ProjectOnPlane(center-point,Vector3.up)));
  GetComponent<City>().AddEnemy(actor);SpawnedCount++;return actor.GetComponent<CampaignEnemy>();
 }
 void SpawnBoss(){Boss=Spawn(bossPrefab,SpawnedCount);bossSpawned=true;}
}
