using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public enum CinematicStepType
{
    FreezePlayer,
    UnfreezePlayer,
    SwitchCamera,
    RestoreCamera,
    Wait,
    WalkPlayer,
    StopWalk,
    ZoomCamera,
    PlayDialogue,
    Shake,
    OpenFakeWall
}

[System.Serializable]
public class CinematicStep
{
    public CinematicStepType Type;

    [Header("Camera (SwitchCamera / RestoreCamera / ZoomCamera)")]
    public CinemachineCamera Camera;

    [Header("Generic Values")]
    public float FloatA; // duration, zoom target size, or walk direction, depending on Type
    public float FloatB; // secondary duration (used by ZoomCamera)

    [Header("Dialogue")]
    public List<DialogueLine> Dialogue;

    [Header("Shake")]
    public CinemachineImpulseSource ImpulseSource;


    [Header("Fake Wall")]
    public FakeWall Wall;
}