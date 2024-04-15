using System;
using Sandbox;

public sealed class  Fire : Component, Component.ITriggerListener
{
	[Property] private List<PointLight> pointLight {get; set;}
	[Property] private float scale {get; set;}
	[Property] private float power{get;set;}
	[Property] private float light {get;set;}
	[Property] private Curve lightCurve {get;set;}
	[Property] public float fireHealth {get; set;}
	[Property] public Vector3 targetPosDead {get;set;}
	[Property] public List<Vector3> healthMove {get;set;}
	[Property] public float dieSpeed {get; set;}
	[Property] float t;
	float startsize;
	protected override void OnStart()
	{
		startsize = fireHealth;
	}
	protected override void OnUpdate()
	{
		t+=Time.Delta;
		if(t>=1) t = 0;
		foreach(PointLight pl in pointLight)
		{
			pl.Radius=light+(lightCurve.Evaluate(t*scale)*power);
		}
		if(fireHealth <= 1)
		{
			Transform.Position = Vector3.Lerp(Transform.Position,targetPosDead,Time.Delta*dieSpeed);
			if(Vector3.DistanceBetween(Transform.Position,targetPosDead) < 1){
				GameObject.Destroy();
			}
		}
		else
		{
			Transform.LocalPosition = Vector3.Lerp(healthMove[0],healthMove[1],startsize-(fireHealth/startsize));
		}
	}
	void ITriggerListener.OnTriggerEnter( Collider other ) 
	{
		if(other.Tags.Has("fireex"))
		{
			fireHealth--;
			other.GameObject.Destroy();
		}
		HEALTHDETECTOR hD = other.Components.Get<HEALTHDETECTOR>();
		if(hD!=null)
		{
			hD.bleedAmount+=100;
		}
	}
	void ITriggerListener.OnTriggerExit( Collider other ) {}
}