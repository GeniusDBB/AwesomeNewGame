using UnityEngine;
using Unity.Cinemachine;

[DisallowMultipleComponent]
public class CameraFixedHeight : CinemachineExtension
{
    [Tooltip("The camera's center height in world coordinates.")]
    [SerializeField] private float _worldY = 0f;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage != CinemachineCore.Stage.Finalize)
            return;

        // Final camera position includes PositionCorrection.
        Vector3 position = state.RawPosition;
        position.y = _worldY - state.PositionCorrection.y;
        state.RawPosition = position;
    }
}