using Sandbox;

public sealed class Hallucination : Component
{
	[Property] private float DisapearAngle;
	[Property] private float DisapearDis;
	[Property] private float DisapearTime;
	[Property] private string animationName;
	GameObject player;
	SkinnedModelRenderer modelRenderer;
	SoundPointComponent soundPointComponent;
	protected override void OnStart()
	{
		modelRenderer = Components.Get<SkinnedModelRenderer>();
		soundPointComponent = Components.Get<SoundPointComponent>();
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
	bool lois;
	protected override async void OnFixedUpdate()
	{
		if(Vector3.DistanceBetween(Transform.Position,player.Transform.Position) > DisapearDis) GameObject.Destroy();
		if(Vector3.GetAngle(Transform.World.Forward,-player.Transform.World.Forward) < DisapearAngle && !lois)
		{
			if(soundPointComponent != null) soundPointComponent.StartSound();
			if(animationName!=null) modelRenderer.Set(animationName,true);
			lois = true;
			await Task.DelaySeconds(DisapearTime);
			GameObject.Destroy();
		} 
	}
}