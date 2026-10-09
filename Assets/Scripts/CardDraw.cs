using System.Collections.Generic;
using UnityEngine;
public static class CardDraw
{
 public static string Family(GameObject card)
 {
  var skill=card.GetComponent<LootRune>().useRune;if(!skill)return card.name;
  var custom=skill.GetComponent<CampaignSkill>();if(custom)return custom.effect==CampaignSkill.Effect.Recovery?"Shield":custom.effect.ToString();
  if(skill.GetComponent<SkillDamage>())return "Damage";
  if(skill.GetComponent<SkillCrit>())return "Critical";
  if(skill.GetComponent<SkillSwordSize>())return "Size";
  if(skill.GetComponent<SkillSwordSpeed>())return "Speed";
  if(skill.GetComponent<SkillSwordImpactForce>())return "Force";
  if(skill.GetComponent<SkillBodyHealth>())return "Shield";
  return skill.name;
 }
 public static List<GameObject> Draw(List<GameObject> deck,int count)
 {
  var available=new List<GameObject>();var skills=new HashSet<GameObject>();
  foreach(var card in deck)if(card&&card.GetComponent<LootRune>()&&skills.Add(card.GetComponent<LootRune>().useRune))available.Add(card);
  var result=new List<GameObject>();
  while(result.Count<count&&available.Count>0)
  {
   int total=0;foreach(var c in available)total+=Mathf.Max(1,c.GetComponent<LootRune>().runeType);
   int roll=Random.Range(0,total),index=0;
   for(;index<available.Count-1;index++){roll-=Mathf.Max(1,available[index].GetComponent<LootRune>().runeType);if(roll<0)break;}
   var chosen=available[index];result.Add(chosen);string family=Family(chosen);available.RemoveAll(x=>Family(x)==family);
  }
  return result;
 }
}
