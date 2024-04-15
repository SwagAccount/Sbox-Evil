using Sandbox;

public sealed class SimpleProjectileDamage : Component, Component.ITriggerListener
{
	[Property] private float damage;
	[Property] private float bleedDamage;
	protected override void OnUpdate()
	{

	}
	void ITriggerListener.OnTriggerEnter(Collider other)
    {
		Log.Info("fuclk");
		HEALTHDETECTOR hd = other.GameObject.Components.Get<HEALTHDETECTOR>();
		if(hd !=null)
		{
			hd.hp+=damage;
			hd.bleedAmount+=bleedDamage;
		}
		
		GameObject.Destroy();
    }
}