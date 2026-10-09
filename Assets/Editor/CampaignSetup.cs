using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class CampaignSetup
{
 const string Root="Assets/Campaign";
 static T Load<T>(string path) where T:UnityEngine.Object=>AssetDatabase.LoadAssetAtPath<T>(path);
 static GameObject Save(GameObject instance,string path){var result=PrefabUtility.SaveAsPrefabAsset(instance,path);UnityEngine.Object.DestroyImmediate(instance);return result;}
 public static void Create()
 {
  foreach(var folder in new[]{Root,Root+"/Enemies",Root+"/Cards",Root+"/Skills",Root+"/Weapons",Root+"/Evolution",Root+"/Houses",Root+"/Stages",Root+"/Materials"})Directory.CreateDirectory(folder);
  if(!File.Exists(Root+"/Interface.ttf"))File.Copy("C:/Windows/Fonts/arial.ttf",Root+"/Interface.ttf");AssetDatabase.Refresh();
  EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
  var global=GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>();
  // Repair backward/terminal evolution and references that point at weapons instead of SwordEvo stands.
  var displays=AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs/Weapons/Player/Anvill"}).Select(g=>Load<GameObject>(AssetDatabase.GUIDToAssetPath(g))).Where(p=>p.GetComponent<SwordEvo>()&&p.GetComponent<SwordEvo>().newSword).ToArray();
  foreach(var guid in AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs/Weapons/Player"}))
  {
   var path=AssetDatabase.GUIDToAssetPath(guid);var prefab=Load<GameObject>(path);var hero=prefab.GetComponent<SwordHero>();if(!hero)continue;
   if(path.Contains("/E4/"))hero.evoSwords=Array.Empty<GameObject>();
   else hero.evoSwords=hero.evoSwords.Where(x=>x).Select(x=>x.GetComponent<SwordEvo>()?x:displays.FirstOrDefault(d=>d.GetComponent<SwordEvo>().newSword==x)).Where(x=>x).Distinct().ToArray();
   EditorUtility.SetDirty(prefab);PrefabUtility.SavePrefabAsset(prefab);
  }
  string[] bases={"E4/Sword01(E4) Variant.prefab","E4/Axe02(E4).prefab","E4/Spear03(E4).prefab","E4/Dagger07L(E4).prefab","E3/Hammer02(E3).prefab"};
  string[] names={"Длинный меч стража","Боевой топор","Копьё прорыва","Кинжал ловкача","Молот кузнеца"};
  var choices=new List<GameObject>();
  for(int i=0;i<bases.Length;i++)
  {
   var instance=UnityEngine.Object.Instantiate(Load<GameObject>("Assets/Prefabs/Weapons/Player/"+bases[i]));instance.name=names[i];var sword=instance.GetComponent<Sword>();var hero=instance.GetComponent<SwordHero>();hero.evoSwords=Array.Empty<GameObject>();
   sword.maxSpeed=new[]{6f,5.2f,4.8f,7f,4.5f}[i];sword.forceMove=550;
   foreach(var hit in instance.GetComponentsInChildren<ImpactTrigger>(true)){hit.minDamage=new[]{20,25,22,16,28}[i];hit.maxDamage=new[]{32,38,34,27,42}[i];}
   var weapon=Save(instance,Root+"/Weapons/Weapon"+i+".prefab");
   var stand=UnityEngine.Object.Instantiate(displays.First());stand.name=names[i];stand.GetComponent<SwordEvo>().newSword=weapon;choices.Add(Save(stand,Root+"/Evolution/Choice"+i+".prefab"));
  }
  foreach(var guid in AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs/Weapons/Player/E1"}))
  {var prefab=Load<GameObject>(AssetDatabase.GUIDToAssetPath(guid));var hero=prefab.GetComponent<SwordHero>();if(hero){hero.evoSwords=choices.ToArray();EditorUtility.SetDirty(prefab);PrefabUtility.SavePrefabAsset(prefab);}}
  var templates=new[]{"PeasantMale Variant","Soldier Variant","BlacksmithMale","Soldier Variant","MonkMage","Soldier Variant","BlacksmithMale"};
  var enemyNames=new[]{"Боец","Ловкач","Тяжёлый боец","Копейщик","Маг","Элитный копейщик","Палач"};
  var styles=new[]{CampaignEnemy.Style.Fighter,CampaignEnemy.Style.Skirmisher,CampaignEnemy.Style.Heavy,CampaignEnemy.Style.Spearman,CampaignEnemy.Style.Mage,CampaignEnemy.Style.Spearman,CampaignEnemy.Style.Heavy};
  var enemies=new List<GameObject>();
  for(int i=0;i<templates.Length;i++)
  {
   var source=Load<GameObject>("Assets/Prefabs/Dolls/Ready/"+templates[i]+".prefab");var actor=UnityEngine.Object.Instantiate(source);actor.name=enemyNames[i];
   actor.GetComponent<MainHealth>().maxHealth=new[]{35,45,85,65,60,180,280}[i];actor.GetComponent<MainHealth>().bodyLevel=new[]{5,6,8,8,10,22,35}[i];actor.GetComponent<EnemyPower>().power=new[]{1,1,2,2,2,4,5}[i];
   var profile=actor.AddComponent<CampaignEnemy>();profile.style=styles[i];profile.displayName=enemyNames[i];profile.boss=i>=5;profile.elite=i==5;
   var spawner=actor.GetComponentInChildren<SwordSpawner>(true);string[] weapons={"Knife","Dagger01","HammerTH01","Spear01","Sceptre01","Spear02","HammerTH02"};
   var match=AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs/Weapons/Enemies"}).Select(g=>Load<GameObject>(AssetDatabase.GUIDToAssetPath(g))).FirstOrDefault(p=>p.name==weapons[i]&&p.GetComponent<Sword>());
   if(match)spawner.mainWeapons=new[]{match};spawner.weaponLSpawnProb=0;
   actor.GetComponent<Body>().bodySpeed=i==1?3.5f:i==2?1.8f:2.5f;
   var material=new Material(actor.GetComponent<Body>().rendererModel.GetComponent<Renderer>().sharedMaterial);material.color=new[]{new Color(.7f,.8f,.65f),new Color(.55f,.75f,.9f),new Color(.8f,.5f,.3f),new Color(.5f,.7f,.8f),new Color(.65f,.45f,.85f),new Color(.9f,.7f,.2f),new Color(.85f,.3f,.2f)}[i];AssetDatabase.CreateAsset(material,Root+"/Materials/Enemy"+i+".mat");actor.GetComponent<Body>().rendererModel.GetComponent<Renderer>().sharedMaterial=material;
   enemies.Add(Save(actor,Root+"/Enemies/Enemy"+i+".prefab"));
  }
  var elite=UnityEngine.Object.Instantiate(enemies[6]);elite.name="Элитный палач";elite.GetComponent<CampaignEnemy>().elite=true;elite.GetComponent<CampaignEnemy>().displayName=elite.name;elite.GetComponent<MainHealth>().maxHealth=420;elite.GetComponent<MainHealth>().bodyLevel=50;var finalBoss=Save(elite,Root+"/Enemies/FinalBoss.prefab");
  string[] cardTitles={"Заточенный клинок","Быстрая рука","Длинное лезвие","Тяжёлый удар","Точный удар","Огненный шар","Призрачный двойник","Волна клинка","Пробивающий укол","Восстановление","Мощная заточка","Лёгкое оружие","Широкий клинок","Сильный толчок"};
  string[] descriptions={"Урон оружия +20%.","Скорость оружия +12%.","Размер оружия +10%.","Сила отбрасывания +25%.","Шанс критического удара +10%.","Оружие выпускает огненный шар каждые 3 секунды.","Шанс вызвать временного двойника при ударе: +15%.","Открывает волну клинка с перезарядкой 3 секунды.","Открывает пробивающий укол и повышает шанс его срабатывания.","Новое тело получает на 15% больше лечения при вселении.","Урон оружия +35%.","Скорость оружия +18%.","Размер оружия +15%.","Сила отбрасывания +40%."};
  CampaignSkill.Effect[] effects={CampaignSkill.Effect.Damage,CampaignSkill.Effect.Speed,CampaignSkill.Effect.Size,CampaignSkill.Effect.Force,CampaignSkill.Effect.Critical,CampaignSkill.Effect.Fireball,CampaignSkill.Effect.Clone,CampaignSkill.Effect.Slash,CampaignSkill.Effect.Pierce,CampaignSkill.Effect.Recovery,CampaignSkill.Effect.Damage,CampaignSkill.Effect.Speed,CampaignSkill.Effect.Size,CampaignSkill.Effect.Force};
  float[] amounts={.2f,.12f,.1f,.25f,.1f,.2f,.15f,.2f,.15f,.15f,.35f,.18f,.15f,.4f};
  global.runes.RemoveAll(x=>!x);
  var cardTemplate=Load<GameObject>("Assets/Prefabs/UI/Runes/Bronze/Damage/RuneDamage.prefab");
  for(int i=0;i<cardTitles.Length;i++)
  {
   var skillObject=new GameObject(cardTitles[i]);skillObject.AddComponent<AllSkill>();var skill=skillObject.AddComponent<CampaignSkill>();skill.title=cardTitles[i];skill.description=descriptions[i];skill.effect=effects[i];skill.amount=amounts[i];var skillPrefab=Save(skillObject,Root+"/Skills/Skill"+i+".prefab");
   var card=UnityEngine.Object.Instantiate(cardTemplate);card.name=cardTitles[i];var rune=card.GetComponent<LootRune>();rune.useRune=skillPrefab;rune.runeAdded=null;rune.runeType=i<5?2:1;global.runes.Add(Save(card,Root+"/Cards/Card"+i+".prefab"));
  }
  var current=UnityEngine.Object.FindFirstObjectByType<City>();var baseCity=UnityEngine.Object.Instantiate(current.gameObject);baseCity.name="Campaign base";
  string[] stageTitles={"Окраина деревни","Старая мельница","Двор кузницы","Монастырь","Крепость палача"};
  string[] houseSources={"Hut","WindMill","BlacksmithBuild","Church","BlacksmithBuild"};
  var stages=new List<GameObject>();
  for(int i=0;i<5;i++)
  {
   var cityObject=UnityEngine.Object.Instantiate(baseCity);cityObject.name=stageTitles[i];var city=cityObject.GetComponent<City>();city.level=i;city.anvillFinish=i==1||i==3;city.newLevelMinEnemyPower=4+i*2;city.minPowerPerLevel=8+i*3;city.maxPowerPerLevel=city.minPowerPerLevel+3;city.tilesNum=i<2?5:6;city.newLevelCouldown=2;
   var stage=cityObject.AddComponent<CampaignStage>();stage.number=i+1;stage.title=stageTitles[i];stage.bossPrefab=i==2?enemies[6]:i==3?enemies[5]:i==4?finalBoss:null;
   var house=UnityEngine.Object.Instantiate(Load<GameObject>("Assets/Prefabs/City/Buildings/Ready/"+houseSources[i]+".prefab"));house.name="Дом этапа "+(i+1);var houseScript=house.GetComponent<House>();houseScript.housePower=4+i*2;houseScript.enemy=new List<GameObject>(i==0?new[]{enemies[0]}:i==1?new[]{enemies[0],enemies[1],enemies[3]}:i==2?new[]{enemies[2],enemies[1]}:i==3?new[]{enemies[3],enemies[4]}:new[]{enemies[1],enemies[2],enemies[3],enemies[4]});city.houses=new[]{Save(house,Root+"/Houses/House"+i+".prefab")};
   stages.Add(Save(cityObject,Root+"/Stages/Stage"+(i+1)+".prefab"));
  }
  UnityEngine.Object.DestroyImmediate(baseCity);UnityEngine.Object.DestroyImmediate(current.gameObject);UnityEngine.Object.Instantiate(stages[0]);
  global.city=stages.Skip(1).ToArray();global.accuredRunes.Clear();
  var hud=UnityEngine.Object.FindFirstObjectByType<CampaignHUD>();if(!hud)hud=new GameObject("Five round campaign").AddComponent<CampaignHUD>();hud.font=Load<Font>(Root+"/Interface.ttf");hud.joystickPrefab=Load<GameObject>("Assets/Joystick Pack/Prefabs/Variable Joystick.prefab");
  EditorUtility.SetDirty(global);EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  Debug.Log("CAMPAIGN_SETUP_PASS stages=5 cards="+global.runes.Count+" newWeapons="+choices.Count+" enemyTypes="+enemies.Count);
 }
}
