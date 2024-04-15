using Sandbox;

public sealed class Footstepper : Component
{
	[Property] private float radius {get; set;} = 2f;
	private BulletHoleDB db;
	GameObject player;
	protected override void OnAwake()
	{
		IEnumerable<GameObject> balls = Scene.GetAllObjects(true);
		foreach(GameObject go in balls)
		{
			if(go.Tags.Has("footstepBase"))
			{
				db = go.Components.Get<BulletHoleDB>();
				break;
			}
		}
		IEnumerable<GameObject> playerballs = Scene.GetAllObjects(true);
		foreach(GameObject go in playerballs)
		{
			if(go.Tags.Has("player"))
			{
				player = go;
				break;
			}
		}
	}
	bool AHH;
	protected override void OnUpdate()
	{
		var tr = Scene.Trace.Ray(Transform.Position,Transform.Position+(radius*Vector3.Down)).IgnoreGameObject(player).Run();
		if(tr.Hit && !AHH)
		{
			AHH = true;
			Log.Info(tr.Surface);
			GameObject swag = db.FindBulletHoleByMaterial(tr.Surface.ResourceName);
			if(swag != null)
			{
				GameObject sound = swag.Clone();
				sound.Transform.Position = Transform.Position;
			}
		}
		else if (!tr.Hit)
		{
			AHH=false;
		}
	}

}