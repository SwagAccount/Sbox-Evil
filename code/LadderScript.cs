using Sandbox;
using System;

public sealed class LadderScript : Component
{
	[Property] private Interactable interactable;
	[Property] private float progress;
	[Property] private float Speed = 1;
	[Property] private float handSpeed;
	[Property] private GameObject bars;
	[Property] private GameObject Body;
	[Property] private GameObject LeftHand;
	[Property] private GameObject RightHand;
	[Property] private GameObject Camera;
	[Property] private GameObject Bottom;
	[Property] private GameObject Top;
	[Property] private float lerp;
	[Property] private int currentBar;
	[Property] private int nextBar;
	List<GameObject> playerPositions;
	List<GameObject> lhPositions;
	List<GameObject> rhPositions;
	[Property] public bool climbing;
	MovementLocker movementLocker;
	protected override void OnStart()
	{
		IEnumerable<GameObject> playerballs = Scene.GetAllObjects(true);
		foreach(GameObject go in playerballs)
		{
			
			if(go.Tags.Has("player"))
			{
				movementLocker = go.Components.Get<MovementLocker>();
				break;
			}
		}

		playerPositions = new List<GameObject>();
		lhPositions = new List<GameObject>();
		rhPositions = new List<GameObject>();
		foreach(GameObject child in bars.Children)
		{
			playerPositions.Add(child.Children[0]);
			lhPositions.Add(child.Children[1]);
			rhPositions.Add(child.Children[2]);
		}
	}
	[Property] bool exiting;
	GameObject exitTarget;
	[Property] private float exitSpeed;
	[Property] private float exitDis = 0.1f;
	protected override void OnFixedUpdate()
	{
		if(climbing)
		{
			if(!exiting)
			{
				progress = MathX.Clamp(progress+Input.AnalogMove.x * Speed,0f,1f);
				movementLocker.locked = true;
				movementLocker.endAt = true;
				movementLocker.rightHandT = RightHand;
				movementLocker.leftHandT = LeftHand;
				movementLocker.playerTarget = Body;
				movementLocker.cameraTarget = Camera;

				currentBar = ConvertFloatToIndex(progress,bars.Children.Count);
				nextBar = Math.Min(currentBar + 1, bars.Children.Count-1);

				lerp = GetFractionBetweenIndices(progress,bars.Children.Count,currentBar,nextBar)*2;
				Body.Transform.Position = Vector3.Lerp(playerPositions[currentBar].Transform.Position,playerPositions[nextBar].Transform.Position,lerp); 

				LeftHand.Transform.Position = Vector3.Lerp(LeftHand.Transform.Position,lhPositions[currentBar].Transform.Position,handSpeed);
				RightHand.Transform.Position = Vector3.Lerp(RightHand.Transform.Position,rhPositions[lerp>0.5f?nextBar:currentBar].Transform.Position,handSpeed);
				if(progress+Input.AnalogMove.x * Speed < 0)
				{
					exiting = true;
					exitTarget = Bottom;
				}
				else if (progress+Input.AnalogMove.x * Speed > 1)
				{
					exiting = true;
					exitTarget = Top;
				}
			}
			else
			{
				Body.Transform.Position = Vector3.Lerp(Body.Transform.Position,exitTarget.Transform.Position,exitSpeed);
				if(Vector3.DistanceBetween(Body.Transform.Position, exitTarget.Transform.Position) < exitDis)
				{
					exiting = false;
					climbing = false;
					movementLocker.locked = false;
				}
			}
		}
		else
		{
			if(interactable.Interacted) 
			{
				climbing =true;
				progress = Vector3.DistanceBetween(movementLocker.Transform.Position,Top.Transform.Position) < Vector3.DistanceBetween(movementLocker.Transform.Position,Bottom.Transform.Position) ? 1 : 0;
				interactable.Interacted = false;
			}
		}
	}
	public static int ConvertFloatToIndex(float value, int listSize)
	{
		int index = (int)Math.Round(value * (listSize - 1));

		return Math.Min(index, listSize - 1);
	}

	public static float GetFractionBetweenIndices(float value, int listSize, int currentI, int nextI)
	{

		float scaledValue = value * (listSize - 1);
		float currentIndexValue = currentI;
		float nextIndexValue = nextI;

		float distanceBetweenIndices = nextIndexValue - currentIndexValue;
		float positionFromCurrentIndex = scaledValue - currentIndexValue;

		float fraction = positionFromCurrentIndex / distanceBetweenIndices;

		return Math.Clamp(fraction, 0f, 1f);
	}

}