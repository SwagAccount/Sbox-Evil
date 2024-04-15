using Sandbox;

public sealed class ManualItemAdder : Component
{
	[Property] private ThreeDinv threeDinv {get;set;}
	[Property] private string name {get;set;}
	[Property] private GunSaveData gunSaveData {get;set;}
	[Property] private bool add {get;set;}
	protected override void OnUpdate()
	{
		if(add)
		{
			threeDinv.pickUpWorldItem(gunSaveData,name);
			add = false;
		}
	}
}