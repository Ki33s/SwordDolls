using UnityEngine;
using UnityEngine.UI;
public class Crystall:MonoBehaviour
{
 public ParticleSystem shield;public CapsuleCollider shieldCollider;public GameObject destroyEffect;public Image arrow;
 Image nowArrow;bool ready,destroyed;public bool Ready=>ready;
 void FixedUpdate()
 {
  if(!ready||destroyed)return;var player=GameObject.FindGameObjectWithTag("Player");if(!player)return;
  if(nowArrow&&Camera.main){var from=Camera.main.WorldToScreenPoint(player.transform.position);var to=Camera.main.WorldToScreenPoint(transform.position);nowArrow.transform.position=new Vector3(Mathf.Clamp(from.x,40,Screen.width-40),Mathf.Clamp(from.y,40,Screen.height-40));nowArrow.transform.rotation=Quaternion.Euler(0,0,Mathf.Atan2(to.y-from.y,to.x-from.x)*Mathf.Rad2Deg);}
  if(Vector3.ProjectOnPlane(player.transform.position-transform.position,Vector3.up).magnitude<2)Complete();
 }
 public void CrystalReady()
 {
  if(ready)return;ready=true;if(shield)shield.loop=false;if(shieldCollider)Destroy(shieldCollider);
  var canvas=GameObject.Find("CanvasExp");if(arrow&&canvas){nowArrow=Instantiate(arrow,canvas.transform,false);}
  CombatFx.Pulse(transform.position+Vector3.up,Color.cyan,2);
 }
 void OnTriggerEnter(Collider other){var sword=other.GetComponentInParent<Sword>();if(ready&&sword&&sword.player)Complete();}
 void Complete()
 {
  if(destroyed)return;destroyed=true;if(destroyEffect)Destroy(Instantiate(destroyEffect,transform.position+Vector3.up,Quaternion.identity),4);
  var city=GetComponentInParent<City>();if(city)city.CrystallDestroyed();if(nowArrow)Destroy(nowArrow.gameObject);Destroy(gameObject);
 }
 void OnDestroy(){if(nowArrow)Destroy(nowArrow.gameObject);}
}
