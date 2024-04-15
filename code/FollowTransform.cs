using Sandbox;

public sealed class FollowTransform : Component
{
	[Property] public GameObject followed {get; set;}
    [Property] public string tag {get;set;}
    [Property] public bool followPos {get; set;} = true;
    [Property] public bool followRot {get; set;} = true;
    [Property] public bool lerpTo {get; set;} = true;
    [Property] public float lerpSpeedPos {get; set;}
    [Property] public float lerpSpeedRot {get; set;}
    [Property] public float lerpDis {get; set;}
    [Property] public bool locked {get; set;}
    GameObject lastF;
	protected override void OnStart()
	{
        if(tag!=null && tag!="")
        {
            IEnumerable<GameObject> balls = Scene.GetAllObjects(true);
            foreach(GameObject go in balls)
            {
                
                if(go.Tags.Has(tag))
                {
                    followed = go;
                    break;
                }
            }
        }
	}
	protected override void OnPreRender()
	{
        if(lastF!=followed) locked = false;
		if(followed != null)
        {
            if(!lerpTo||locked)
            {
                if (followPos) Transform.Position = followed.Transform.World.Position;

                if (followRot) Transform.Rotation = followed.Transform.World.Rotation;
            }
            else
            {
                locked = Vector3.DistanceBetween(Transform.Position,followed.Transform.Position) <= lerpDis;

                if (followPos) Transform.Position = Vector3.Lerp(Transform.Position,followed.Transform.World.Position,Time.Delta*lerpSpeedPos);

                if (followRot) Transform.Rotation = Angles.Lerp(Transform.Rotation,followed.Transform.World.Rotation,Time.Delta*lerpSpeedRot);
            }
        }
        
        lastF = followed;
	}
}