using Sandbox;

public sealed class SwankDoor : Component
{
	[Property] private GameObject rotatedDoor {get; set;}
	[Property] private List<Interactable> interactables {get; set;}
	[Property] private GameObject HandLT {get; set;}
	[Property] private GameObject HandT {get; set;}
	[Property] public int doorCode {get; set;}
	[Property] private GameObject PlayerT {get; set;}
	[Property] private GameObject CameraT {get; set;}
	[Property] private List<Vector3> playerPositions {get; set;} = new List<Vector3>(2);
	[Property] private List<Angles> doorRotations {get; set;} = new List<Angles>(2);
	[Property] private List<Vector3> doorPositions {get; set;} = new List<Vector3>(2);
	[Property] public Progress progress {get; set;}
	[Property] private float sens  {get; set;}
	[Property] private bool animated  {get; set;}
	[Property] private SkinnedModelRenderer renderer  {get; set;}
	[Property] public SoundPointComponent unlockSound  {get; set;}
	private MovementLocker movementLocker;
	private GameObject player;
	bool Using;
	protected override void OnAwake()
	{
		IEnumerable<GameObject> playerballs = Scene.GetAllObjects(true);
		foreach(GameObject go in playerballs)
		{
			
			if(go.Tags.Has("player"))
			{
				player = go;
				break;
			}
		}
		movementLocker = player.Components.Get<MovementLocker>();
	}
	List<float> previousProgress;
	bool moved;
	bool Interacted()
	{
		foreach(Interactable interactable in interactables)
		{
			if(interactable.Interacted) return true;
		}
		return false;
	}
	void SetInteracted(bool to)
	{
		foreach(Interactable interactable in interactables)
		{
			interactable.Interacted = to;
		}
	}
	protected override void OnUpdate()
	{
		if(Interacted())
		{
			SetInteracted(false);
			if (progress.progress[1] == 1) Using = true;
			else ConsoleSystem.Run( "playermessage", "Locked." );
		}
		if(!animated)
		{
			rotatedDoor.Transform.LocalRotation = Angles.Lerp(doorRotations[0],doorRotations[1],progress.progress[0]);
			if(doorPositions.Count > 0) rotatedDoor.Transform.LocalPosition = Vector3.Lerp(doorPositions[0],doorPositions[1],progress.progress[0]);
		}
		else
		{
			renderer.Set("progress", progress.progress[0]);
		}
		PlayerT.Transform.LocalPosition = Vector3.Lerp(playerPositions[0],playerPositions[1],progress.progress[0]);
		if(Using)
		{
			movementLocker.locked = true;
			movementLocker.endAt = true;
			movementLocker.playerTarget = PlayerT;
			movementLocker.cameraTarget = CameraT;
			movementLocker.leftHandT = HandLT;
			movementLocker.rightHandT = HandT;
			progress.progress[0] = MathX.Clamp(progress.progress[0]+(-Input.AnalogLook.yaw * sens * Time.Delta),0,1);
			if(progress.progress[0] >= 0.01f && progress.progress[0] <= 0.99f) moved = true;
			if(moved)
			{
				if(progress.progress[0] <= 0.01f)
				{
					Using = false;
					movementLocker.locked = false;
					Scene.NavMesh.Generate(Scene.PhysicsWorld);
				}
				else if(progress.progress[0] >= 0.99f)
				{
					Using = false;
					movementLocker.locked = false;
					Scene.NavMesh.Generate(Scene.PhysicsWorld);
				}
			}
		}
		else
		{
			moved = false;
			previousProgress = progress.progress;
		}
		/*
		movementLocker.locked = Using;
		if(Using)
		{
			movementLocker.playerTarget = playerPos;
			movementLocker.cameraTarget = cameraPos;
			movementLocker.leftHandT = leftHandPos;
			switchProgress = MathX.Clamp(switchProgress+(Input.AnalogLook.pitch * sens * Time.Delta),0,1);
			if(switchProgress!=previousProgress) moved = true;
			if(moved)
			{
				if(switchProgress == 0)
				{
					Using = false;
					if(doorScript!=null)doorScript.open=false;
				}
				else if(switchProgress == 1)
				{
					Using = false;
					if(doorScript!=null)doorScript.open=true;
				}
			}
		}
		else
		{
			moved = false;
			previousProgress = switchProgress;
		}
		*/
	}
}