using UnityEngine;
public sealed class CampaignSkill : MonoBehaviour
{
 public enum Effect { Damage, Speed, Size, Force, Critical, Fireball, Clone, Slash, Pierce, Recovery, Shield, Echo, Orbit, LifeSteal, Poison, Frost, Execute, Magnet, ShieldRegen, Chain, KillBlast, Pulse }
 public Effect effect;
 public string title, description;
 public float amount=.2f;
 void Start()
 {
  var all=GetComponent<AllSkill>(); var hero=all.playerSword.GetComponent<SwordHero>(); var sword=hero.GetComponent<Sword>();
  var combat=hero.GetComponent<HeroCombat>();if(!combat)combat=hero.gameObject.AddComponent<HeroCombat>();
  foreach(var hit in hero.GetComponentsInChildren<ImpactTrigger>(true))
  {
   switch(effect)
   {
    case Effect.Damage:hit.minDamage+=Mathf.RoundToInt(hero.minDamage*amount);hit.maxDamage+=Mathf.RoundToInt(hero.maxDamage*amount);break;
    case Effect.Critical:hit.critChance=Mathf.Clamp(hit.critChance+amount,0,.8f);break;
    case Effect.Force:break;
    case Effect.Clone:hit.duplicateCloneChance=Mathf.Clamp(hit.duplicateCloneChance+amount,0,.65f);break;
    case Effect.Pierce:hit.prickChance=Mathf.Clamp(hit.prickChance+amount,0,.65f);break;
   }
  }
  switch(effect)
  {
   case Effect.Speed:sword.forceMove=Mathf.RoundToInt(sword.forceMove*(1+amount));sword.maxSpeed*=1+amount;sword.RefreshMovementPower();break;
   case Effect.Force:foreach(var impact in hero.GetComponentsInChildren<Impact>(true))impact.forceBack=Mathf.RoundToInt(impact.forceBack*(1+amount));break;
   case Effect.Size:hero.transform.localScale*=1+amount;break;
   case Effect.Fireball:sword.fireballShot=true;sword.fireballLearned=true;sword.fireballShotCouldown=3;break;
   case Effect.Clone:sword.duplicateLearned=true;break;
   case Effect.Slash:sword.slashShot=true;sword.slashLearned=true;sword.slashCouldown=3;break;
   case Effect.Pierce:sword.prickShot=true;sword.prickCouldown=3;break;
   case Effect.Recovery:combat.shieldBonus+=amount;combat.RefreshShield();break;
   case Effect.Shield:combat.shieldBonus+=amount;combat.RefreshShield();break;
   case Effect.Echo:combat.echoes+=Mathf.RoundToInt(amount);break;
   case Effect.Orbit:combat.orbitBlades+=Mathf.RoundToInt(amount);break;
   case Effect.LifeSteal:combat.lifeSteal+=amount;break;
   case Effect.Poison:combat.poison+=amount;break;
   case Effect.Frost:combat.frost+=amount;break;
   case Effect.Execute:combat.execute+=amount;break;
   case Effect.Magnet:combat.magnet+=amount;break;
   case Effect.ShieldRegen:combat.shieldRegen+=amount;break;
   case Effect.Chain:combat.chainChance+=amount;break;
   case Effect.KillBlast:combat.killBlast+=amount;break;
   case Effect.Pulse:combat.pulseDamage+=amount;break;
  }
  Destroy(gameObject,1);
 }
}
