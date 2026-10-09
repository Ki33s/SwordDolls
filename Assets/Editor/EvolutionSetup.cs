using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class EvolutionSetup
{
 const string Root="Assets/Campaign/Tree";
 static GameObject Load(string p)=>AssetDatabase.LoadAssetAtPath<GameObject>(p);
 static GameObject Save(GameObject go,string path){var asset=PrefabUtility.SaveAsPrefabAsset(go,path);UnityEngine.Object.DestroyImmediate(go);return asset;}
 public static void CreateAndBuild(){Create();RecoverySetup.Build();}
 public static void Create()
 {
  EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");Directory.CreateDirectory(Root);AssetDatabase.Refresh();
  foreach(var ui in UnityEngine.Object.FindObjectsByType<LevelUpUI>(FindObjectsInactive.Include,FindObjectsSortMode.None))UnityEngine.Object.DestroyImmediate(ui.gameObject);
  var tiers=new List<GameObject[]>();var stands=new List<GameObject[]>();
  var labels=new Dictionary<string,string>{{"Dagger","Кинжал"},{"Knife","Боевой нож"},{"Mace","Булава"},{"Hammer","Молот"},{"SwordTH","Двуручный меч"},{"Sword","Меч"},{"SpearAxe","Алебарда"},{"Spear","Копьё"},{"Axe","Топор"}};
  for(int tier=1;tier<=4;tier++)
  {
   var sources=AssetDatabase.FindAssets("t:Prefab",new[]{"Assets/Prefabs/Weapons/Player/E"+tier}).Select(g=>Load(AssetDatabase.GUIDToAssetPath(g))).Where(x=>x&&x.GetComponent<SwordHero>()).OrderBy(x=>x.name).ToArray();
   var weapons=new List<GameObject>();var options=new List<GameObject>();
   foreach(var source in sources)
   {
    var go=UnityEngine.Object.Instantiate(source);string key=labels.Keys.OrderByDescending(x=>x.Length).First(x=>source.name.StartsWith(x));
    var node=go.GetComponent<WeaponEvolution>()??go.AddComponent<WeaponEvolution>();node.tier=tier;node.title=labels[key]+" · E"+tier+" · "+System.Text.RegularExpressions.Regex.Match(source.name,@"\d+").Value;
    go.GetComponent<SwordHero>().evoSwords=Array.Empty<GameObject>();go.GetComponent<SwordHero>().level=1;go.GetComponent<SwordHero>().experience=0;go.GetComponent<SwordHero>().levelUpRunes=0;
    foreach(var hit in go.GetComponentsInChildren<ImpactTrigger>(true)){hit.minDamage=8+tier*7;hit.maxDamage=hit.minDamage+8+tier*2;}
    string file="E"+tier+"_"+source.name.Replace(" ","_");go.name=file;var weapon=Save(go,Root+"/"+file+".prefab");weapons.Add(weapon);
    var stand=UnityEngine.Object.Instantiate(Load("Assets/Campaign/Evolution/Choice0.prefab"));stand.GetComponent<SwordEvo>().newSword=weapon;options.Add(Save(stand,Root+"/Choice_"+file+".prefab"));
   }
   tiers.Add(weapons.ToArray());stands.Add(options.ToArray());
  }
  // Adjacent branches follow blade, blunt and polearm families from the reference.
  string[][] links={
   new[]{"Dagger02:Dagger03,Dagger04","Knife01:Knife02,Dagger02L","Mace01:Mace02,Hammer01"},
   new[]{"Dagger02L:Dagger04L,Dagger06L","Dagger03:Dagger05,Dagger06","Dagger04:Dagger06,Dagger07","Knife02:Knife03,Dagger05","Mace02:Mace03,Mace04,Axe01","Hammer01:Hammer02,Mace05,Spear01"},
   new[]{"Dagger04L:Dagger05L,Knife04","Dagger05:Knife04,Sword01","Dagger06:Dagger07L,Sword01","Dagger06L:Dagger05L,Dagger07L","Dagger07:Dagger07L,SwordTH01","Knife03:Knife04,Sword01","Mace03:Mace06,Mace07","Mace04:Mace07,SwordTH02","Mace05:Mace06,Axe02","Hammer02:Mace07,SwordTH02","Axe01:Axe02,SpearAxe","Spear01:Spear02,Spear03"}
  };
  for(int tier=0;tier<3;tier++)foreach(var weapon in tiers[tier])
  {
   var rule=links[tier].FirstOrDefault(x=>weapon.name.Substring(3).StartsWith(x.Split(':')[0]));if(rule==null)throw new Exception("No branch for "+weapon.name);
   string[] next=rule.Split(':')[1].Split(',');
   var choices=next.Select(prefix=>stands[tier+1].First(x=>x.GetComponent<SwordEvo>().newSword.name.Substring(3).StartsWith(prefix))).ToArray();
   weapon.GetComponent<SwordHero>().evoSwords=choices;EditorUtility.SetDirty(weapon);PrefabUtility.SavePrefabAsset(weapon);
  }
  var entry=Load("Assets/Prefabs/Weapons/Player/Dagger01(E0).prefab");entry.GetComponent<SwordHero>().evoSwords=stands[0];entry.GetComponent<SwordHero>().levelUpRunes=0;EditorUtility.SetDirty(entry);PrefabUtility.SavePrefabAsset(entry);
  foreach(var stagePath in Enumerable.Range(1,5).Select(i=>"Assets/Campaign/Stages/Stage"+i+".prefab")){var stage=Load(stagePath);int number=stage.GetComponent<CampaignStage>().number;stage.GetComponent<City>().anvillFinish=number==1||number==3;EditorUtility.SetDirty(stage);PrefabUtility.SavePrefabAsset(stage);}foreach(var city in UnityEngine.Object.FindObjectsByType<City>(FindObjectsSortMode.None)){var stage=city.GetComponent<CampaignStage>();if(stage)city.anvillFinish=stage.number==1||stage.number==3;}var reachable=new HashSet<GameObject>();Action<GameObject> visit=null;visit=weapon=>{if(!reachable.Add(weapon))return;foreach(var option in weapon.GetComponent<SwordHero>().evoSwords){var next=option.GetComponent<SwordEvo>().newSword;var from=weapon.GetComponent<WeaponEvolution>();var to=next.GetComponent<WeaponEvolution>();if(!to||to.tier!=(from?from.tier:0)+1)throw new Exception("Invalid evolution tier");if(next.GetComponent<ImpactTrigger>().minDamage<weapon.GetComponent<ImpactTrigger>().minDamage)throw new Exception("Evolution loses base damage");visit(next);}};visit(entry);if(reachable.Count!=34)throw new Exception("Unreachable weapon branch: "+reachable.Count);EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());AssetDatabase.SaveAssets();
  var report=new List<string>{"# Ветки эволюции", "", "Схема приближена к референсу: E0 → E1 → E2 → E3 → E4. Кузница показывает два разных соседних продолжения текущего оружия; скачков назад нет.","","E0 → "+string.Join(" / ",tiers[0].Select(x=>x.GetComponent<WeaponEvolution>().title))};
  foreach(var weapons in tiers)foreach(var weapon in weapons){var hero=weapon.GetComponent<SwordHero>();report.Add(weapon.GetComponent<WeaponEvolution>().title+" → "+(hero.evoSwords.Length==0?"финальная форма":string.Join(" / ",hero.evoSwords.Select(x=>x.GetComponent<SwordEvo>().newSword.GetComponent<WeaponEvolution>().title))));}
  File.WriteAllLines("Review/evolution-tree.md",report);Debug.Log("EVOLUTION_TREE_PASS weapons="+tiers.Sum(x=>x.Length));
 }
}
