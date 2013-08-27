using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour
{
	
	
	public GameObject TitleObject;
	
	public Renderer PressStartObject;
	
	
	void Start()
	{
		StartCoroutine(_FlashPressStart());
	}
	
	
	void Update()
	{
		if(Input.GetMouseButtonDown(0) && flashInterval>0.1f)
		{
			flashInterval = 0.05f;
			StartCoroutine(_FadeOutAndLoadLevel());
		}
	}
	
	
	
	private float flashInterval = 0.25f;
	
	IEnumerator _FlashPressStart()
	{
		while(true)
		{
			PressStartObject.enabled = !PressStartObject.enabled;
			
			yield return new WaitForSeconds(flashInterval);
		}
	}
	
	IEnumerator _FadeOutAndLoadLevel()
	{
		yield return new WaitForSeconds(1f);
		
		Application.LoadLevel("Game");
	}
		
	
	
}
