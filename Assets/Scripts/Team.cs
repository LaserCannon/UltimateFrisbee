using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum TeamSide
{
	None,
	PlayerSide,
	AISide,
}


public class Team
{
	
	public List<Player> Players = new List<Player>();
	
	public TeamSide Side = TeamSide.None;
	
	public int UpfieldDir = 1;
	
	
	public Team(Player playerPrefab, List<PlayerStats> stats = null)
	{
		for(int i=0;i<7;i++)
		{
			Player newPlayer = (Player)MonoBehaviour.Instantiate(playerPrefab);
			newPlayer.MyTeam = this;
			Players.Add(newPlayer);
		}
	}
	
	
	public void ArrangePlayers(bool receiving)
	{
		for(int i=0;i<Players.Count;i++)
		{
			float angle = Mathf.PI * (float)i/(float)Players.Count;
			Players[i].SetPosition(new Vector2(Mathf.Cos(angle),Mathf.Sin(angle)) * 6f * -UpfieldDir + (receiving ? Vector2.up*-UpfieldDir*10f : Vector2.zero));
		}
	}
	
	public void PlayersToFormation(Formation formation)
	{
		int count = 0;
		for(int i=0;i<Players.Count;i++)
		{
			if(GameController.main.HasDisc==Players[i])
				i++;
			if(i<Players.Count)
				Players[i].SetTarget( formation.PosForT((float)(count)/(float)(Players.Count-2)) );
			count++;
		}
	}
	
	
	public Player FindClosestTeamMemberTo(Vector3 pos, List<Player> exclude = null)
	{
		float closestDist = 99999f;
		Player closest = null;
		
		foreach(Player p in Players)
		{
			float dist = (p.transform.position-pos).magnitude;
			
			if( dist<closestDist && (exclude==null || !exclude.Contains(p)) )
			{
				closestDist = dist;
				closest = p;
			}
		}
		return closest;
	}
	
	
	public Player ChooseRandomCatcher(Player otherThan)
	{
		List<Player> others = new List<Player>(Players);
		others.Remove(otherThan);
		
		return others[Random.Range(0,others.Count-1)];
	}
	
	
	
	public void OnDiscThrow()
	{
		List<Player> threeClosest = new List<Player>();
		
		for(int i=0;i<3;i++)
		{
			threeClosest.Add(FindClosestTeamMemberTo(GameController.main.DiscPosition3,threeClosest));
		}
		
		//foreach(Player p in threeClosest)
		foreach(Player p in Players)
		{
			Vector3 discToPlayer = p.transform.position-GameController.main.DiscPosition3;
			Vector3 discDir = GameController.main.DiscVelocity3.normalized * 100f;
			
			Vector3 estimatedDiscCatchOffset = Vector3.Project(discToPlayer,discDir);
			Vector2 estimated2 = new Vector2(estimatedDiscCatchOffset.x, estimatedDiscCatchOffset.z);
			
			p.SetTarget(GameController.main.DiscPosition2 + estimated2);
			
		//	p.SetTarget(GameController.main.DiscPosition2 +
		//				GameController.main.DiscVelocity2 * (p.transform.position-GameController.main.DiscPosition3).magnitude/p.Speed);
		}
	}
	
	
	
	public void OnDiscCatch(Player playerCaught)
	{
		if(playerCaught.MyTeam==this)
		{	
			for(int i=0;i<Players.Count;i++)
			{
				if(playerCaught==Players[i])
					Players[i].State = PlayerState.Holding;
				else
					Players[i].State = PlayerState.Offense;
				if(Side==TeamSide.AISide)
				{
					Players[i].RunUpfield(30f * (float)i/(float)Players.Count - 15f/2f, playerCaught.transform.position.z + UpfieldDir * 12f);
				}
			}
			
			if(playerCaught.MyTeam.Side==TeamSide.AISide)
			{
				playerCaught.Invoke("ThrowAI",Random.Range(1f,3f));
			}
		}
		else
		{
			Player coverPlayer = FindClosestTeamMemberTo(playerCaught.transform.position);
			
			if(Side==TeamSide.AISide)
			{
				coverPlayer.Cover(new Vector2(playerCaught.transform.position.x,playerCaught.transform.position.z),2f);
			}
			
			for(int i=0;i<Players.Count;i++)
			{
				Players[i].State = PlayerState.Defense;
				
				if(coverPlayer!=Players[i])
				{
					if(Side==TeamSide.AISide)
					{
						Players[i].Cover(new Vector2(playerCaught.transform.position.x,playerCaught.transform.position.z), Random.Range(8f,15f));
					}
				}
			}
		}
	}
	
}
