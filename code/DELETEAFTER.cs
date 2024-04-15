using Sandbox;

public sealed class DELETEAFTER : Component
{
	[Property] public float time {get; set;}
    protected override void OnAwake()
    {
		doit();
    }
	async void doit()
	{
		await Task.DelayRealtimeSeconds(time);
		GameObject.Destroy();

	}
}