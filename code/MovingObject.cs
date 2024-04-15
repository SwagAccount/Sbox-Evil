using System;
using Sandbox;

public sealed class MovingObject : Component
{
    [Property] public List<Vector3> positions { get; set; }
    [Property] public List<Angles> angles { get; set; }
    [Property] public List<float> PositionSpeeds { get; set; }
    [Property] public List<float> RotationSpeeds { get; set; }
    [Property] public bool Loop { get; set; }
    [Property] public float PosMult { get; set; } = 1f;
    [Property] public float SpeedMult { get; set; } = 1f;

    [Property] private int currentIndex = 0;
    [Property] private float transitionProgress = 0.0f;

    protected override void OnUpdate()
    {
        if (positions.Count == 0 || angles.Count == 0 || PositionSpeeds.Count == 0 || RotationSpeeds.Count == 0) return;

        int nextIndex = (currentIndex + 1) % positions.Count;

        // Assuming the speeds are defined per transition, and there's a speed defined for each transition
        float currentPosSpeed = PositionSpeeds[Math.Min(currentIndex, PositionSpeeds.Count - 1)] *  SpeedMult;
        float currentRotSpeed = RotationSpeeds[Math.Min(currentIndex, RotationSpeeds.Count - 1)] *  SpeedMult;

        transitionProgress += Time.Delta * Math.Min(currentPosSpeed, currentRotSpeed);

        Transform.LocalPosition = Vector3.Lerp(positions[currentIndex]*PosMult, positions[nextIndex]*PosMult, transitionProgress);
        Transform.LocalRotation = Angles.Lerp(angles[currentIndex], angles[nextIndex], transitionProgress);

        if (transitionProgress >= 1.0f)
        {
            currentIndex = nextIndex;

            transitionProgress = 0.0f;

            if (!Loop && currentIndex == positions.Count - 1)
            {
                this.Enabled = false;
            }
        }
    }
}
