using System.Reflection;
using Sandbox;

public sealed class InteractScript : Component
{
	[Property] float range {get; set;}
	[Property] GameObject ignor {get; set;}
	[Property] MovementLocker movementLocker {get; set;}
	[Property] GameObject handTarget {get; set;}
	[Property] GameObject handTargetReal {get; set;}
	[Property] FollowTransform leftHand {get; set;}
	[Property] WeaponDealer weaponDealer {get; set;}
	[Property] PlayerUI playerUI {get; set;}
	Interactable interactable;
	protected override void OnUpdate()
	{
		if(interactable!=null)
		{
			playerUI.interactType = interactable.interactType;
			handTarget.Transform.Position = interactable.Transform.Position;
			handTarget.Transform.Rotation = Rotation.LookAt(Transform.World.Forward);
			if(leftHand.locked && interactable.ItemGrabber)
			{
				interactable.Interacted = true;
				interactable = null;
				movementLocker.forceUnqequip = true;
				movementLocker.locked = false;
			}
			else if (!interactable.ItemGrabber)
			{
				interactable.Interacted = true;
				interactable = null;
			}
		}
		else
		{
			
		}
		if(interactable==null)
		{
			var sTR = Scene.Trace.IgnoreGameObject(ignor).Ray(Transform.Position,Transform.Position+(Transform.World.Forward*range)).Size(1f).Run();
			playerUI.interactType = null;
			if(sTR.GameObject != null)
			{
				var inte = sTR.GameObject.Components.Get<Interactable>();
				if(inte!=null)
				{
					playerUI.interactType = inte.interactType;
					if(Input.Pressed("use"))
					{
						interactable = inte;
						if(interactable.ItemGrabber)
						{
							movementLocker.playerTarget = null;
							movementLocker.cameraTarget = null;
							movementLocker.rightHandT = null;
							movementLocker.leftHandT = handTargetReal;
							movementLocker.forceUnqequip = false;
							movementLocker.locked = true;
						}
					}
				}
			}
		}
		
	}
}