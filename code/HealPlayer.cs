using Sandbox;

public sealed class HealPlayer : Component
{
	[Property] private float healAmount;
	[Property] private float highAmount;
	[Property] private bool smoke;
	protected override void OnStart()
	{
		HEALTHDETECTOR hd = null;
		SurvivalFeatures sf = null;
		IEnumerable<GameObject> playerballs = Scene.GetAllObjects(true);
		foreach(GameObject go in playerballs)
		{
			
			if(go.Tags.Has("player"))
			{
				hd = go.Components.Get<HEALTHDETECTOR>();
				sf = go.Components.Get<SurvivalFeatures>();
				break;
			}
		}
		if(hd!=null&&sf!=null)
		{
			hd.hp = MathX.Clamp(hd.hp-healAmount,0,1000);
			sf.Highness = MathX.Clamp(sf.Highness+highAmount,0,1f);
			sf.addicted++;
			if(smoke) sf.Tabbaco = 1;
		}
	}
}