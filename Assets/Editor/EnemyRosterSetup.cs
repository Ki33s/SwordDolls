using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class EnemyRosterSetup
{
 const string Root="Assets/Campaign";
 static GameObject Load(string p)=>AssetDatabase.LoadAssetAtPath<GameObject>(p);
 static GameObject Save(GameObject go,string p){var asset=PrefabUtility.SaveAsPrefabAsset(go,p);UnityEngine.Object.DestroyImmediate(go);return asset;}
 public static void CreateAndBuild(){Create();RecoverySetup.Build();}
 public static void Create()
 {
  Directory.CreateDirectory(Root+"/EnemyWeapons");Directory.CreateDirectory(Root+"/Enemies/Roster");AssetDatabase.Refresh();
  EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
  string[] names={"Деревенский боец","Метатель мельник","Разбойница","Копейщик","Кузнец громила","Щитоносец","Берсерк","Огненный маг","Целительница","Капитан стражи","Вожак берсерков","Старший маг","Кузнец палач","Настоятельница","Военачальник"};
  string[] models={"PeasantMale Variant","Miller","PeasantFemale Variant","Soldier Variant","BlacksmithMale","Monk","BlacksmithFeMale Variant","MonkMage","Nun","Soldier Variant","BlacksmithFeMale Variant","MonkMage","BlacksmithMale","Nun","Soldier Variant"};
  string[] weapons={"Knife Variant","Stick Variant","Dagger01","Spear01","HammerTH01","Mace01","Axe01","Sceptre01","Broom1 Variant","Sword01","AxeTH01","Sceptre01","HammerTH02","Sceptre01","SpearAxe"};
  var styles=new[]{CampaignEnemy.Style.Fighter,CampaignEnemy.Style.Bomber,CampaignEnemy.Style.Skirmisher,CampaignEnemy.Style.Spearman,CampaignEnemy.Style.Heavy,CampaignEnemy.Style.Guard,CampaignEnemy.Style.Charger,CampaignEnemy.Style.Mage,CampaignEnemy.Style.Healer,CampaignEnemy.Style.Guard,CampaignEnemy.Style.Charger,CampaignEnemy.Style.Mage,CampaignEnemy.Style.Heavy,CampaignEnemy.Style.Mage,CampaignEnemy.Style.Charger};
  int[] hp={32,42,28,55,85,70,65,45,50,140,160,150,240,280,360};
  float[] speed={2.3f,2.1f,3.6f,2.5f,1.8f,2,3.3f,2.4f,2.2f,2.4f,3.5f,2.5f,1.9f,2.5f,3.2f};
  int[] damage={8,9,9,12,18,10,16,8,6,16,22,12,24,14,25};
  string[] skills={"Короткие выпады","Огненный снаряд и взрыв вблизи","Быстрый выпад и отскок","Дальний укол копьём","Удар по земле вокруг себя","Щит: 60% защиты спереди","Рывок после замаха","Огненные снаряды с дистанции","Лечит союзников в радиусе 8 м","Щит, усиленные удары","Быстрые мощные рывки","Частые огненные снаряды","Удары по земле; ускоряется при потере HP","Веер снарядов; три фазы","Рывки и снаряды; три фазы"};
  var sources=AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs/Weapons/Enemies"}).Select(g=>Load(AssetDatabase.GUIDToAssetPath(g))).Where(p=>p&&p.GetComponent<Sword>()).ToArray();
  var fallback=Load("Assets/Prefabs/Weapons/Player/Dagger01(E0).prefab");if(!fallback)fallback=AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs/Weapons/Player"}).Select(g=>Load(AssetDatabase.GUIDToAssetPath(g))).First(p=>p.GetComponent<Sword>()&&p.GetComponent<Sword>().glowSpotPrefab);
  var actors=new GameObject[names.Length];
  for(int i=0;i<actors.Length;i++)
  {
   var source=sources.FirstOrDefault(p=>Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(p))==weapons[i]);if(!source)throw new Exception("Enemy weapon missing: "+weapons[i]);
   var weapon=UnityEngine.Object.Instantiate(source);var sword=weapon.GetComponent<Sword>();if(!sword.glowSpotPrefab)sword.glowSpotPrefab=fallback.GetComponent<Sword>().glowSpotPrefab;
   sword.maxSpeed=speed[i]+1;sword.forceMove=350+damage[i]*8;
   foreach(var hit in weapon.GetComponentsInChildren<ImpactTrigger>(true)){hit.minDamage=damage[i];hit.maxDamage=damage[i]+5;}
   foreach(var impact in weapon.GetComponentsInChildren<Impact>(true))impact.forceBack=350+damage[i]*20;
   var weaponPrefab=Save(weapon,Root+"/EnemyWeapons/Type"+i+".prefab");
   var actor=UnityEngine.Object.Instantiate(Load("Assets/Prefabs/Dolls/Ready/"+models[i]+".prefab"));
   var profile=actor.GetComponent<CampaignEnemy>();if(!profile)profile=actor.AddComponent<CampaignEnemy>();profile.displayName=names[i];profile.style=styles[i];profile.skillDescription=skills[i];profile.boss=i>=12;profile.elite=i>=9;profile.skillDamage=damage[i];profile.skillCooldown=i>=9?4:6;profile.projectileCooldown=i>=9?2.5f:3.8f;profile.attackReach=styles[i]==CampaignEnemy.Style.Spearman?2.7f:styles[i]==CampaignEnemy.Style.Heavy?2.1f:1.5f;
   var health=actor.GetComponent<MainHealth>();health.maxHealth=hp[i];health.bodyLevel=i<9?3+i/2:i<12?12:22;
   actor.GetComponent<EnemyPower>().power=1;actor.GetComponent<Body>().bodySpeed=speed[i];
   actor.transform.localScale*=i>=12?1.3f:i>=9?1.15f:1;
   var spawner=actor.GetComponentInChildren<SwordSpawner>(true);spawner.mainWeapons=new[]{weaponPrefab};spawner.weaponLSpawnProb=0;
   if(styles[i]==CampaignEnemy.Style.Guard)
   {
    var shield=AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs/Weapons/Enemies"}).Select(g=>Load(AssetDatabase.GUIDToAssetPath(g))).First(p=>Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(p))=="Shield03");spawner.leftWeapons=new[]{shield};spawner.weaponLSpawnProb=1;
   }
   actors[i]=Save(actor,Root+"/Enemies/Roster/Type"+i+".prefab");
  }
  int[][] pools={new[]{0,1,2,5},new[]{0,1,2,3,5},new[]{0,2,3,4,5,6},new[]{1,2,3,5,6,7,8},new[]{0,1,2,3,4,5,6,7,8}};
  int[][] mini={Array.Empty<int>(),new[]{9},new[]{10},new[]{11},new[]{9,10}};
  int[] counts={3,4,5,7,9};
  var global=GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>();
  var sceneCity=UnityEngine.Object.FindFirstObjectByType<City>();UnityEngine.Object.DestroyImmediate(sceneCity.gameObject);
  var stages=new GameObject[5];
  for(int i=0;i<5;i++)
  {
   var stageObject=UnityEngine.Object.Instantiate(Load(Root+"/Stages/Stage"+(i+1)+".prefab"));var stage=stageObject.GetComponent<CampaignStage>();
   stage.authoredEncounters=true;stage.enemyPool=pools[i].Select(x=>actors[x]).ToArray();stage.miniBosses=mini[i].Select(x=>actors[x]).ToArray();stage.minEnemies=i==0?2:counts[i];stage.maxEnemies=i==0?4:counts[i];stage.bossPrefab=i>=2?actors[12+i-2]:null;
   var city=stageObject.GetComponent<City>();city.minPowerPerLevel=4;city.maxPowerPerLevel=4;city.newLevelMinEnemyPower=counts[i]+mini[i].Length+(stage.bossPrefab?1:0);
   stages[i]=Save(stageObject,Root+"/Stages/Stage"+(i+1)+".prefab");
  }
  UnityEngine.Object.Instantiate(stages[0]);global.city=stages.Skip(1).ToArray();EditorUtility.SetDirty(global);
  // Verification fixture remains a weak, capturable actor.
  var fixture=UnityEngine.Object.Instantiate(actors[0]);Save(fixture,"Assets/Resources/CampaignTestEnemy.prefab");
  EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  Directory.CreateDirectory("Review");
  File.WriteAllText("Review/enemies.md","# Противники\n\n"+string.Join("\n",Enumerable.Range(0,actors.Length).Select(i=>$"- {names[i]}: модель {models[i]}, HP {hp[i]}, скорость {speed[i]}, урон {damage[i]}–{damage[i]+5}. {skills[i]}."))+"\n\nЭтапы: 2–4; 5; 7; 9; 12 противников, включая минибоссов и боссов. Первый состав перемешивается, остальные гарантируют разные типы выбором без повторов до исчерпания пула. Вселение считается нейтрализацией; кристалл требует нейтрализовать состав этапа.\n");
  Debug.Log("ENEMY_ROSTER_SETUP_PASS regular=9 miniboss=3 boss=3 counts=2-4,5,7,9,12");
 }
}
