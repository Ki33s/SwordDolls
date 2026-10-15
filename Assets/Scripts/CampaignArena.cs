using UnityEngine;
public sealed class CampaignArena:MonoBehaviour
{
 public GameObject[] props=new GameObject[0];public float Radius=>GetComponent<City>().tilesNum*GetComponent<City>().tilesLength*.5f-2;
 void Start()
 {
  for(int i=0;i<props.Length;i++){float angle=i*Mathf.PI*2/Mathf.Max(1,props.Length);var point=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*(Radius-.5f);var prop=Instantiate(props[i],point,Quaternion.Euler(0,-angle*Mathf.Rad2Deg,0),transform);BreakableProp.Setup(prop);}
  var city=GetComponent<City>();if(city.houses.Length>0){var house=Instantiate(city.houses[0],new Vector3(-Radius+2,0,Radius-2),Quaternion.identity,transform);house.transform.localScale*=.45f;BreakableProp.Setup(house);house.GetComponent<BreakableProp>().hp=100;}
  Invoke(nameof(PrepareScenery),.5f);Invoke(nameof(CenterPlayer),.1f);
 }
 void CenterPlayer()
 {
  var player=GameObject.FindGameObjectWithTag("Player");if(!player)return;
  var sword=player.GetComponent<Sword>();var root=sword&&sword.head?sword.head.transform.root:player.transform;
  Vector3 offset=-Vector3.ProjectOnPlane(root.position,Vector3.up);root.position+=offset;
  if(player.transform.root!=root)player.transform.position+=offset;
 }
 void PrepareScenery()
 {
  foreach(Transform child in transform)
  {
   if(child.GetComponent<Crystall>()||child.GetComponent<Anvill>()||child.GetComponent<City>()||child.GetComponent<TerrainVisuals>())continue;
   var renderers=child.GetComponentsInChildren<MeshRenderer>();if(renderers.Length==0)continue;
   var bounds=renderers[0].bounds;foreach(var renderer in renderers)bounds.Encapsulate(renderer.bounds);
   if(bounds.size.y>.4f)BreakableProp.Setup(child.gameObject);
  }
 }
 public bool Inside(Vector3 point)=>Mathf.Abs(point.x)<=Radius&&Mathf.Abs(point.z)<=Radius;
 public Vector3 FreePoint(int seed)
 {
  for(int i=0;i<80;i++)
  {
   float angle=(seed*137.5f+i*43)*Mathf.Deg2Rad;float radius=5+(seed+i)%5;
   Vector3 point=new Vector3(Mathf.Cos(angle),0,Mathf.Sin(angle))*Mathf.Min(radius,Radius-3);point.y=.15f;
   if(!Physics.CheckSphere(point+Vector3.up*1.1f,.5f,~0,QueryTriggerInteraction.Ignore))return point;
  }
  return new Vector3(Mathf.Clamp(seed%5-2,-Radius+3,Radius-3),.15f,3);
 }
 void FixedUpdate()
 {
  var city=GetComponent<City>();foreach(var actor in city.allEnemies)if(actor&&actor.GetComponent<Body>())KeepInside(actor.transform,actor.GetComponent<Body>().head.transform.position);
  var player=GameObject.FindGameObjectWithTag("Player");if(player){var sword=player.GetComponent<Sword>();var root=sword&&sword.head?sword.head.transform.root:player.transform;if(sword&&sword.head)KeepInside(root,sword.head.transform.position);if(player.transform.root!=root)KeepInside(player.transform,player.transform.position);else KeepInside(root,player.transform.position);}
 }
 void KeepInside(Transform root,Vector3 reference)
 {
  Vector3 corrected=reference;corrected.x=Mathf.Clamp(reference.x,-Radius,Radius);corrected.z=Mathf.Clamp(reference.z,-Radius,Radius);if(reference.y<-.5f)corrected.y=1.5f;
  Vector3 delta=corrected-reference;if(delta.sqrMagnitude<.0001f)return;
  root.position+=delta;foreach(var rb in root.GetComponentsInChildren<Rigidbody>())rb.linearVelocity=Vector3.zero;
 }
}
