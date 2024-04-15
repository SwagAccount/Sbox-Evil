using Sandbox;

public sealed class Breakable : Component
{
	[Property] private HEALTHDETECTOR hEALTHDETECTOR {get;set;}
	[Property] private float healthBreakAmount{get;set;}
	[Property] private GameObject broken {get;set;}
	[Property] private SoundPointComponent soundPointComponent {get;set;}
	float lastHP;
	protected override void OnUpdate()
	{
		if(hEALTHDETECTOR.hp - lastHP > healthBreakAmount)
		{
			broken.Enabled = true;
			
			Vector3 pos = broken.Transform.Position;
			Angles rot = broken.Transform.Rotation;
			broken.SetParent(null);
			broken.Transform.Position = pos;
			broken.Transform.Rotation = rot;
			soundPointComponent.StartSound();
			GameObject.Destroy();
		}
		lastHP = hEALTHDETECTOR.hp;
	}
}