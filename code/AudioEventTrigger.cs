using System.Threading.Tasks;
using Sandbox;

public sealed class AudioEventTrigger : Component, Component.ITriggerListener
{
	[Property] private SoundPointComponent Que {get;set;}
	[Property] private SoundPointComponent Audio {get;set;}
	[Property] private float AudioDuration {get;set;}
	[Property] private float QueLerp {get;set;}
	[Property] private float QueTarget {get;set;} =1f;
	async void startSound()
	{
		Audio.StartSound();
		await Task.DelaySeconds(AudioDuration);
		GameObject.Destroy();
	}
	protected override void OnUpdate()
	{
		
		if(Que!=null)
		{
			Que.Volume = MathX.Lerp(Que.Volume, QueTarget, Time.Delta * QueLerp);
			if(Que.Volume <= 0.01f)
			{
				startSound();
			}
		}
		else
		{	
			if(QueTarget == 0)
			{
				startSound();
			}
		}
	}
	void ITriggerListener.OnTriggerEnter( Collider other ) 
	{
		if(other.Tags.Has("player"))
		{
			QueTarget = 0;
		}
	}
}