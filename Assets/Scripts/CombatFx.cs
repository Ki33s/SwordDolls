using UnityEngine;
public static class CombatFx
{
 public static void Pulse(Vector3 position,Color color,float radius)
 {
  var global=GameObject.Find("GlobalParameters");var prefab=global?global.GetComponent<GlobalParameters>().fireball:null;
  var go=new GameObject("Combat pulse");go.transform.position=position;
  var ps=go.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);var main=ps.main;main.loop=false;main.duration=.3f;main.startLifetime=.4f;main.startSpeed=radius*3;main.startColor=color;main.startSize=.14f;
  var emission=ps.emission;emission.rateOverTime=0;emission.SetBursts(new[]{new ParticleSystem.Burst(0,28)});
  if(prefab){var source=prefab.GetComponentInChildren<ParticleSystemRenderer>();if(source)ps.GetComponent<ParticleSystemRenderer>().sharedMaterial=source.sharedMaterial;}
  ps.Play();Object.Destroy(go,1);
 }
}
