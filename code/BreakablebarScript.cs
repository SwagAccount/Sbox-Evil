using Sandbox;
using Sandbox.ModelEditor;

public sealed class BreakablebarScript : Component
{
	[Property] private List<Rigidbody> rigidbodies {get; set;}
	[Property] private HEALTHDETECTOR hd {get; set;}
	[Property] private GameObject forcePoint {get; set;}
	[Property] private float breakForce {get;set;}
	[Property] private float health {get;set;}
	[Property] private PhysicsLock unlocked {get;set;}
	[Property] private SoundPointComponent soundPointComponent {get;set;}
	protected override void OnUpdate()
	{
		if(hd.hp >= health)
		{
			soundPointComponent.StartSound();
			foreach(Rigidbody rbin in rigidbodies)
			{
				
				rbin.Locking = unlocked;
				rbin.ApplyForceAt(forcePoint.Transform.Position, forcePoint.Transform.World.Forward*breakForce);
				rbin.GameObject.SetParent(null);
			}
			GameObject.Destroy();
		}
	}
}