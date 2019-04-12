using UnityEngine;
using System.Collections;



public enum GameState
{
	None,
	InPlay,
	OutOfPlay,
	Touchdown,
	GameOver,
}


public class GameController : MonoBehaviour
{
	
	public static GameController main;
	
	
	public Disc DiscPrefab;
	public Player PlayerPrefab;
	public Player AIPrefab;
	
	public Collider EndzoneA;
	public Collider EndzoneB;
	
	public AudioClip ScoreSound;
	

	private Team playerTeam;
	private Team aiTeam;
	
	private Disc currentDisc = null;
	
	private Player hasDisc = null;
	private Player lastHadDisc = null;
	
	private GameState state = GameState.None;
	
	private int playerScore = 0;
	private int aiScore = 0;
	
	
	public Vector2 DiscPosition2
	{
		get { return new Vector2(currentDisc.transform.position.x,currentDisc.transform.position.z); }
	}
	public Vector3 DiscPosition3
	{
		get { return currentDisc.transform.position; }
	}
	
	public Vector2 DiscVelocity2
	{
		get { return new Vector2(currentDisc.Velocity.x,currentDisc.Velocity.z); }
	}
	public Vector3 DiscVelocity3
	{
		get { return currentDisc.Velocity; }
	}
	
	public GameState State
	{
		get { return state; }
	}
	
	public Player HasDisc
	{
		get { return hasDisc; }
	}
	
	public Team PlayerTeam
	{
		get { return playerTeam; }
	}
	
	public Team AITeam
	{
		get { return aiTeam; }
	}
	
	
	void Awake()
	{
		main = this;
		
		Application.targetFrameRate = 60;
	}
	
	void Start()
	{
		state = GameState.InPlay;
		
		playerTeam = new Team(PlayerPrefab);
		aiTeam = new Team(AIPrefab);
		
		playerTeam.Side = TeamSide.PlayerSide;
		aiTeam.Side = TeamSide.AISide;
		
		playerTeam.UpfieldDir = 1;
		aiTeam.UpfieldDir = -1;
		
		currentDisc = (Disc)Instantiate(DiscPrefab,Vector3.up,Quaternion.identity);
		
		FollowCamera.main.Target = currentDisc.transform;
		
		ResetGame();
	}
	
	
	public void SetDiscOutOfPlay()
	{
		if(state==GameState.InPlay)
		{
			state = GameState.OutOfPlay;
			
			//currentDisc.collider.enabled = false;
			
			Invoke ("TurnOver",2.0f);
		}
	}
	
	private void TurnOver()
	{
		state = GameState.InPlay;
		currentDisc.GetComponent<Collider>().enabled = true;
		
		Team teamToTurnoverTo = null;
		if(lastHadDisc==null || lastHadDisc.MyTeam==aiTeam)
			teamToTurnoverTo = playerTeam;
		else if(lastHadDisc.MyTeam==playerTeam)
			teamToTurnoverTo = aiTeam;
		
		Player p = teamToTurnoverTo.FindClosestTeamMemberTo(currentDisc.transform.position);
		GrabDisc(p);
	}
	
	
	public void Touchdown()
	{
		StartCoroutine(_Touchdown());
	}
	
	private IEnumerator _Touchdown()
	{
		state = GameState.Touchdown;
		
		yield return new WaitForSeconds(0.25f);
		
		playerTeam.UpfieldDir = -playerTeam.UpfieldDir;
		aiTeam.UpfieldDir = -playerTeam.UpfieldDir;
		
		GetComponent<AudioSource>().PlayOneShot(ScoreSound);
		
		if(hasDisc.MyTeamSide==TeamSide.PlayerSide)
		{
			playerScore++;
			UIManager.main.DoTitleText("P1 SCORE!!!",2f,0.15f);
		}
		else
		{
			aiScore++;
			UIManager.main.DoTitleText("AI SCORE!!!",2f,0.15f);
		}
		
		UIManager.main.UpdateScore(playerScore,aiScore);
		
		
		Invoke("ResetGame",4f);
	}

	public void ResetGame()
	{
		UIManager.main.BlinkArrows(playerTeam.UpfieldDir>0f);
		UIManager.main.SetPlayerTargets(playerTeam.Players,aiTeam.Players);
		
		state = GameState.InPlay;
		
		playerTeam.ArrangePlayers(false);
		aiTeam.ArrangePlayers(true);
		
		GrabDisc(playerTeam.Players[0],true);
	}
	
	public void GrabDisc(Player player, bool force = false)
	{
		if(hasDisc==null || force)
		{
			currentDisc.GetComponent<Collider>().enabled = true;
		
			Physics.IgnoreCollision(player.GetComponent<Collider>(),currentDisc.GetComponent<Collider>());
			if(lastHadDisc!=null)
			{
				Physics.IgnoreCollision(lastHadDisc.GetComponent<Collider>(),currentDisc.GetComponent<Collider>(),false);
			}
			
			hasDisc = player;
			lastHadDisc = player;
			
			player.ConfirmHasDisc(currentDisc);
			
			currentDisc.Catch();
			
			currentDisc.GetComponent<Rigidbody>().isKinematic = true;
			currentDisc.transform.position = player.transform.position + Vector3.forward*0.4f + Vector3.right*0.4f;
			
			playerTeam.OnDiscCatch(player);
			aiTeam.OnDiscCatch(player);
			
			if(player.MyTeam.UpfieldDir==currentDisc.CurrentEndzone)
			{
				Touchdown();
			}
		}
	}
	
	public void DropDisc()
	{
		if(lastHadDisc!=null)
		{
			Physics.IgnoreCollision(lastHadDisc.GetComponent<Collider>(),currentDisc.GetComponent<Collider>(),false);
		}
		
		lastHadDisc = null;
		hasDisc = null;
		
		currentDisc.GetComponent<Rigidbody>().isKinematic = false;
		
		UIManager.main.Zoom.ZoomIn();
	}
	
	public void ReleaseDisc(Vector2 direction)
	{
		hasDisc = null;
			
		currentDisc.GetComponent<Rigidbody>().isKinematic = false;
		
		currentDisc.Throw(direction);
		
		playerTeam.OnDiscThrow();
		aiTeam.OnDiscThrow();
		
		UIManager.main.Zoom.ZoomIn();
	}
	
	
}
