using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class ExpansionSetup
{
 const string Root="Assets/Campaign";
 static GameObject Load(string path)=>AssetDatabase.LoadAssetAtPath<GameObject>(path);
 static GameObject Save(GameObject go,string path){var asset=PrefabUtility.SaveAsPrefabAsset(go,path);UnityEngine.Object.DestroyImmediate(go);return asset;}
 public static void CreateAndBuild(){Create();EnemyReview.Run();RecoverySetup.Build();}
 public static void Create()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");Directory.CreateDirectory(Root+"/Expanded");AssetDatabase.Refresh();
  var global=GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>();
  string[] models={"Mage","Hermit","Jester","Prince","Queen","FortuneTeller"};
  string[] names={"Ледяной чародей","Чумной отшельник","Шут убийца","Княжеский командир","Грозовая королева","Призыватель теней"};
  string[] desc={"Снаряды замедляют на 4 секунды","Ядовитые снаряды наносят периодический урон","Прыгает за спину перед ударом","Дарит союзникам щит","Бьёт молнией по площади","Выпускает веер призрачных снарядов"};
  var styles=new[]{CampaignEnemy.Style.FrostMage,CampaignEnemy.Style.Plague,CampaignEnemy.Style.Assassin,CampaignEnemy.Style.Commander,CampaignEnemy.Style.Storm,CampaignEnemy.Style.Spectral};
  var extras=new List<GameObject>();
  for(int i=0;i<6;i++)
  {
   var actor=UnityEngine.Object.Instantiate(Load(Root+"/Enemies/Roster/Type"+(i==2?2:i==3?3:7)+".prefab"));
   var source=UnityEngine.Object.Instantiate(Load("Assets/PolygonFantasyKingdom/Prefabs/Characters/SM_Chr_"+models[i]+"_01.prefab"));
   SwapSkin(actor,source);UnityEngine.Object.DestroyImmediate(source);
   var enemy=actor.GetComponent<CampaignEnemy>();enemy.style=styles[i];enemy.displayName=names[i];enemy.skillDescription=desc[i];enemy.skillCooldown=6;enemy.projectileCooldown=4;
   actor.GetComponent<MainHealth>().maxHealth=new[]{48,55,40,80,75,65}[i];actor.GetComponent<MainHealth>().bodyLevel=7;
   extras.Add(Save(actor,Root+"/Expanded/Enemy"+i+".prefab"));
  }
  string[] cardNames={"Энергетический щит","Усиленный щит","Меч эхо","Парные эхо клинки","Орбитальный клинок","Вампирский удар","Ядовитое лезвие","Ледяное лезвие","Казнь","Магнит опыта","Восстановление щита","Цепная молния","Взрыв при убийстве","Грозовой импульс","Прочный щит","Кровавое восстановление"};
  string[] cardText={"Щит равен 25% здоровья тела. При вселении заполняется.","Щит +40% здоровья тела.","Меч рядом повторяет движения и наносит 40% урона.","Два дополнительных меча повторяют движения рядом.","Клинок вращается вокруг меча и поражает врагов.","Удары лечат тело на 10% нанесённого урона.","Попадания отравляют: 3 урона в секунду на 4 секунды.","Попадания замедляют врага на 4 секунды.","Удары по врагам с HP ниже 20% наносят ещё 50% урона.","Сферы опыта притягиваются на 4 м дальше.","Щит восстанавливает 3 единицы в секунду.","20% шанс передать половину урона соседнему врагу.","Убитые враги взрываются: 15 урона в радиусе 3,5 м.","Раз в 5 секунд импульс наносит 12 урона вокруг меча.","Щит +60% здоровья тела.","Удары лечат на дополнительные 15% урона."};
  var effects=new[]{CampaignSkill.Effect.Shield,CampaignSkill.Effect.Shield,CampaignSkill.Effect.Echo,CampaignSkill.Effect.Echo,CampaignSkill.Effect.Orbit,CampaignSkill.Effect.LifeSteal,CampaignSkill.Effect.Poison,CampaignSkill.Effect.Frost,CampaignSkill.Effect.Execute,CampaignSkill.Effect.Magnet,CampaignSkill.Effect.ShieldRegen,CampaignSkill.Effect.Chain,CampaignSkill.Effect.KillBlast,CampaignSkill.Effect.Pulse,CampaignSkill.Effect.Shield,CampaignSkill.Effect.LifeSteal};
  float[] amounts={.25f,.4f,1,2,1,.1f,3,1,.5f,4,3,.2f,15,12,.6f,.15f};
  global.runes.RemoveAll(x=>!x||AssetDatabase.GetAssetPath(x).Contains("/Expanded/Card"));
  for(int i=0;i<cardNames.Length;i++)
  {
   var go=new GameObject(cardNames[i]);go.AddComponent<AllSkill>();var skill=go.AddComponent<CampaignSkill>();skill.title=cardNames[i];skill.description=cardText[i];skill.effect=effects[i];skill.amount=amounts[i];var prefab=Save(go,Root+"/Expanded/Skill"+i+".prefab");
   var card=UnityEngine.Object.Instantiate(Load(Root+"/Cards/Card0.prefab"));var rune=card.GetComponent<LootRune>();rune.useRune=prefab;rune.runeAdded=null;rune.runeType=1;global.runes.Add(Save(card,Root+"/Expanded/Card"+i+".prefab"));
  }
  var recovery=Load(Root+"/Skills/Skill9.prefab").GetComponent<CampaignSkill>();recovery.title="Защитное вселение";recovery.description="При вселении новое тело получает дополнительный щит 15% HP.";EditorUtility.SetDirty(recovery);PrefabUtility.SavePrefabAsset(recovery.gameObject);
  var standTemplate=Load(Root+"/Evolution/Choice0.prefab");
  var choices=Enumerable.Range(0,5).Select(i=>Load(Root+"/Evolution/Choice"+i+".prefab")).ToList();
  string[] weaponSources={"E4/SwordTH01.prefab","E4/SwordTH02.prefab","E4/SpearAxe(E4).prefab","E4/Spear02(E4).prefab","E4/Mace07(E4).prefab","E4/Mace06(E4).prefab","E4/Knife04(E4).prefab","E4/Dagger05L(E4).prefab","E3/Axe01(E3).prefab","E3/Mace05(E3).prefab"};
  string[] weaponNames={"Двуручный меч","Меч палача","Алебарда","Пика","Звезда войны","Тяжёлая булава","Нож охотника","Стилет","Секира","Шипастая дубина"};
  var finalWeapons=new List<GameObject>();
  for(int i=0;i<weaponNames.Length;i++)
  {
   var go=UnityEngine.Object.Instantiate(Load("Assets/Prefabs/Weapons/Player/"+weaponSources[i]));var sword=go.GetComponent<Sword>();go.GetComponent<SwordHero>().evoSwords=Array.Empty<GameObject>();sword.forceMove=600;sword.maxSpeed=i==6||i==7?7:5.2f;
   foreach(var hit in go.GetComponentsInChildren<ImpactTrigger>(true)){hit.minDamage=22+i%4*2;hit.maxDamage=hit.minDamage+14;}
   var weapon=Save(go,Root+"/Expanded/"+weaponNames[i]+".prefab");finalWeapons.Add(weapon);var stand=UnityEngine.Object.Instantiate(standTemplate);stand.GetComponent<SwordEvo>().newSword=weapon;choices.Add(Save(stand,Root+"/Expanded/Choice"+i+".prefab"));
  }
  foreach(var guid in AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs/Weapons/Player/E1"}))
  {var prefab=Load(AssetDatabase.GUIDToAssetPath(guid));var hero=prefab.GetComponent<SwordHero>();if(!hero)continue;hero.evoSwords=choices.ToArray();foreach(var hit in prefab.GetComponentsInChildren<ImpactTrigger>(true)){hit.minDamage=Mathf.Max(14,hit.minDamage);hit.maxDamage=Mathf.Max(24,hit.maxDamage);}EditorUtility.SetDirty(prefab);PrefabUtility.SavePrefabAsset(prefab);}
  var entry=Load("Assets/Prefabs/Weapons/Player/Dagger01(E0).prefab");var firstChoices=entry.GetComponent<SwordHero>().evoSwords.Where(x=>x&&!AssetDatabase.GetAssetPath(x).Contains("/Expanded/")).ToList();
  for(int i=0;i<5;i++)
  {
   var go=UnityEngine.Object.Instantiate(finalWeapons[i]);go.GetComponent<SwordHero>().evoSwords=choices.ToArray();foreach(var hit in go.GetComponentsInChildren<ImpactTrigger>(true)){hit.minDamage=14+i;hit.maxDamage=24+i;}var weapon=Save(go,Root+"/Expanded/"+weaponNames[i]+" новичка.prefab");var stand=UnityEngine.Object.Instantiate(standTemplate);stand.GetComponent<SwordEvo>().newSword=weapon;firstChoices.Add(Save(stand,Root+"/Expanded/Entry"+i+".prefab"));
  }
  entry.GetComponent<SwordHero>().evoSwords=firstChoices.ToArray();EditorUtility.SetDirty(entry);PrefabUtility.SavePrefabAsset(entry);
  var scenery=AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/PolygonFantasyKingdom/Prefabs"}).Select(g=>AssetDatabase.GUIDToAssetPath(g)).Where(p=>p.Contains("SM_Prop_")&&(p.Contains("Barrel")||p.Contains("Crate")||p.Contains("Cart"))).Take(12).Select(Load).ToArray();
  var current=UnityEngine.Object.FindFirstObjectByType<City>();UnityEngine.Object.DestroyImmediate(current.gameObject);var stagePrefabs=new GameObject[5];
  for(int i=0;i<5;i++)
  {
   var go=UnityEngine.Object.Instantiate(Load(Root+"/Stages/Stage"+(i+1)+".prefab"));var stage=go.GetComponent<CampaignStage>();var original=stage.enemyPool.Where(x=>!AssetDatabase.GetAssetPath(x).Contains("/Expanded/")).ToList();if(i>0)original.AddRange(extras.Take(i+2));stage.enemyPool=original.Distinct().ToArray();
   var arena=go.GetComponent<CampaignArena>();if(!arena)arena=go.AddComponent<CampaignArena>();arena.props=scenery;
   stagePrefabs[i]=Save(go,Root+"/Stages/Stage"+(i+1)+".prefab");
  }
  UnityEngine.Object.Instantiate(stagePrefabs[0]);global.city=stagePrefabs.Skip(1).ToArray();EditorUtility.SetDirty(global);EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  Debug.Log("EXPANSION_SETUP_PASS cards="+global.runes.Count+" normalTypes=15 finalForgeChoices="+choices.Count+" firstForgeChoices="+firstChoices.Count+" breakableProps="+scenery.Length);
 }
 static void SwapSkin(GameObject actor,GameObject source)
 {
  var body=actor.GetComponent<Body>();var target=body.rendererModel.GetComponent<SkinnedMeshRenderer>();if(!target)throw new Exception("Ragdoll skin missing");
  string desired=source.name.Replace("(Clone)","");var skin=source.GetComponentsInChildren<SkinnedMeshRenderer>(true).FirstOrDefault(r=>r.sharedMesh&&(r.name==desired||r.sharedMesh.name==desired));if(!skin)throw new Exception("Requested character mesh missing: "+desired);Debug.Log("EXPANDED_SKIN "+desired+" mesh="+skin.sharedMesh.name);
  var bones=target.bones.Where(b=>b).GroupBy(b=>b.name).ToDictionary(g=>g.Key,g=>g.First());
  if(skin.bones.Any(b=>b&&!bones.ContainsKey(b.name)))throw new Exception("New model has incompatible ragdoll bones: "+source.name);
  target.sharedMesh=skin.sharedMesh;target.sharedMaterials=skin.sharedMaterials;target.bones=skin.bones.Select(b=>b?bones[b.name]:null).ToArray();target.localBounds=skin.localBounds;
  var path=AssetDatabase.GetAssetPath(skin.sharedMesh);if(AssetImporter.GetAtPath(path)is ModelImporter importer&&!importer.isReadable){importer.isReadable=true;importer.SaveAndReimport();}
 }
}
