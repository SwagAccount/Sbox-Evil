using Sandbox;

public sealed class Interactable : Component
{
	[Property] public bool Interacted;
	[Property] public bool ItemGrabber;
	[Property] public string interactType;
	[Property] public GameObject objectRef;
	protected override void OnUpdate()
	{
	}
}