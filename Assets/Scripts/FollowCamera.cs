using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{
	
	public Transform Target;
	
	
	public static FollowCamera main;
	
	
	void Awake()
	{
		main = this;
	}
	
	
	void LateUpdate ()
	{
		Vector3 pos = transform.position;
		pos.x = Target.position.x;
		pos.z = Target.position.z;
		
		
		transform.position = Vector3.Lerp(transform.position,pos,Time.deltaTime*10f);
	}
}
