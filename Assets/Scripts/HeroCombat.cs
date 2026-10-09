using System.Collections.Generic;
using UnityEngine;
public sealed class HeroCombat:MonoBehaviour
{
 public float shieldBonus,lifeSteal,poison,frost,execute,magnet=5,shieldRegen,thorns,chainChance,killBlast,pulseDamage;
 public int echoes,orbitBlades;public float echoDamage=.4f;
 float regenTimer,pulseTimer;readonly List<EchoBlade> copies=new List<EchoBlade>();bool exploding;
 public MainHealth BodyHealth {get{var sword=GetComponent<Sword>();return sword&&sword.head?sword.head.transform.root.GetComponent<MainHealth>():null;}}
 public void RefreshShield(){var sword=GetComponent<Sword>();var hp=BodyHealth;if(!hp)return;var bonuses=hp.GetComponent<PlayerBodyBonuses>();if(!bonuses)bonuses=hp.gameObject.AddComponent<PlayerBodyBonuses>();bonuses.Apply(sword.bodyHealthBonus);}
 void Update()
 {
  if(CampaignMenu.OpenCount>0||Time.timeScale==0)return;
  copies.RemoveAll(x=>!x);
  while(copies.Count<echoes+orbitBlades){var go=new GameObject("Echo weapon");var copy=go.AddComponent<EchoBlade>();copy.Initialize(this,copies.Count,copies.Count>=echoes);copies.Add(copy);}
  var hp=BodyHealth;if(!hp)return;
  regenTimer-=Time.deltaTime;if(regenTimer<=0){RefreshShield();var shield=hp.GetComponent<BodyShield>();if(shield)shield.Restore(shieldRegen);regenTimer=1;}
  pulseTimer-=Time.deltaTime;if(pulseDamage>0&&pulseTimer<=0){Burst(transform.position,pulseDamage,4,Color.cyan);pulseTimer=5;}
 }
 public static void Hit(ImpactTrigger source,Health part,float damage)
 {
  var player=GameObject.FindGameObjectWithTag("Player");if(!player||player.GetComponent<ImpactTrigger>().dollId!=source.dollId)return;
  var combat=player.GetComponent<HeroCombat>();if(combat)combat.OnHit(part,damage);
 }
 public void OnHit(Health part,float damage)
 {
  if(!part)return;var hp=part.transform.root.GetComponent<MainHealth>();var target=part.transform.root.GetComponent<CampaignEnemy>();if(!hp||!target||target.Captured)return;
  var own=BodyHealth;if(own&&lifeSteal>0)own.Heal(damage*lifeSteal/own.maxHealth);
  if(poison>0||frost>0){var status=hp.GetComponent<CombatStatus>();if(!status)status=hp.gameObject.AddComponent<CombatStatus>();status.Apply(poison,frost);}
  if(execute>0&&hp.health>0&&hp.health<hp.maxHealth*.2f)hp.Damage(damage*execute,part.GetComponent<Rigidbody>(),0,Vector3.zero);
  if(chainChance>0&&Random.value<chainChance)
   foreach(var enemy in FindObjectsByType<CampaignEnemy>(FindObjectsSortMode.None)){var other=enemy.GetComponent<MainHealth>();if(other&&enemy!=target&&!enemy.Captured&&Vector3.Distance(enemy.transform.position,target.transform.position)<4){other.Damage(damage*.5f,enemy.GetComponent<Body>().head.GetComponent<Rigidbody>(),200,Vector3.zero);CombatFx.Pulse(enemy.transform.position+Vector3.up,Color.cyan,1);break;}}
 }
 public void OnKilled(Vector3 point){if(killBlast>0&&!exploding){exploding=true;Burst(point,killBlast,3.5f,new Color(1,.4f,.1f));exploding=false;}}
 void Burst(Vector3 point,float damage,float radius,Color color)
 {
  CombatFx.Pulse(point,color,radius);
  foreach(var actor in FindObjectsByType<CampaignEnemy>(FindObjectsSortMode.None)){var hp=actor.GetComponent<MainHealth>();if(hp&&hp.health>0&&!actor.Captured&&Vector3.Distance(point,actor.transform.position)<radius)hp.Damage(damage,actor.GetComponent<Body>().head.GetComponent<Rigidbody>(),300,Vector3.zero);}
  foreach(var prop in FindObjectsByType<BreakableProp>(FindObjectsSortMode.None))if(Vector3.Distance(point,prop.transform.position)<radius)prop.Damage(damage,point);
 }
}
