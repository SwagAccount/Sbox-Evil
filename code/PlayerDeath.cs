using Sandbox;

public sealed class PlayerDeath : Component
{
	[Property] public HEALTHDETECTOR hp;
	[Property] private Movement movement;
	[Property] private CharacterController characterController;
	[Property] private Inventory inventory;
	[Property] private ThreeDinv tdi;
	[Property] private CameraController cameraController;
	[Property] private GameObject weaponDealer;
	[Property] private GameObject lhand;
	[Property] private GameObject rhand;
	[Property] public float Health;
	protected override void OnStart()
	{
		IEnumerable<GameObject> balls = Scene.GetAllObjects(true);
		foreach(GameObject go in balls)
		{
			
			if(go.Tags.Has("inventory"))
			{
				tdi = go.Components.Get<ThreeDinv>();
				break;
			}
		}
	}
	protected override void OnUpdate()
	{
		if(hp.hp >= Health)
		{
			movement.Destroy();
			inventory.Destroy();
			characterController.Destroy();
			tdi.Destroy();
			cameraController.Destroy();
			weaponDealer.Destroy();
			lhand.Destroy();
			rhand.Destroy();
			Rigidbody rb = Components.Create<Rigidbody>();
			rb.AngularVelocity = Transform.World.Right;
			this.Destroy();
		}
	}
}