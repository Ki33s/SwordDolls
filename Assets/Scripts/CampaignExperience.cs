using UnityEngine;
public sealed class CampaignExperience : MonoBehaviour
{
 public int amount=3;float life=20;
 public static void Spawn(Vector3 position,int amount)
 {
  var orb=GameObject.CreatePrimitive(PrimitiveType.Sphere);orb.name="Опыт +"+amount;orb.transform.position=position+Vector3.up*.2f;orb.transform.localScale=Vector3.one*.2f;
  orb.GetComponent<Collider>().isTrigger=true;
  var body=orb.AddComponent<Rigidbody>();body.useGravity=false;body.mass=.1f;
  var renderer=orb.GetComponent<Renderer>();renderer.material=new Material(Shader.Find("Universal Render Pipeline/Lit"));renderer.material.color=new Color(.15f,.8f,1);
  orb.AddComponent<CampaignExperience>().amount=amount;
 }
 void FixedUpdate()
 {
  life-=Time.fixedDeltaTime;if(life<=0){Destroy(gameObject);return;}
  var hero=GameObject.FindGameObjectWithTag("Player");if(!hero)return;
  Vector3 delta=hero.transform.position-transform.position;
  if(delta.magnitude<.7f){hero.GetComponent<SwordHero>().ExpGain(amount);Destroy(gameObject);return;}
  if(delta.magnitude<(hero.GetComponent<HeroCombat>()?hero.GetComponent<HeroCombat>().magnet:5))GetComponent<Rigidbody>().linearVelocity=delta.normalized*5;
 }
 void OnDestroy(){var renderer=GetComponent<Renderer>();if(renderer&&renderer.sharedMaterial)Destroy(renderer.sharedMaterial);}
}
