using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Formation : MonoBehaviour
{
	
	public float interval = 0.25f;
	
	public Renderer FormationNodePrefab;
	
	List<Vector2> path = new List<Vector2>();
	
	List<Renderer> formationNodes = new List<Renderer>();
	
	
	public Vector2 Firstpos
	{
		get { return path[0]; }
	}
	
	public Vector2 Lastpos
	{
		get { return path[path.Count-1]; }
	}
	
	
	
	void Start()
	{
		PlayerInput.main.OnTouchStart += TouchStarted;
		PlayerInput.main.OnTouchUpdate += TouchUpdated;
		PlayerInput.main.OnTouchEnd += TouchEnded;
		
		for(int i=0;i<6;i++)
		{
			Renderer node = (Renderer)Instantiate(FormationNodePrefab);
			formationNodes.Add(node);
			node.enabled = false;
		}
	}
	
	
	
	void TouchStarted(Vector2 pos)
	{
		path.Clear();
		
		//Convert pos
		pos = ScreenPointToCoordinates(pos);
		
		path.Add(pos);
	}
	
	void TouchUpdated(Vector2 pos)
	{
		//Safety
		if(interval<=0f)
			interval = 0.01f;
		
		//Convert pos
		pos = ScreenPointToCoordinates(pos);
		
		float dist = (pos-Lastpos).magnitude;
		
		while(dist>interval && path.Count<1000)
		{
			path.Add(Lastpos + (pos-Lastpos).normalized*interval);
			dist -= interval;
		}
		
		if((!IsTouchStartCloseToPlayer() || (GameController.main.HasDisc!=null && GameController.main.HasDisc.MyTeamSide==TeamSide.AISide)) && path.Count>2)
		{
			for(int i=0;i<formationNodes.Count;i++)
			{
				formationNodes[i].enabled = true;
				Vector2 p = PosForT((float)i/(float)(formationNodes.Count-1));
				formationNodes[i].transform.position = new Vector3(p.x,5,p.y);
			}
		}
	}
	
	void TouchEnded(Vector2 pos)
	{
		if(path.Count>2 && GameController.main.HasDisc!=null &&
			(!IsTouchStartCloseToPlayer() || (GameController.main.HasDisc!=null && GameController.main.HasDisc.MyTeamSide==TeamSide.AISide)))
		{
			GameController.main.PlayerTeam.PlayersToFormation(this);
		
			for(int i=0;i<formationNodes.Count;i++)
			{
				formationNodes[i].enabled = false;
			}
		}
	}
	
	
	public static Vector2 ScreenPointToCoordinates(Vector2 screenPos)
	{
		Vector3 worldPos = FollowCamera.main.GetComponent<Camera>().ScreenToWorldPoint(screenPos);
		return new Vector2(worldPos.x,worldPos.z);
	}
	
	public static bool IsTouchStartCloseToPlayer()
	{
		 return GameController.main.HasDisc==null ||
		//	GameController.main.HasDisc.MyTeamSide==TeamSide.PlayerSide &&
			(ScreenPointToCoordinates(PlayerInput.main.TouchStart)-GameController.main.HasDisc.Position).magnitude < 4f;
	}
	
	public Vector2 PosForT(float t)
	{
		t = Mathf.Clamp01 (t);
		
		float segmentsThrough = t*(float)(path.Count-2);
		int index = (int)Mathf.Floor(segmentsThrough);
		float part = segmentsThrough-(float)index;
		
		return Vector2.Lerp(path[index],path[index+1],part);
	}
	
	
}
