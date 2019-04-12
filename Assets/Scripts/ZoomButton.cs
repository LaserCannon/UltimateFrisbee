using UnityEngine;
using System.Collections;

public class ZoomButton : MonoBehaviour
{
	
	public float CamSizeIn = 15f;
	public float CamSizeOut = 30f;
	
	private bool zoomed = false;
	
	private float targetCamSize = 15f;
	
	
	
	void Awake()
	{
		targetCamSize = CamSizeIn;
	}
	
	void Update()
	{
		GetComponent<Renderer>().enabled = GameController.main.HasDisc!=null;
		
		if(Input.GetMouseButtonUp(0) && GameController.main.HasDisc!=null)
		{
			RaycastHit hit = new RaycastHit();
			if(Physics.Raycast(FollowCamera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition),out hit))
			{
				if(hit.collider==GetComponent<Collider>())
				{
					Toggle();
				}
			}
		}
		
		
		FollowCamera.main.GetComponent<Camera>().orthographicSize = Mathf.Lerp(FollowCamera.main.GetComponent<Camera>().orthographicSize,targetCamSize,Time.deltaTime*10f);
	
	}
	
	
	public void Toggle()
	{
		zoomed = !zoomed;
		
		targetCamSize = zoomed ? CamSizeOut : CamSizeIn;
	}
	
	public void ZoomOut()
	{
		zoomed = true;
		targetCamSize = CamSizeOut;
	}
	
	public void ZoomIn()
	{
		zoomed = false;
		targetCamSize = CamSizeIn;
	}
	
	
}
