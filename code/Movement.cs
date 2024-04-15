using System;
using System.Diagnostics;
using Sandbox;

public sealed class Movement : Component
{
	[Property] private GameObject leftFoot { get; set; }
	[Property] private GameObject rightFoot { get; set; }
	[Property] private float footStepSpeed { get; set; }
	[Property] private float footStepHeight { get; set; }
	[Property] private GameObject cam { get; set; }
	[Property] private CapsuleCollider capsuleCollider{get;set;}
	[Property] private SurvivalFeatures survivalFeatures {get;set;}
	[Property] private float camSpeed { get; set; }
	[Property] public float GroundControl { get; set; } = 4.0f;
	[Property] public float grav { get; set; } = 8f;
	[Property] public float AirControl { get; set; } = 0.1f;
	[Property] public float MaxForce { get; set; } = 50f;
	[Property] public float Speed { get; set; } = 160f;
	[Property] public Curve RunSpeed { get; set; } = 290f;
	[Property] public float WalkSpeed { get; set; } = 90f;
	[Property] public float CrouchSpeed { get; set; } = 90f;
	[Property] public float JumpForce { get; set; } = 400f;
	[Property] public float CrouchHeight { get; set; } = 0.5f;
    public bool IsCrouching = false;
    public bool IsSprinting = false;
    private CharacterController characterController;
	Vector3 WishVelocity = Vector3.Zero;
	private float startHeight;
	private Vector3 camStartPos;
	

	[Property] private Vector3 camTargetPos;
	Vector3 lfStart;
	Vector3 rfStart;
	protected override void OnAwake()
    {
		lfStart = leftFoot.Transform.LocalPosition;
		rfStart = rightFoot.Transform.LocalPosition;
		Scene.PhysicsWorld.Gravity = new Vector3(0,0,grav);
        characterController = Components.Get<CharacterController>();
		startHeight = characterController.Height;
		camStartPos = cam.Transform.LocalPosition;
		camTargetPos = camStartPos;
    }
	Vector3 lastPos;
	float fTime;
	protected override void OnUpdate()
	{
		capsuleCollider.End = cam.Transform.LocalPosition;
		cam.Transform.LocalPosition = Vector3.Lerp(cam.Transform.LocalPosition,camTargetPos,camSpeed*Time.Delta);
        IsSprinting = Input.Down("Run");
		if(Input.Pressed("Jump")) Jump();
		fTime+=(characterController.Velocity.Abs().x+characterController.Velocity.Abs().y+characterController.Velocity.Abs().z)*Time.Delta;
		Vector3 newPosL = lfStart;
		Vector3 newPosR = rfStart;
		newPosL.z+=((float)Math.Sin((double)(fTime*footStepSpeed))+1)*footStepHeight;
		newPosR.z+=((float)Math.Sin((double)(fTime*footStepSpeed)+Math.PI)+1)*footStepHeight;
		leftFoot.Transform.LocalPosition = newPosL;
		rightFoot.Transform.LocalPosition = newPosR;
	}
	protected override void OnFixedUpdate()
	{
		UpdateCrouch(); 	
		BuildWishVelocity();
		Move();
	}
	void UpdateCrouch()
    {
		var tr = Scene.Trace.Ray(Transform.Position,Transform.Position+(Transform.World.Up * 64)).IgnoreGameObject(this.GameObject).Run();
        if(characterController is null) return;
        if(Input.Down("Duck"))
        {
            IsCrouching = true;
            characterController.Height = CrouchHeight; 
			Vector3 newCamPos = camStartPos;
			newCamPos.z *= CrouchHeight/startHeight;
			camTargetPos = newCamPos;
        }
		else
        {
			
			if(IsCrouching)
			{
				if(!tr.Hit)
				{
					Log.Info("FUCK");
					IsCrouching = false;
					characterController.Height = startHeight;
					camTargetPos = camStartPos;
				}
			}
			if(!IsCrouching)
			{
				characterController.Height = startHeight;
				camTargetPos = camStartPos;
			}
        }
    }
	void Jump()
    {
        if(!characterController.IsOnGround) return;
		survivalFeatures.Stamina-=survivalFeatures.StaminaReduceJump;
        characterController.Punch(Vector3.Up * JumpForce);
    }
	void BuildWishVelocity()
    {
        WishVelocity = Transform.Local.Forward*Input.AnalogMove.x-Transform.Local.Right*Input.AnalogMove.y;

        WishVelocity = WishVelocity.WithZ( 0 );

        if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

        if(IsCrouching) WishVelocity *= CrouchSpeed;
        else if(IsSprinting) WishVelocity *= RunSpeed.Evaluate(survivalFeatures.Stamina);
        else WishVelocity *= Speed;
    }
	 void Move()
	{
		var gravity = Scene.PhysicsWorld.Gravity;

		if ( characterController.IsOnGround )
		{
			characterController.Velocity = characterController.Velocity.WithZ( 0 );
			characterController.Accelerate( WishVelocity );
			characterController.ApplyFriction( GroundControl );
		}
		else
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f;
			characterController.Accelerate( WishVelocity.ClampLength( MaxForce ) );
			characterController.ApplyFriction( AirControl );
		}

		characterController.Move();

		if ( !characterController.IsOnGround )
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f;
		}
		else
		{
			characterController.Velocity = characterController.Velocity.WithZ( 0 );
		}
	}
}