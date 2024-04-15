using Sandbox;

public sealed class SetRagdoll : Component
{
	[Property] private ModelPhysics modelPhysics;
	[Property] private List<Rigidbody> droppedObjects;
	GameObject lastParent;
	protected override void OnStart()
	{

	}
	protected override void OnUpdate()
	{
		
		if(GameObject.Parent == Scene)
		{
			if(modelPhysics!=null)modelPhysics.Enabled = true;
			if(droppedObjects!=null)
			{
				for(int i = 0; i < droppedObjects.Count; i++)
				{
					droppedObjects[i].Enabled = true;
					PhysicsLock physicsLock = new PhysicsLock();
					physicsLock.X = false;
					physicsLock.Y = false;
					physicsLock.Z = false;
					physicsLock.Roll = false;
					physicsLock.Yaw = false;
					physicsLock.Pitch = false;
					droppedObjects[i].Locking = physicsLock;
					droppedObjects[i].GameObject.SetParent(null);
				}
			}
			
			Enabled = false;
		}
	}
}