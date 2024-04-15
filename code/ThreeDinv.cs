using System;
using Sandbox;
using Sandbox.Services;

public sealed class ThreeDinv : Component
{
	[Property] private GameObject Player {get; set;}
	[Property] private GameObject dropPos {get; set;}
	[Property] public GameObject invParent {get; set;}
	[Property] private Inventory inv {get; set;}
	[Property] private MovementLocker mL {get; set;}
	[Property] private GameObject cam {get; set;}
	[Property] private GameObject camd {get; set;}
	[Property] private GameObject PlayerpS {get; set;}
	[Property] private GameObject hand {get; set;}
	[Property] private GameObject handP {get; set;}
	[Property] private GameObject handPL {get; set;}
	[Property] private Vector2 handPos;
	[Property] private Vector2 handPosXClamp {get; set;}
	[Property] private Vector2 handPosYClamp {get; set;}
	[Property] private float sens {get; set;}
	[Property] public List<ItemDetails> items {get; set;}
	[Property] public List<GameObject> itemdoc {get; set;}
	[Property] public List<GameObject> worlditemdoc {get; set;}
	bool hasItem;
	[Property] ItemDetails currentItem;
	[Property] public ItemDetails currentEquip;
	bool ininv;
	private Settings settings;
	private GameObject dropParent;
	protected override void OnAwake()
	{
		IEnumerable<GameObject> playerballs = Scene.GetAllObjects(true);
		int has = 0;
		foreach(GameObject go in playerballs)
		{
			
			if(go.Tags.Has("player"))
			{
				Player = go;
				break;
			}
		}
		foreach(GameObject go in playerballs)
		{
			
			if(go.Tags.Has("settings"))
			{
				settings = go.Components.Get<Settings>();
				break;
			}
		}
		foreach(GameObject go in playerballs)
		{
			
			if(go.Tags.Has("wis"))
			{
				dropParent = go.Children[0];
				break;
			}
		}
		inv = Player.Components.Get<Inventory>();
		mL = Player.Components.Get<MovementLocker>();
	}
	public void pickUpWorldItem(GunSaveData gunSaveData, string gunName)
	{
		GameObject newItem = null;
		for(int i = 0; i < itemdoc.Count; i++)
		{
			ItemDetails ID = itemdoc[i].Components.Get<ItemDetails>();
			string swag = itemdoc[i].Name.Substring(0,itemdoc[i].Name.Length-4);
			Log.Info(swag);
			if(swag == gunName)
			{
				newItem = itemdoc[i];
				break;
			}
		}
		if(newItem!=null)
		{
			GameObject spawnedItem = newItem.Clone();
			ItemDetails itemDetails = spawnedItem.Components.Get<ItemDetails>();
			itemDetails.gunSaveData.clipContent = gunSaveData.clipContent;
			itemDetails.gunSaveData.currentMode = gunSaveData.currentMode;
			itemDetails.gunSaveData.bulletType = gunSaveData.bulletType;
			spawnedItem.Parent = invParent;
			spawnedItem.Transform.LocalPosition = Vector3.Zero;
			hand.Transform.LocalPosition = new Vector3(0,0,hand.Transform.LocalPosition.z);
			spawnedItem.Transform.LocalRotation = Angles.Zero;
			items.Add(itemDetails);
			closest = spawnedItem;
			pickUpItem();
			ininv = true;
		}
	}
	public void equipWeapon(ItemDetails itemDetails)
	{
		if(currentEquip != null)
		{
			currentEquip.gunSaveData.clipContent = inv.weaponsData[0].clipContent;
		}
		currentEquip = itemDetails;
		inv.weapons[0] = itemDetails.name;
		inv.weaponsData[0].clipContent = itemDetails.gunSaveData.clipContent;
		inv.weaponsData[0].currentMode = itemDetails.gunSaveData.currentMode;
		inv.weaponsData[0].bulletType = itemDetails.gunSaveData.bulletType;
	}
	bool lastinv;
	GameObject prevHandT;
	GameObject prevHandTL;
	[Property] Vector2 offsetPos;
	GameObject closest;
	GameObject lastClosest;
	protected override void OnUpdate()
	{
		invParent.Enabled = ininv;
		if(ininv)
		{
			closest = getClostestItem(hand);
			
			if(closest!=lastClosest)
			{
				ItemDetails closestid = closest == null ? null : closest.Components.Get<ItemDetails>();
				ItemDetails lastclosestid = lastClosest == null ? null: lastClosest.Components.Get<ItemDetails>();
				
				if(closestid!=null) closestid.hovered =true;
				if(lastclosestid!=null) lastclosestid.hovered = false;
			}
			mL.locked =true;
			mL.playerTarget = PlayerpS;
			mL.cameraTarget = camd;
			mL.rightHandT = handP;
			mL.leftHandT = handPL;
			cam.Transform.Rotation = Rotation.LookAt(Transform.World.Forward);
			cam.Transform.Position = hand.Transform.Position;
			handPos.x = MathX.Clamp(handPos.x-Input.AnalogLook.pitch*sens*settings.MouseSens*Time.Delta,handPosXClamp.x-offsetPos.x,handPosXClamp.y-offsetPos.x);
			handPos.y = MathX.Clamp(handPos.y+Input.AnalogLook.yaw*sens*settings.MouseSens*Time.Delta,handPosYClamp.x-offsetPos.y,handPosYClamp.y-offsetPos.y);
			hand.Transform.LocalPosition = new Vector3(handPos.x,handPos.y,hand.Transform.LocalPosition.z);
			if(!hasItem)
			{
				if(Input.Pressed("attack1")) pickUpItem();
				if(Input.Pressed("Score"))
				{
					ininv = false;
					mL.rightHandT = prevHandT;
					mL.leftHandT = prevHandTL;
					mL.locked =false;
				}
			}
			else if (hasItem && Input.Pressed("attack1"))
			{
				placeItem();
			}
			if(hasItem)
			{
				float r = 0;
				if(Input.Pressed("Menu"))
				{
					r = 15;
				}
				else if(Input.Pressed("reload"))
				{
					r = -15;
				}
				
				Angles rot = hand.Transform.LocalRotation;
				rot.yaw+=r;
				hand.Transform.LocalRotation = rot;
			}
			else
			{
				hand.Transform.LocalRotation = Angles.Zero;
				if(Input.Pressed("use") && items.Count > 0)
				{
					equipWeapon(getClostestItem(hand).Components.Get<ItemDetails>());
				}
				if(Input.Pressed("attack2") && items.Count > 0)
				{
					dropItem();
				}
			}
			lastClosest = closest;
		}
		else
		{
			if(Input.Pressed("Score") && !mL.locked)
			{
				ininv = true;
				prevHandT = mL.rightHand.followed;
				prevHandTL = mL.leftHand.followed;
			}
			Transform.Position = Player.Transform.Position;
			Transform.Rotation = Player.Transform.Rotation;
		}
	}
	GameObject getClostestItem(GameObject from)
	{
		GameObject closest = null;
		float cDis = 10000000000;
		foreach(ItemDetails i in items)
		{
			Vector2 frompos = new Vector2(from.Transform.Position.x,from.Transform.Position.y);
			Vector2 itempos = new Vector2(i.Transform.Position.x,i.Transform.Position.y);
			float dis = Vector2.DistanceBetween(frompos,itempos);
			if(dis < cDis)
			{
				cDis = dis;
				
				closest = i.GameObject;
			}
		}
		return closest;
	}
	void pickUpItem()
	{
		if(closest!=null)
		{
			Rotation bf = closest.Transform.Rotation;
			Vector3 pbf = closest.Transform.Position;
			currentItem = closest.Components.Get<ItemDetails>();
			closest.Parent = hand;
			closest.Transform.Position = pbf;
			closest.Transform.Rotation = bf;
			offsetPos = new Vector2(closest.Transform.LocalPosition.x,closest.Transform.LocalPosition.y);
			hasItem = true;
		}
		
	}
	void placeItem()
	{
		if(currentItem.collisionCount > 0)
		{
			return;
		}
		Rotation rBF = currentItem.Transform.Rotation;
		Vector3 pBF = currentItem.Transform.Position;
		currentItem.GameObject.Parent = invParent;
		currentItem.Transform.Position = pBF;
		currentItem.Transform.Rotation = rBF;
		hasItem = false;
		offsetPos = Vector3.Zero;
	}
	public void deleteItem(ItemDetails id)
	{
		if(id == currentEquip)
		{
			currentEquip = null;
			inv.weapons[0] = "";
			inv.weaponsData[0].clipContent = new List<int>();
			inv.weaponsData[0].currentMode = 0;
			inv.weaponsData[0].bulletType = 0;
		}
		items.Remove(id);
		id.GameObject.Destroy();
	}
	void dropItem()
	{
		ItemDetails id = closest.Components.Get<ItemDetails>();
		
		items.Remove(id);
		GameObject newItem = null;
		for(int i = 0; i < worlditemdoc.Count; i++)
		{
			ItemDetails ID = worlditemdoc[i].Components.Get<ItemDetails>();
			string swag = worlditemdoc[i].Name.Substring(0,worlditemdoc[i].Name.Length-5);
			if(swag == id.name)
			{
				newItem = worlditemdoc[i];
				break;
			}
		}
		GameObject newSpawn = newItem.Clone();
		WorldItemScript worldItemScript = newSpawn.Components.Get<WorldItemScript>();
		worldItemScript.gunSaveData.clipContent = id.gunSaveData.clipContent;
		worldItemScript.gunSaveData.currentMode = id.gunSaveData.currentMode;
		worldItemScript.gunSaveData.bulletType = id.gunSaveData.bulletType;
		newSpawn.SetParent(dropParent);
		newSpawn.Transform.Position = dropPos.Transform.Position;
		newSpawn.Transform.Rotation = dropPos.Transform.Rotation;
		if(id == currentEquip)
		{
			currentEquip = null;
			inv.weapons[0] = "";
			inv.weaponsData[0].clipContent = new List<int>();
			inv.weaponsData[0].currentMode = 0;
			inv.weaponsData[0].bulletType = 0;
		}
		closest.Destroy();
	}

}

