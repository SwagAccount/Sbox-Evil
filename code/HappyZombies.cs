using Sandbox;

public sealed class HappyZombies : Component
{
	[Property] private float time;
	protected override void OnStart()
	{
		fart();
	}
	async void fart()
	{
		EnemySaveSystem enemySaveSystem = null;
		IEnumerable<GameObject> balls = Scene.GetAllObjects(true);
		foreach(GameObject go in balls)
		{
			enemySaveSystem = go.Components.Get<EnemySaveSystem>();
			if(enemySaveSystem !=null)
			{
				break;
			}
		}
		enemySaveSystem.toggleHappyZombies();
		await Task.DelaySeconds(time);
		enemySaveSystem.toggleHappyZombies();
		GameObject.Destroy();
	}
}