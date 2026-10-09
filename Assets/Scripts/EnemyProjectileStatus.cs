using UnityEngine;
public sealed class EnemyProjectileStatus:MonoBehaviour
{
 public float poison,frost;
 void OnTriggerEnter(Collider other)
 {
  var hp=other.transform.root.GetComponent<MainHealth>();var body=other.transform.root.GetComponent<Body>();if(!hp||!body||!body.sword||!body.sword.GetComponent<Sword>().player)return;
  var status=hp.GetComponent<CombatStatus>();if(!status)status=hp.gameObject.AddComponent<CombatStatus>();status.Apply(poison,frost);
 }
}
