using Sandbox;

public sealed class MovementLocker : Component
{
	[Property] public bool locked {get; set;}
	[Property] public bool endAt {get; set;}
	[Property] public GameObject cameraTarget {get; set;}
	[Property] public GameObject playerTarget {get; set;}
	[Property] public GameObject leftHandT {get; set;}
	[Property] public GameObject rightHandT {get; set;}
	[Property] public FollowTransform leftHand {get; set;}
	[Property] public FollowTransform rightHand {get; set;}
	[Property] public CameraController cameraController {get; set;}
	[Property] public WeaponDealer weaponDealer {get; set;}
	[Property] public Movement movement {get; set;}
	[Property] public float camRotSpeed {get; set;}
	[Property] public float camPosSpeed {get; set;}
	[Property] public float playerRotSpeed {get; set;}
	[Property] public float playerPosSpeed {get; set;}
	[Property] public float unfixdis{get; set;}
	[Property] public bool forceUnqequip {get; set;} = true;

	bool lastLocked = false;
	bool lastFixed = false;
	Vector3 playerReturnPosition;
	Angles playerReturnAngles;
	Vector3 cameraReturnPosition;
	Angles cameraReturnAngles;
	GameObject lHandReturn;
	GameObject rHandReturn;
	[Property] bool Fixed {get; set;}
	Vector3 targetPlayerPos;
	Vector3 targetCamPos;
	Angles targetPlayerRot;
	Angles targetCamRot;

	[Property] public float CustomLerpAngleCheck {get; set;} = 140;
	public Vector3 CustomLerp(Vector3 from, Vector3 target, float speed)
    {
        Vector3 direction = target - from;
		Vector3 newPos = Vector3.Lerp(from,target,speed);
		Vector3 newDirection = target - newPos;
		if(Vector3.GetAngle(direction, newDirection) < CustomLerpAngleCheck)
		{
			return newPos;
		}
		else
		{
			return target;
		}

    }
	protected override void OnAwake()
	{
		targetPlayerPos = Transform.Position;
		targetCamPos = cameraController.Transform.Position;
		targetPlayerRot = Transform.Rotation;
		targetCamRot = cameraController.Transform.Rotation;
	}
	protected override void OnUpdate()
	{
		if(locked && !lastLocked)
		{
			playerReturnAngles = Transform.Rotation;
			playerReturnPosition = Transform.Position;
			cameraReturnAngles = cameraController.GameObject.Transform.Rotation;
			cameraReturnPosition = cameraController.GameObject.Transform.Position;
		}
		
		if(locked)
		{
			Fixed = true;
			if(playerTarget!=null)
			{
				targetPlayerPos = playerTarget.Transform.Position;
				targetPlayerRot = playerTarget.Transform.Rotation;
			}
			else
			{
				targetPlayerPos = Transform.Position;
				targetPlayerRot = Transform.Rotation;
			}
			if(playerTarget!=null)
			{
				targetCamPos = cameraTarget.Transform.Position;
				targetCamRot = cameraTarget.Transform.Rotation;
			}
			else
			{
				targetCamPos = cameraController.Transform.Position;
				targetCamRot = cameraController.Transform.Rotation;
			}
			
			if(endAt)
			{
				playerReturnAngles = targetPlayerRot;
				playerReturnPosition = targetPlayerPos;
				lHandReturn = leftHand.followed;
				rHandReturn = leftHand.followed;
				cameraReturnAngles = targetCamRot;
				cameraReturnPosition = targetCamPos;
			}
			weaponDealer.forceUnqequip = forceUnqequip;
			if(!forceUnqequip || weaponDealer.gunValues == null)
			{
				if(leftHandT!=null) leftHand.followed = leftHandT;
				if(rightHandT!=null) rightHand.followed = rightHandT;
			}
			
		}
		else
		{
			//rightHand.SetParent(null);
			//rightHand.SetParent(null);
			leftHandT = null;
			rightHandT = null;
			endAt = false;
			targetPlayerPos = playerReturnPosition;
			targetCamPos = cameraReturnPosition;
			targetPlayerRot = playerReturnAngles;
			targetCamRot = cameraReturnAngles;
			float playerDis = 
			Vector3.DistanceBetween(Transform.Position,targetPlayerPos); //+ 
			//Vector3.DistanceBetween(new Vector3(Transform.Rotation.x,Transform.Rotation.y,Transform.Rotation.z),new Vector3(targetPlayerRot.pitch, targetPlayerRot.yaw, targetPlayerRot.roll));
			float cameraDis = 
			Vector3.DistanceBetween(cameraController.Transform.Position,targetCamPos) + 
			Vector3.DistanceBetween(
				new Vector3(cameraController.Transform.Rotation.x,cameraController.Transform.Rotation.y,cameraController.Transform.Rotation.z),
				new Vector3(targetCamRot.pitch, targetCamRot.yaw, targetCamRot.roll)
			);
			if(playerDis < unfixdis)
			{
				Fixed = false;
			}
		}
		if(Fixed)
		{
			movement.Enabled = false;
			cameraController.Enabled = false;
			Angles anBalls = cameraController.Transform.Rotation;
			cameraController.GameObject.SetParent(null);
			cameraController.Transform.Rotation = anBalls;
			Transform.Position = CustomLerp(Transform.Position,targetPlayerPos,playerPosSpeed*Time.Delta);
			Transform.Rotation = Angles.Lerp(Transform.Rotation,targetPlayerRot,playerRotSpeed*Time.Delta);
			cameraController.Transform.Position = CustomLerp(cameraController.Transform.Position,targetCamPos,camPosSpeed*Time.Delta);
			cameraController.Transform.Rotation = Angles.Lerp(cameraController.Transform.Rotation,targetCamRot,camRotSpeed*Time.Delta);
		}
		else
		{
			if(lastFixed)
			{
				Transform.Position = targetPlayerPos;
				Transform.Rotation = targetPlayerRot;
				cameraController.Transform.Position = targetCamPos;
				cameraController.Transform.Rotation = targetCamRot;
				weaponDealer.followHands();
			}
			movement.Enabled = true;
			cameraController.Enabled = true;
			cameraController.GameObject.SetParent(this.GameObject);
			weaponDealer.forceUnqequip = false;
		}
		lastLocked = locked;
		lastFixed = Fixed;
	}
}