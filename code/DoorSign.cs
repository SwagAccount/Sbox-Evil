using Sandbox;

public sealed class DoorSign : Component
{
	[Property] private DoorSystem Door{get;set;}
	[Property] private List<Material> OnOffMaterial{get;set;}
	[Property ] ModelRenderer modelRenderer;
	protected override void OnStart()
	{
		modelRenderer = Components.Get<ModelRenderer>();
	}
	bool lastOpen = true;
	protected override void OnUpdate()
	{
		if(Door.open!=lastOpen)
		{
			modelRenderer.MaterialOverride = Door.open ?
				OnOffMaterial[0] :
				OnOffMaterial[1] ;
		}
		lastOpen = Door.open;
	}
}