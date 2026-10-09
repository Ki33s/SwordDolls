using System.Collections.Generic;
using UnityEngine;
public class Anvill : MonoBehaviour
{
 public GameObject swordPlace,leftE,rightE;
 public List<GameObject> Options {get;private set;}=new List<GameObject>();
 public int Selected=>choice.Selected;
 Sword player;AxisChoice choice=new AxisChoice();bool chosen;
 void Start()
 {
  player=GameObject.FindGameObjectWithTag("Player").GetComponent<Sword>();
  var candidates=new List<GameObject>();foreach(var item in player.GetComponent<SwordHero>().evoSwords)if(item&&!candidates.Contains(item))candidates.Add(item);
  while(Options.Count<2&&candidates.Count>0)
  {
   int index=Random.Range(0,candidates.Count);var prefab=candidates[index];candidates.RemoveAt(index);
   if(!prefab.GetComponent<SwordEvo>()||!prefab.GetComponent<SwordEvo>().newSword)continue;
   Vector3 point=Options.Count==0?leftE.transform.position:Options.Count==1?rightE.transform.position:(leftE.transform.position+rightE.transform.position)*.5f+Vector3.forward*2;
   var option=Instantiate(prefab,point,Quaternion.identity,transform);option.GetComponent<SwordEvo>().anvill=gameObject;
   foreach(var collider in option.GetComponentsInChildren<Collider>())collider.enabled=false;
   Options.Add(option);
  }
  CampaignMenu.Open();player.PrepareAxisEvolution();
 }
 void Update(){if(chosen||Options.Count==0)return;int index=choice.Step(SwordInput.Read(),Options.Count);if(index>=0)Select(index);}
 public void Select(int index){if(chosen||index<0||index>=Options.Count)return;player.PrepareAxisEvolution();player.GetComponent<SwordHero>().ChooseEvo(Options[index]);}
 public void EvoChoosed(GameObject evoSword){chosen=true;foreach(var option in Options)if(option)option.GetComponent<SwordEvo>().used=true;Debug.Log("CAMPAIGN_EVOLUTION "+evoSword.GetComponent<SwordEvo>().newSword.name);Destroy(gameObject);}
 void OnDestroy(){CampaignMenu.Close();}
 static string WeaponTitle(string name)
 {
  switch(name)
  {
   case "Weapon0": return "Длинный меч стража";
   case "Weapon1": return "Боевой топор";
   case "Weapon2": return "Копьё прорыва";
   case "Weapon3": return "Кинжал ловкача";
   case "Weapon4": return "Молот кузнеца";
  }
  if(name.StartsWith("Dagger"))return "Кинжал";
  if(name.StartsWith("Knife"))return "Боевой нож";
  if(name.StartsWith("Mace"))return "Булава";
  if(name.StartsWith("Sword"))return "Меч";
  if(name.StartsWith("Axe"))return "Топор";
  if(name.StartsWith("Spear"))return "Копьё";
  if(name.StartsWith("Hammer"))return "Молот";
  return name;
 }
 void OnGUI()
 {
  if(chosen||!player)return;var titles=new string[Options.Count];var descriptions=new string[Options.Count];
  for(int i=0;i<Options.Count;i++)
  {
   var weapon=Options[i].GetComponent<SwordEvo>().newSword;var sword=weapon.GetComponent<Sword>();var hit=weapon.GetComponent<ImpactTrigger>();
   var evolution=weapon.GetComponent<WeaponEvolution>();titles[i]=evolution?evolution.title:WeaponTitle(weapon.name);descriptions[i]="Урон: "+hit.minDamage+"–"+hit.maxDamage+"\nСкорость: "+sword.maxSpeed+"\n"+(sword.twoHanded?"Двуручное оружие":"Одноручное оружие");
  }
  int index=CampaignMenu.Draw("КУЗНИЦА — ЭВОЛЮЦИЯ ОРУЖИЯ",titles,descriptions,choice.Selected,choice.Focus);if(index>=0)Select(index);
 }
}
