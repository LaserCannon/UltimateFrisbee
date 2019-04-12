using UnityEngine;
using System.Collections;



public enum PlayerState
{
	None,
	
	Offense,
	Defense,
	Holding,
}



public class Player : MonoBehaviour
{
	
	public float Speed = 5f;
	
	
	public AudioClip CatchClip;
	public AudioClip ThrowClip;
	
	
	
	
	private PlayerState state = PlayerState.None;
	
	private Team team;
	
	private Vector2 targetPos = new Vector2(10,10);
	
	
	public Team MyTeam
	{
		get { return team; }
		set { team = value; }
	}
	
	public TeamSide MyTeamSide
	{
		get { return team.Side; }
	}
	
	public PlayerState State
	{
		get { return state; }
		set { state = value; }
	}
	
	public Vector2 Position
	{
		get { return new Vector2(transform.position.x,transform.position.z); }
	}
	
	
	
	void Start()
	{
		targetPos = new Vector2(transform.position.x,transform.position.z);
	}
	
	
	
	void Update()
	{
		
		if(GameController.main.State==GameState.InPlay && (state==PlayerState.Defense || state==PlayerState.Offense))
		{
			transform.position = Vector3.MoveTowards(transform.position,new Vector3(targetPos.x,0f,targetPos.y), Time.deltaTime*Speed);
		}
		
	}
	
	
	
	public void RunUpfield(float horizontalgoal, float verticalgoal)
	{
		targetPos = new Vector2(horizontalgoal + Random.Range(-3f,3f), verticalgoal + Random.Range(-5f,5f)*MyTeam.UpfieldDir);
	}
	
	
	public void Cover(Vector2 position, float radius)
	{
		float angle = Random.Range(Mathf.PI/5f,Mathf.PI*4f/5f);
		targetPos = position + new Vector2(Mathf.Cos(angle),Mathf.Sin(angle)) * Random.Range(radius,radius+2f) * -MyTeam.UpfieldDir;
	}
	
	public void SetTarget(Vector2 position)
	{
		targetPos = position;
	}
	
	public void SetPosition(Vector2 position)
	{
		targetPos = position;
		transform.position = new Vector3(targetPos.x,transform.position.y,targetPos.y);
	}
	
	
	
	public void ConfirmHasDisc(Disc disc)
	{
		state = PlayerState.Holding;
		
		if(MyTeamSide==TeamSide.PlayerSide)
		{
			PlayerInput.main.OnSwipe += TryThrowDisc;
		}
	}
	
	private void TryGrabDisc()
	{
		if(GameController.main.State==GameState.InPlay)
		{
			GameController.main.GrabDisc(this);
			
			GetComponent<AudioSource>().PlayOneShot(CatchClip);
		}
	}
	
	private void TryThrowDisc(Vector2 direction)
	{
		if(Formation.IsTouchStartCloseToPlayer())
		{
			ThrowDisc(direction);
		}
	}
	
	private void ThrowDisc(Vector2 direction)
	{
		if(state==PlayerState.Holding && GameController.main.State==GameState.InPlay)
		{
			state = PlayerState.Offense;
			
			GameController.main.ReleaseDisc(direction);
		
			GetComponent<AudioSource>().PlayOneShot(ThrowClip);
		
			if(MyTeamSide==TeamSide.PlayerSide)
			{
				PlayerInput.main.OnSwipe -= ThrowDisc;
			}
		}
	}
	
	private void ThrowAI()
	{
		Player catcher = MyTeam.ChooseRandomCatcher(this);
		Vector3 dir = (catcher.transform.position-transform.position).normalized * Random.Range(900f,1500f);
		
		ThrowDisc(new Vector2(-dir.x,-dir.z));
	}
	
	public void OnCollisionEnter(Collision col)
	{
		if(col.gameObject.tag=="Disc")
		{
			TryGrabDisc();
		}
	}
	
	
}
