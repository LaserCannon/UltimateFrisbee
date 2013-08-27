using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
	
	
	public delegate void SwipeDelegate(Vector2 dirmag);
	public event SwipeDelegate OnSwipe = null;
	
	public delegate void PositionDelegate(Vector2 position);
	public event PositionDelegate OnTouchStart = null;
	public event PositionDelegate OnTouchUpdate = null;
	public event PositionDelegate OnTouchEnd = null;
	
	
	
	public float SwipeMinDist = 0.1f;
	public float SwipeMaxDist = 0.2f;
	public float SwipeMaxTime = 0.5f;
	
	public float SwipeStartDist = 0.05f;
	
	
	
	private Vector2 touchStart = Vector2.zero;
	private float timeStart = 0f;
	
	private bool isSwipePending = false;
	
	
	public Vector2 TouchStart
	{
		get { return touchStart; }
	}
	
	
	public static PlayerInput main;
	
	void Awake()
	{
		main = this;
	}
	
	
	void Update()
	{
		if(Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
		{
			Vector2 touchPos = Input.mousePosition;
			
			if(Input.GetMouseButtonDown(0))
			{
				touchStart = touchPos;
				timeStart = Time.time;
				
				isSwipePending = true;
				
				if(OnTouchStart!=null)
					OnTouchStart(touchPos);
			}
			else if(isSwipePending)
			{
				Vector2 difference = touchStart-touchPos;
				float timeDifference = Time.time-timeStart;
				float distRatio = difference.magnitude / Screen.height;
				
				if(difference.magnitude<Screen.height*SwipeStartDist)
				{
					timeStart = Time.time;
				}
				
				if(OnTouchUpdate!=null)
					OnTouchUpdate(touchPos);
				
				if(Input.GetMouseButtonUp(0))
				{
					if(OnTouchEnd!=null)
						OnTouchEnd(touchPos);
					
					if(distRatio >= SwipeMinDist && timeDifference <= SwipeMaxTime)
					{
						if(OnSwipe!=null)
						{
							OnSwipe(difference / timeDifference);
						}
					}
					
					touchStart = Vector2.zero;
					isSwipePending = false;
				}
				else
				{
					if(distRatio > SwipeMaxDist)
					{
						if(timeDifference <= SwipeMaxTime)
						{
							if(OnSwipe!=null)
							{
								OnSwipe(difference / timeDifference);
							}
						}
						
						isSwipePending = false;
					}
				}
			}
			else
			{
				if(Input.GetMouseButtonUp(0))
				{
					if(OnTouchEnd!=null)
						OnTouchEnd(touchPos);
					
					touchStart = Vector2.zero;
					isSwipePending = false;
				}
				else
				{
					if(OnTouchUpdate!=null)
						OnTouchUpdate(touchPos);
				}
			}
		}
	}
	
}
