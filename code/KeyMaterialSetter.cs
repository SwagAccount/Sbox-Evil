using Sandbox;

public sealed class KeyMaterialSetter : Component
{
	[Property] private GunValues gunValues {get;set;}
	[Property] private ItemDetails itemDetails {get;set;}
	[Property] private WorldItemScript worldItemDetails {get;set;}
	[Property] private ModelRenderer modelRenderer {get;set;}
	[Property] private List<Material> materials {get;set;}
	[Property] public Inventory inv { get; set; }
	protected override void OnStart()
	{
		IEnumerable<GameObject> balls = Scene.GetAllObjects(true);
		foreach(GameObject go in balls)
		{
			
			if(go.Tags.Has("player"))
			{
				inv = go.Components.Get<Inventory>();
				break;
			}
		}
		if (gunValues != null) modelRenderer.MaterialOverride = materials[inv.weaponsData[0].currentMode];
		else if (itemDetails != null)  modelRenderer.MaterialOverride = materials[itemDetails.gunSaveData.currentMode];
		else if (worldItemDetails != null)  modelRenderer.MaterialOverride = materials[worldItemDetails.gunSaveData.currentMode];
	}
}