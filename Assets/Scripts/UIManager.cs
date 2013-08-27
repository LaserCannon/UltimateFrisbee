using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class UIManager : MonoBehaviour
{
	
	public static UIManager main;
	
	
	public Renderer ArrowR;
	public Renderer ArrowL;
	
	public TextMesh ScoreText;
	
	public TextMesh TitleText;
	
	public Renderer PlayerPointerPrefab;
	public Renderer AIPointerPrefab;
	
	public ZoomButton Zoom;
	
	
	private List<Renderer> playerPointers = new List<Renderer>();
	private List<Renderer> aiPointers = new List<Renderer>();
	
	
	private List<Player> playerTargets = new List<Player>();
	private List<Player> aiTargets = new List<Player>();
	
	
	void Awake()
	{
		main = this;
		
		ArrowL.enabled = false;
		ArrowR.enabled = false;
	}
	
	void Start()
	{
		for(int i=0;i<7;i++)
		{
			Renderer r = (Renderer)Instantiate(PlayerPointerPrefab);
			r.enabled = false;
			r.transform.parent = FollowCamera.main.transform;
			playerPointers.Add(r);
		}
		for(int i=0;i<7;i++)
		{
			Renderer r = (Renderer)Instantiate(AIPointerPrefab);
			r.transform.parent = FollowCamera.main.transform;
			r.enabled = false;
			aiPointers.Add(r);
		}
	}
	
	void Update()
	{
		if(playerTargets!=null && playerTargets.Count>0)
		{
			for(int i=0;i<7;i++)
			{
				PointArrowToPlayer(playerPointers[i],playerTargets[i]);
			}
			for(int i=0;i<7;i++)
			{
				PointArrowToPlayer(aiPointers[i],aiTargets[i]);
			}
		}
		
		transform.localScale = new Vector3(FollowCamera.main.camera.orthographicSize/15f,
											FollowCamera.main.camera.orthographicSize/15f,
											FollowCamera.main.camera.orthographicSize/15f);
	}
	
	private void PointArrowToPlayer(Renderer arrow, Player player)
	{
		Vector3 centerToPlayer = player.transform.position-FollowCamera.main.transform.position;
		centerToPlayer.y = 0f;
		
		float xw = FollowCamera.main.camera.orthographicSize * FollowCamera.main.camera.aspect;
		float yw = FollowCamera.main.camera.orthographicSize;
		
		if(	Mathf.Abs(centerToPlayer.z) > yw ||
			Mathf.Abs(centerToPlayer.x) > xw)
		{
			arrow.enabled = true;
			
			float yoff = (centerToPlayer.z/centerToPlayer.x) * xw * Mathf.Sign(centerToPlayer.x);
			float xoff = xw * Mathf.Sign(centerToPlayer.x);
			
			if(Mathf.Abs (yoff) > yw)
			{
				xoff = (centerToPlayer.x/centerToPlayer.z) * yw * Mathf.Sign(centerToPlayer.z);
				yoff = yw * Mathf.Sign(centerToPlayer.z);
			}
			
			Vector3 pos = Vector3.MoveTowards(new Vector3(xoff,0f,yoff), Vector3.zero, 1f);
			
			
			arrow.transform.position = arrow.transform.parent.position + pos - Vector3.up*5f;
			arrow.transform.rotation = Quaternion.LookRotation(new Vector3(centerToPlayer.x,0,centerToPlayer.z));
		}
		else
		{
			arrow.enabled = false;
		}
	}
	
	
	public void SetPlayerTargets(List<Player> players, List<Player> ai)
	{
		playerTargets = players;
		aiTargets = ai;
	}
	
	
	public void UpdateScore(int playerScore, int opponentScore)
	{
		ScoreText.text = "P1: " + playerScore + "  -  AI: " + opponentScore;
	}
	
	public void BlinkArrows(bool up)
	{
		ArrowL.transform.localPosition = new Vector3(-6,-1,up?12:-12);
		ArrowR.transform.localPosition = new Vector3(6,-1,up?12:-12);
		
		ArrowL.transform.rotation = Quaternion.LookRotation(up?-Vector3.forward:Vector3.forward);
		ArrowR.transform.rotation = Quaternion.LookRotation(up?-Vector3.forward:Vector3.forward);
		
		StartCoroutine(_BlinkArrows());
	}
	
	private int _blinkid = 0;
	private IEnumerator _BlinkArrows()
	{
		int blinkid = ++_blinkid;
		
		//float startTime = Time.time;
		
		ArrowL.enabled = false;
		ArrowR.enabled = false;
		
		yield return null;
		
		while(GameController.main.HasDisc!=null /*Time.time-startTime<2f*/ && _blinkid==blinkid)
		{
			ArrowL.enabled = !ArrowL.enabled;
			ArrowR.enabled = !ArrowR.enabled;
			
			yield return new WaitForSeconds(0.255f);
		}
		
		if(_blinkid==blinkid)
		{
			ArrowL.enabled = false;
			ArrowR.enabled = false;
		}
	}
	
	
	public void DoTitleText(string text, float duration = 2f, float interval = 0.1f)
	{
		StartCoroutine(_DoTitleText(text,duration,interval));
	}
	
	private int _titletextid = 0;
	private IEnumerator _DoTitleText(string text, float duration = 2f, float interval = 0.1f)
	{
		int id = ++_titletextid;
		
		string cur = "";
		int i = 0;
		
		while(cur.Length<text.Length && id==_titletextid)
		{
			cur += text[i];
			i++;
			
			TitleText.text = cur;
			
			yield return new WaitForSeconds(interval);
		}
		
		float time = 0f;
		while(time<duration && id==_titletextid)
		{
			time+=0.2f;
			yield return new WaitForSeconds(0.2f);
			
			if(cur=="")	cur = text;
			else 		cur = "";
			
			TitleText.text = cur;
		}
		
		if(id==_titletextid)
		{
			TitleText.text = "";
		}
	}
	
}
