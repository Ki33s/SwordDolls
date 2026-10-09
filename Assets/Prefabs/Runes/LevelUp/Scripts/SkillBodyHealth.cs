using UnityEngine;
public class SkillBodyHealth : MonoBehaviour
{
 public float healthPlusK=.1f;
 void Start()
 {
  var sword=GetComponent<AllSkill>().playerSword.GetComponent<Sword>();sword.bodyHealthBonus+=healthPlusK;
  if(sword.head)
  {
   var root=sword.head.transform.root;var bonuses=root.GetComponent<PlayerBodyBonuses>();if(!bonuses)bonuses=root.gameObject.AddComponent<PlayerBodyBonuses>();bonuses.Apply(sword.bodyHealthBonus);
  }
 }
}
