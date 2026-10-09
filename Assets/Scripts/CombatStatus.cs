using UnityEngine;
public sealed class CombatStatus:MonoBehaviour
{
 float poison,frost,remaining,tick;public float MoveMultiplier=>remaining>0&&frost>0?.6f:1;
 public void Apply(float damage,float slow){poison=Mathf.Max(poison,damage);frost=Mathf.Max(frost,slow);remaining=4;CombatFx.Pulse(transform.position+Vector3.up,slow>0?Color.cyan:Color.green,1);}
 void Update()
 {
  if(remaining<=0){Destroy(this);return;}remaining-=Time.deltaTime;tick-=Time.deltaTime;
  if(tick<=0&&poison>0){var body=GetComponent<Body>();var hp=GetComponent<MainHealth>();if(hp&&body&&hp.health>0)hp.Damage(poison,body.head.GetComponent<Rigidbody>(),0,Vector3.zero);tick=1;}
 }
}
