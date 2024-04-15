using Sandbox;

public sealed class DoorOpener : Component
{
	[Property] public Inventory inv { get; set; }
	[Property] public float dis { get; set; }
	protected override void OnStart()
	{
		IEnumerable<GameObject> balls = Scene.GetAllObjects(true);
		foreach(GameObject go in balls)
		{
			
			if(go.Tags.Has("player"))
			{
				inv = go.Components.Get<Inventory>();
				break;
			}
		}
		var tr = Scene.Trace.Ray(Transform.Position,Transform.Position+Transform.World.Forward*dis).IgnoreGameObject(inv.GameObject).Run();
		if(tr.Hit)
		{
			Log.Info(tr.GameObject.Name);
			Interactable interactable = tr.GameObject.Components.Get<Interactable>();
			if(interactable!=null)
			{
				
				SwankDoor swankDoor = interactable.objectRef != null ? interactable.objectRef.Components.Get<SwankDoor>():null;
				Log.Info(swankDoor);
				if(swankDoor != null)
				{
					if(swankDoor.doorCode == inv.weaponsData[0].currentMode)
					{
						swankDoor.progress.progress[1] = 1;
						swankDoor.unlockSound.StartSound();
					}
				}

				DoorSystem doorScript = interactable.Components.Get<DoorSystem>();
				
				if(doorScript != null)
				{
					if(doorScript.progress != null && doorScript.doorCode == inv.weaponsData[0].currentMode)
					{
						doorScript.progress.progress[0] = 1;
					}
				}
			}
			
			
		}
		GameObject.Destroy();
	}
}