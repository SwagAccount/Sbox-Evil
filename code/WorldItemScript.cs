using Sandbox;

public sealed class WorldItemScript : Component
{
	[Property] private Interactable interactable {get; set;}
	ThreeDinv threeDinv;
	[Property] public string name {get; set;}
	[Property] public GunSaveData gunSaveData {get; set;}
	[Property] public Rigidbody rb {get; set;}
	[Property] public bool stat {get; set;}

	protected override void OnStart()
	{
		IEnumerable<GameObject> balls = Scene.GetAllObjects(true);
		foreach(GameObject go in balls)
		{
			if(go.Tags.Has("inventory"))
			{
				threeDinv = go.Components.Get<ThreeDinv>();
				break;
			}
		}
		if(stat)
		{
			PhysicsLock physicsLock = new PhysicsLock();
			physicsLock.X = true;
			physicsLock.Y = true;
			physicsLock.Z = true;
			physicsLock.Roll = true;
			physicsLock.Yaw = true;
			physicsLock.Pitch = true;
			rb.Locking = physicsLock;
		}
	}
	protected override void OnUpdate()
	{
		if(interactable.Interacted)
		{
			threeDinv.pickUpWorldItem(gunSaveData,name);
			GameObject.Destroy();
		}

	}
}