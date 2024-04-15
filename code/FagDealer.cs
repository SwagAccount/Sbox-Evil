using Sandbox;

public sealed class FagDealer : Component
{
	[Property] public float progress;
	[Property] private Curve lightCurve;
	[Property] private float scale {get; set;}
	[Property] private float power{get;set;}
	[Property] private float light {get;set;}
	[Property] private Vector2 smokePositions {get;set;}
	[Property] private PointLight pointLight {get;set;}
	[Property] private SkinnedModelRenderer model;
	[Property] float t;
	protected override void OnUpdate()
	{
		t+=Time.Delta;
		if(t>=1) t = 0;
		if(pointLight != null)
		{
			pointLight.Radius = GameObject.Parent != Scene ? light+(lightCurve.Evaluate(t*scale)*power) : 0;
			pointLight.Transform.LocalPosition = new Vector3(0,0,MathX.Lerp(smokePositions.x,smokePositions.y,(progress-0.5f)*2));
		}
		
		model.Set("progress", progress);
	}
}