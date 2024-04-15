using Sandbox;

public sealed class WeaponInfoUI : Component
{
	[Property] private TextRenderer Name;
	[Property] private GameObject chamberRound;
	[Property] private TextRenderer Type;
	[Property] private GameObject ammo;
	[Property] private int ammoSpace;
	[Property] private string AmmoChar;
	[Property] private float scale = 0.2f;
	[Property] private float fontSize = 128;
	ItemDetails itemDetails;
	protected override void OnStart()
	{
		itemDetails = GameObject.Parent.Components.Get<ItemDetails>();	
		Name.Text = itemDetails.name;
		Type.Text = itemDetails.bulletType;

		for(int i = 0; i < itemDetails.ammoMax; i++)
		{
			GameObject ammoicon = new GameObject();
			ammoicon.SetParent(ammo);
			ammoicon.Transform.LocalPosition = new Vector3(0, 0, (-ammoSpace)*i);
			ammoicon.Transform.LocalRotation = Angles.Zero;
			ammoicon.Transform.LocalScale = Vector3.One;
			TextRenderer tr = ammoicon.Components.Create<TextRenderer>();
			tr.Scale = scale;
			tr.FontSize = fontSize;
			tr.Text = AmmoChar;
			tr.FontFamily = "halflife2";
		}
	}
	int lastCount = 0;
	protected override void OnUpdate()
	{
		Transform.Rotation = GameObject.Parent.Parent.Transform.Rotation;
		//Ammo.Text = (itemDetails.ammoMax > 0) ? $"{itemDetails.gunSaveData.clipContent.Count}/{itemDetails.ammoMax}" : "";
		ammoDisplay();
	}
	void ammoDisplay()
	{
		if(chamberRound!=null) chamberRound.Enabled = itemDetails.gunSaveData.clipContent.Count > 0;
		for(int i = 0; i < itemDetails.ammoMax; i++)
		{
			if(i < itemDetails.ammoMax)
			{
				ammo.Children[i].Enabled = i < itemDetails.gunSaveData.clipContent.Count-(chamberRound!=null ? 1 : 0);
			}
		}
	}
}