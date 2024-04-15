using Sandbox;

public sealed class ZombieTrigger : Component, Component.ITriggerListener
{
	[Property] List<ZombieAI> zombies {get; set;}
	void ITriggerListener.OnTriggerEnter( Collider other ) 
	{
		if(other.Tags.Has("player"))
		{
			foreach(ZombieAI z in zombies)
			{
				z.eSS.bools[0] = true;
			}
		}
		
	}
	void ITriggerListener.OnTriggerExit( Collider other ) {}
}