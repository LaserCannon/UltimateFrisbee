using UnityEngine;
using System.Collections;

public class Disc : MonoBehaviour
{
	
	public float MinVelocity = 3f;
	public float MaxVelocity = 15f;
	public float PixelToVelocityRatio = 150f;
	
	public Renderer DiscRenderer;
	
	public AudioClip LandSound;
	public AudioClip HitWallSound;
	
	
	private Vector3 velocity = Vector3.zero;
	
	private float fallSpeed = 0.5f;
	private float height = 0f;
	
	private float rotation = 0f;
	
	private int currentEndzone = 0;
	
	
	public int CurrentEndzone 
	{
		get { return currentEndzone; }
	}
	
	public Vector3 Velocity
	{
		get { return velocity; }
	}

	public float Height
	{
		get { return height; }
	}
	
	
	void Start ()
	{
	}
	
	
	public void Throw(Vector2 dirmag)
	{
		enabled = true;
		
		Vector3 vel = new Vector3(-dirmag.x, 0, -dirmag.y)/PixelToVelocityRatio;
		vel = vel.normalized * Mathf.Clamp(vel.magnitude,MinVelocity,MaxVelocity);
		velocity = vel;
		
		StopCoroutine("SpinDisc");
		StartCoroutine("SpinDisc");
		
		height = 1f;

		collider.enabled = false;

		Invoke ("EnableCollision", 0.15f);
	}
	
	public void Catch()
	{
		enabled = false;
		
		height = 1f;
		
		StopCoroutine("SpinDisc");
	}

	private void EnableCollision()
	{
		collider.enabled = true;
	}
	
	
	void Update()
	{
		float oldHeight = height;
		height = Mathf.MoveTowards(height,0f,fallSpeed*Time.deltaTime/2f);
		
		if(GameController.main.HasDisc!=null)
		{
			height = 1f;
		}
		
		if(height<=0f && oldHeight>0f)
		{
			audio.PlayOneShot(LandSound);
		}
			
		if(height>0)
		{
			velocity *= 1f - Time.deltaTime*0.1f;
		}
		else
		{
			velocity *= 1f - Time.deltaTime*1f;
		}
		
		if(height==0f)
		{
			GameController.main.SetDiscOutOfPlay();
			StopCoroutine("SpinDisc");
		}
		
		if(!rigidbody.isKinematic)
		{
			transform.position += Time.deltaTime * velocity;
		}
		
		Vector3 pos = transform.position;
		pos.y = height;
		transform.position = pos;
		
		//Check for endzones (colliders arent working)
		if(GameController.main.EndzoneA.bounds.Contains(transform.position))
		{
			currentEndzone = (int)Mathf.Sign(transform.position.z);
		}
		else if(GameController.main.EndzoneB.bounds.Contains(transform.position))
		{
			currentEndzone = (int)Mathf.Sign(transform.position.z);
		}
		else
		{
			currentEndzone = 0;
		}
			
	}

	
	
	IEnumerator SpinDisc()
	{
		while(true)
		{
			rotation = (rotation+90f)%360f;
			
			DiscRenderer.transform.rotation = Quaternion.identity;
			DiscRenderer.transform.Rotate(Vector3.up,rotation);
			
			float seconds = Mathf.Lerp(0.3f,0.05f,velocity.magnitude/MaxVelocity);
			yield return new WaitForSeconds(seconds);
		}
	}
	
	
	void OnCollisionEnter(Collision col)
	{
		if(col.collider.tag=="Wall")
		{
			audio.PlayOneShot(HitWallSound);
			
			velocity = Vector3.Reflect(velocity,col.contacts[0].normal)/2f;
		}
	}
	
	
	void OnTriggerEnter(Collider other)
	{
		if(other.tag=="Endzone")	
		{
			currentEndzone = (int)Mathf.Sign(other.transform.position.z);
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if(other.tag=="Endzone")	
		{
			currentEndzone = 0;
		}
	}
	
	
}
