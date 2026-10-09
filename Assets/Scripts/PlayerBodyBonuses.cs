using UnityEngine;
public sealed class PlayerBodyBonuses:MonoBehaviour
{
 public void Apply(float total)
 {
  var health=GetComponent<MainHealth>();if(!health)return;
  var body=GetComponent<Body>();var sword=body&&body.sword?body.sword.GetComponent<Sword>():null;
  if((!sword||!sword.player)&&body&&body.head){var player=GameObject.FindGameObjectWithTag("Player");if(player)sword=player.GetComponent<Sword>();}
  var combat=sword?sword.GetComponent<HeroCombat>():null;
  var shield=GetComponent<BodyShield>();if(!shield)shield=gameObject.AddComponent<BodyShield>();
  shield.Configure(health.maxHealth*(total+(combat?combat.shieldBonus:0)));
 }
}
