using Sandbox;

public sealed class DoorScript : Component
{
	[Property] private SkinnedModelRenderer skinnedModel {get; set;}
	[Property] private Collider closedDoor {get;set;}
	[Property] private Collider openDoor {get;set;}
	[Property] private SoundPointComponent soundPoint{get;set;}
	[Property] public bool open {get; set;}
	bool lastOpen;
	[Property] public bool nosound {get; set;}
	protected override void OnStart()
	{
		if(open) nosound = true;
	}
	protected override void OnUpdate()
	{
		skinnedModel.Set("open", open);
		closedDoor.Enabled = !open;
	    openDoor.Enabled = open;
		if(lastOpen!=open && !nosound)
		{
			soundPoint.StartSound();
		}
		lastOpen = open; 
	}
}