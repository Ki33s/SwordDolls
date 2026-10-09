using UnityEngine;
public sealed class BodyShield:MonoBehaviour
{
 public float Capacity {get;private set;} public float Current {get;private set;}
 public void Configure(float capacity,bool refill=false){float extra=Mathf.Max(0,capacity-Capacity);Capacity=Mathf.Max(0,capacity);Current=refill?Capacity:Mathf.Min(Capacity,Current+extra);}
 public void Refill(){Current=Capacity;}
 public float Absorb(float damage){float blocked=Mathf.Min(Current,damage);Current-=blocked;if(blocked>0)CombatFx.Pulse(transform.position+Vector3.up,Color.cyan,.8f);return damage-blocked;}
 public void Restore(float value){Current=Mathf.Min(Capacity,Current+value);}
}
