using Sandbox;

public sealed class AmmoPickup : Component
{
	[Property] private Interactable interactable {get;set;}
	[Property] private int ammoCount {get;set;}
	[Property] private string ammoType {get;set;}
	private Inventory inv;
	protected override void OnStart()
	{
		IEnumerable<GameObject> playerballs = Scene.GetAllObjects(true);
		foreach(GameObject go in playerballs)
		{
			
			if(go.Tags.Has("player"))
			{
				inv = go.Components.Get<Inventory>();
				break;
			}
		}
	}
	protected override void OnUpdate()
	{
		if(interactable.Interacted)
		{
			inv.ammoData[inv.AmmoIndex(ammoType)].ammoCount+=ammoCount;
			GameObject.Destroy();
		}
	}
}