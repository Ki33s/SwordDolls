using System.Collections.Generic;
using UnityEngine;
public sealed class EchoBlade:MonoBehaviour
{
 public HeroCombat owner;public int index;public bool orbit;readonly Dictionary<MainHealth,float> hits=new Dictionary<MainHealth,float>();Material tint;
 public void Initialize(HeroCombat source,int number,bool rotating)
 {
  owner=source;index=number;orbit=rotating;
  var model=source.GetComponent<Sword>().mainModel;foreach(var filter in (model?model.GetComponentsInChildren<MeshFilter>():source.GetComponentsInChildren<MeshFilter>()))
  {
   if(!filter.sharedMesh)continue;var visual=new GameObject("Echo mesh",typeof(MeshFilter),typeof(MeshRenderer));visual.transform.SetParent(transform,false);
   visual.transform.localPosition=source.transform.InverseTransformPoint(filter.transform.position);visual.transform.localRotation=Quaternion.Inverse(source.transform.rotation)*filter.transform.rotation;visual.transform.localScale=filter.transform.lossyScale;
   visual.GetComponent<MeshFilter>().sharedMesh=filter.sharedMesh;var original=filter.GetComponent<Renderer>();if(original){if(!tint){tint=new Material(original.sharedMaterial);if(tint.HasProperty("_BaseColor"))tint.SetColor("_BaseColor",Color.Lerp(tint.GetColor("_BaseColor"),Color.cyan,.45f));else if(tint.HasProperty("_Color"))tint.SetColor("_Color",Color.Lerp(tint.GetColor("_Color"),Color.cyan,.45f));}visual.GetComponent<Renderer>().sharedMaterial=tint;}
  }
  var collider=gameObject.AddComponent<CapsuleCollider>();collider.radius=.2f;collider.height=1.5f;collider.center=Vector3.up*.5f;collider.isTrigger=true;var rb=gameObject.AddComponent<Rigidbody>();rb.isKinematic=true;rb.useGravity=false;
  CombatFx.Pulse(source.transform.position,Color.cyan,1);
 }
 void FixedUpdate()
 {
  if(!owner){Destroy(gameObject);return;}
  float angle=Time.time*110+index*180;Vector3 offset=orbit?Quaternion.Euler(0,angle,0)*Vector3.right*1.4f:Vector3.right*(index%2==0?1:-1)*(.9f+index/2*.5f);
  transform.position=owner.transform.position+offset;transform.rotation=owner.transform.rotation;
 }
 void OnTriggerStay(Collider other)
 {
  if(!owner||!owner.GetComponent<Sword>().hasDoll)return;
  var hp=other.transform.root.GetComponent<MainHealth>();var enemy=other.transform.root.GetComponent<CampaignEnemy>();if(!hp||!enemy||enemy.Captured)return;
  var id=hp;if(hits.TryGetValue(id,out float next)&&Time.time<next)return;hits[id]=Time.time+.35f;
  var weapon=owner.GetComponent<ImpactTrigger>();hp.Damage(Mathf.Max(1,weapon.maxDamage*owner.echoDamage),other.attachedRigidbody,300,(other.transform.position-transform.position).normalized);
  var part=other.GetComponent<Health>();if(part)owner.OnHit(part,weapon.maxDamage*owner.echoDamage);CombatFx.Pulse(other.transform.position,Color.cyan,.5f);
 }
 void OnDestroy(){if(tint)Destroy(tint);}
}
