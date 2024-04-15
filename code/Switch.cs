using System;
using System.Diagnostics.Tracing;
using Sandbox;
using Sandbox.UI;

public sealed class Switch : Component
{
	public event Action onAction;
	public event Action offAction;
	[Property] private Interactable interactable {get; set;}
	[Property] private DoorScript doorScript {get; set;}
	[Property] private DoorSystem doorSystem {get; set;}
	[Property] private SwankDoor swankDoor {get; set;}
	[Property] private LightsScript lightsScript {get; set;}
	[Property] private SkinnedModelRenderer model  {get; set;}
	[Property] public bool Using  {get; set;}
	[Property] public bool on  {get; set;}
	[Property] float delay  {get; set;}
	private MovementLocker movementLocker;
	private GameObject player;
	[Property] private GameObject playerPos  {get; set;}
	[Property] private GameObject cameraPos  {get; set;}
	[Property] private GameObject leftHandPos  {get; set;}
	[Property] private Progress progress {get;set;}
	[Property] private float sens  {get; set;}
	private Settings settings;
	protected override void OnStart()
	{
		Random r = new Random();
		IEnumerable<GameObject> playerballs = Scene.GetAllObjects(true);
		foreach(GameObject go in playerballs)
		{
			
			if(go.Tags.Has("player"))
			{
				player = go;
				break;
			}
		}
		foreach(GameObject go in playerballs)
		{
			
			if(go.Tags.Has("settings"))
			{
				settings = go.Components.Get<Settings>();
				break;
			}
		}
		
		movementLocker = player.Components.Get<MovementLocker>();
		if(progress.progress[0] == 1)
		{
			enableType(true);
		}
	}
	List<float> previousProgress;
	bool moved;
	void enableType(bool to)
	{
		if(doorScript!=null)doorScript.open=to;
		else if(lightsScript!=null)lightsScript.addLight(to);
		else if(doorSystem!=null) doorSystem.progress.progress[1] = to ? 1 : 0;
		else if (swankDoor!=null) swankDoor.progress.progress[1] = to ? 1:0;
	}
	protected override void OnUpdate()
	{
		if(interactable.Interacted)
		{
			interactable.Interacted = false;
			Use();
		}
		model.Set("progress", progress.progress[0]);
		
		if(Using)
		{
			movementLocker.locked = true;
			movementLocker.locked = true;
			movementLocker.playerTarget = playerPos;
			movementLocker.cameraTarget = cameraPos;
			movementLocker.leftHandT = leftHandPos;
			progress.progress[0] = MathX.Clamp(progress.progress[0]+(Input.AnalogLook.pitch * sens * settings.MouseSens * Time.Delta),0,1);
			if(progress.progress[0] >= 0.01f && progress.progress[0] <= 0.99f) moved = true;
			if(moved)
			{
				if(progress.progress[0] == 0)
				{
					movementLocker.locked = false;
					Using = false;
					enableType(false);
				}
				else if(progress.progress[0] == 1)
				{
					Using = false;
					movementLocker.locked = false;
					enableType(true);
				}
			}
		}
		else
		{
			moved = false;
			previousProgress = progress.progress;
		}
	}
	public void Use()
	{
		Using = true;
	}
}