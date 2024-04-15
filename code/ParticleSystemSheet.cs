using Sandbox;

public sealed class ParticleSystemSheet : Component
{
	[Property] ParticleEffect particleEffect {get;set;}
	[Property] float speed {get;set;}
	[Property] float dir {get;set;}
	float f;
	protected override void OnAwake()
	{
		loop();
	}
	async void loop()
	{
		while(true)
		{
			f+=dir;
			ParticleFloat pf = f;
			particleEffect.SequenceId = pf;
			await Task.DelaySeconds(1/speed);
		}
	}
}