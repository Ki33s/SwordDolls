using UnityEngine;
public sealed class BreakableProp:MonoBehaviour
{
 public float hp=25;float nextHit;bool broken;
 void OnCollisionEnter(Collision collision){Hit(collision.collider);}
 void OnTriggerEnter(Collider other){Hit(other);}
 void OnTriggerStay(Collider other){Hit(other);}
 void Hit(Collider other)
 {
  if(broken||Time.time<nextHit)return;
  var sword=other.GetComponentInParent<Sword>();var ghost=other.GetComponentInParent<EchoBlade>();
  var bullet=other.GetComponent<BulletTrigger>();if(!sword&&!ghost&&!bullet)return;
  if(bullet){nextHit=Time.time+.2f;Damage(bullet.maxDamage,other.transform.position);Destroy(other.gameObject);return;}var hit=other.GetComponent<ImpactTrigger>();if(!hit&&sword)hit=sword.GetComponent<ImpactTrigger>();
  nextHit=Time.time+.2f;Damage(hit?Mathf.Max(5,hit.maxDamage):12,other.transform.position);
 }
 public void Damage(float amount,Vector3 source)
 {
  if(broken)return;hp-=amount;if(hp>0)return;broken=true;
  var renderers=GetComponentsInChildren<MeshRenderer>();if(renderers.Length==0){Destroy(gameObject);return;}
  Bounds bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
  for(int i=0;i<7;i++)
  {
   var piece=GameObject.CreatePrimitive(PrimitiveType.Cube);piece.name="Rubble";piece.transform.position=bounds.center+Random.insideUnitSphere*Mathf.Min(1,bounds.extents.magnitude);piece.transform.localScale=Vector3.one*Random.Range(.15f,.4f);
   piece.GetComponent<Renderer>().sharedMaterial=renderers[i%renderers.Length].sharedMaterial;var rb=piece.AddComponent<Rigidbody>();rb.mass=.4f;rb.AddExplosionForce(80,source,3,.7f);Destroy(piece,5);
  }
  CombatFx.Pulse(bounds.center,new Color(.8f,.6f,.3f),1);Destroy(gameObject);
 }
 public static void Setup(GameObject go)
 {
  if(!go.GetComponentInChildren<Collider>())foreach(var renderer in go.GetComponentsInChildren<MeshRenderer>())renderer.gameObject.AddComponent<BoxCollider>();
  var body=go.GetComponent<Rigidbody>();if(!body)body=go.AddComponent<Rigidbody>();body.isKinematic=true;body.useGravity=false;
  if(!go.GetComponent<BreakableProp>())go.AddComponent<BreakableProp>();
 }
}
