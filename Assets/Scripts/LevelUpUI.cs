using System.Collections.Generic;
using UnityEngine;
public class LevelUpUI : MonoBehaviour
{
 // Retain original serialized fields for existing scene/prefab references.
 public List<GameObject> allRunes=new List<GameObject>(),runes=new List<GameObject>();public List<int> runesId=new List<int>();
 public GameObject swordUI,leftRunePos,centerRunePos,rightRunePos,leftRunePickPos,centerPickRunePos,rightPickRunePos,leftRune,centerRune,rightRune;
 public int swordSpeed=750,takeRuneMagnitude=10;
 public List<GameObject> Options { get; private set; }
 public int Selected=>choice.Selected;
 AxisChoice choice=new AxisChoice();GameObject playerSword;bool finished;float unlockAt;public bool CanChoose=>Time.unscaledTime>=unlockAt;
 void Awake(){unlockAt=Time.unscaledTime+1f;CampaignMenu.Open();var group=gameObject.AddComponent<CanvasGroup>();group.alpha=0;group.blocksRaycasts=false;}
 void Start()
 {
  playerSword=GameObject.FindGameObjectWithTag("Player");
  allRunes=GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>().runes;
  Options=CardDraw.Draw(allRunes,3);
  if(Options.Count==0){Finish();return;}
 }
 void Update(){if(Options==null||finished||!CanChoose)return;int selected=choice.Step(SwordInput.Read(),Options.Count);if(selected>=0)Select(selected);}
 public void Select(int index)
 {
  if(!CanChoose||finished||Options==null||index<0||index>=Options.Count)return;finished=true;
  var rune=Options[index].GetComponent<LootRune>();var global=GameObject.Find("GlobalParameters").GetComponent<GlobalParameters>();
  global.accuredRunes.Add(rune.useRune);
  var skill=Instantiate(rune.useRune);var all=skill.GetComponent<AllSkill>();all.playerSword=playerSword;all.runeId=global.runes.IndexOf(Options[index]);all.runeAdded=rune.runeAdded;
  Debug.Log("CAMPAIGN_CARD "+Options[index].name);Finish();
 }
 void Finish(){playerSword.GetComponent<Sword>().FinshLevelUp();Destroy(gameObject);}
 void OnDestroy(){CampaignMenu.Close();}
 void OnGUI()
 {
  if(Options==null||finished)return;
  var titles=new string[Options.Count];var descriptions=new string[Options.Count];
  for(int i=0;i<Options.Count;i++)
  {
   var rune=Options[i].GetComponent<LootRune>();var custom=rune.useRune.GetComponent<CampaignSkill>();
   titles[i]=custom?custom.title:Title(rune.useRune);
   descriptions[i]=custom?custom.description:Describe(rune.useRune);
  }
  int selected=CampaignMenu.Draw("УРОВЕНЬ ГЕРОЯ — ВЫБЕРИ УЛУЧШЕНИЕ",titles,descriptions,choice.Selected,choice.Focus,CanChoose);if(selected>=0)Select(selected);
 }
 static string Title(GameObject skill)
 {
  if(skill.GetComponent<SkillDamage>())return "Заточка оружия";
  if(skill.GetComponent<SkillCrit>())return "Критический удар";
  if(skill.GetComponent<SkillSwordSize>())return "Длинное лезвие";
  if(skill.GetComponent<SkillSwordSpeed>())return "Скорость оружия";
  if(skill.GetComponent<SkillSwordImpactForce>())return "Сила удара";
  if(skill.GetComponent<SkillBodyHealth>())return "Бонусный щит";
  return "Усиление способности";
 }
 static string Describe(GameObject skill)
 {
  if(skill.GetComponent<SkillDamage>())return "Повышает урон оружия.";
  if(skill.GetComponent<SkillCrit>())return "Повышает вероятность критического удара.";
  if(skill.GetComponent<SkillSwordSize>())return "Увеличивает размер и дальность оружия.";
  if(skill.GetComponent<SkillSwordSpeed>())return "Ускоряет движение оружия.";
  if(skill.GetComponent<SkillSwordImpactForce>())return "Усиливает отбрасывание при ударе.";
  if(skill.GetComponent<SkillBodyHealth>())return "Даёт новому телу дополнительный запас щита.";
  return "Усиление выбранной способности.";
 }
}
