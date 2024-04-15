using Microsoft.VisualBasic;
using Sandbox;

public sealed class DoorSystem : Component
{
	[Property] private SaveSystem saveSystem {get;set;}
	[Property] private string NeededLight {get;set;}
	[Property] private LightsScript lightsScript {get;set;}
	[Property] private Interactable interactable {get;set;}
	[Property] public Progress progress {get;set;}
	[Property] public int doorCode {get;set;}
	[Property] private string sceneLoad {get; set;}
	[Property] private Vector3 pPos {get;set;}
	[Property] private Angles pAng {get;set;}
	[Property] private PlayerUI playerUI {get;set;}
	bool lasti;
	public bool open;
	protected override void OnUpdate()
	{
		if(lightsScript!= null) open = lightsScript.levelLight.Contains(NeededLight);
		else if (progress != null) open = progress.progress[0] >= 1;
		else open = true;
		interactable.interactType = open ? "Open" : "Locked";
		if(open)
		{
			if(interactable.Interacted)
			{
				if(!lasti)
				{
					playerUI.opacityTarget = 1;
					Open();
				}

			}
			lasti = interactable.Interacted;
		}
		
	}

	async void Open()
	{
		await Task.DelaySeconds(2);
		saveSystem.loadLevel(sceneLoad,pPos,pAng);
	}
}