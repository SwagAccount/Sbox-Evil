using Sandbox;

public sealed class InventorySystem : Component
{
	[Property] private GameObject invParent {get;set;}
	[Property] private GameObject camTarget {get;set;}
	[Property] private MovementLocker movementLocker {get; set;}
	bool open;
	protected override void OnUpdate()
	{
		if(Input.Pressed("menu")) open = !open;
	}
}