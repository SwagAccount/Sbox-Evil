using Sandbox;

public sealed class ItemDetails : Component, Component.ITriggerListener
{
	[Property] public GunSaveData gunSaveData {get;set;}
	[Property] public string name {get;set;}
	[Property] public float weight {get;set;}
	[Property] public string bulletType {get;set;}
	[Property] public int ammoMax {get;set;}
	[Property] public bool hovered {get;set;}
	[Property] public GameObject weaponInfo {get;set;}
	protected override void OnUpdate()
	{
		if(weaponInfo!=null)
		{
			weaponInfo.Enabled = hovered;
		}
	}
	[Property] public int collisionCount;
	void ITriggerListener.OnTriggerEnter( Collider other )
	{
		if(other.Tags.Has("item"))
		{
			collisionCount++;
		}
	}

	void ITriggerListener.OnTriggerExit( Collider other )
	{
		if(other.Tags.Has("item"))
		{
			collisionCount--;
		}
	}
}