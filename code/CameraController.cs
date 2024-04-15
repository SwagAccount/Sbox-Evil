using Sandbox;

public sealed class CameraController : Component
{
	[Property] private float mouseSensitivity { get; set; } = 100f;
    [Property] private GameObject playerBody { get; set; }
    [Property] private Vector2 CamClamp { get; set; }
    public bool rotatePlayer = true;
    float xRotation = 0f;
	float yRotation = 0f;
    private Settings settings;
    protected override void OnStart()
    {
        IEnumerable<GameObject> balls = Scene.GetAllObjects(true);
		foreach(GameObject go in balls)
		{
            if(go.Tags.Has("settings"))
			{
				settings = go.Components.Get<Settings>();
				break;
			}
        }
        yRotation = playerBody.Transform.Rotation.Angles().yaw;
    }
    protected override void OnUpdate()
    {
        float mouseX = Input.AnalogLook.yaw * mouseSensitivity * settings.MouseSens * Time.Delta;
        float mouseY = Input.AnalogLook.pitch * mouseSensitivity * settings.MouseSens * Time.Delta;
        xRotation += mouseY;
        xRotation = MathX.Clamp(xRotation, CamClamp.x, CamClamp.y);
		yRotation += mouseX;
        Transform.LocalRotation = new Angles(xRotation, 0, 0);
		playerBody.Transform.LocalRotation = new Angles(0,yRotation,0);
    }
}