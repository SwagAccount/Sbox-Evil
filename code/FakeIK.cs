using Sandbox;

public sealed class FakeIK : Component
{
	[Property] private GameObject elbow {get;set;}
	[Property] private GameObject arm {get; set;}
	[Property] private float timeToZero {get; set;}
	[Property] private float handRotTime{get; set;}
	GameObject lastParent;
	Vector3 originPos;
	Angles originRot;
	Vector3 bp;
	Angles ba;
	float t = 0;
	protected override void OnUpdate()
	{
		t += Time.Delta;
		if(Transform.Parent != lastParent)
		{
			t = 0;
			originPos = Transform.LocalPosition;
			originRot = Transform.LocalRotation;
		}
		arm.Transform.Rotation = Rotation.LookAt(elbow.Transform.Position - Transform.Position);
		//Transform.LocalPosition = Vector3.Lerp(originPos,Vector3.Zero,t/timeToZero);
		//Transform.LocalRotation = Angles.Lerp(Transform.LocalRotation,Angles.Zero,handRotTime*Time.Delta);
		//Transform.LocalPosition = Vector3.Zero;
		//Transform.LocalRotation = Angles.Zero;
		lastParent = Transform.Parent;
	}
}