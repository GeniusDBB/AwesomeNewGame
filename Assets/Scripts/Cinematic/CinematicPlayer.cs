using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CinematicPlayer : MonoBehaviour
{
    [SerializeField] private List<CinematicStep> _steps = new();

    private PlayerMovement _player;

    public void Play()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        _player = playerObj.GetComponent<PlayerMovement>();
        StartCoroutine(RunSteps());
    }

    private IEnumerator RunSteps()
    {
        foreach (var step in _steps)
        {
            yield return RunStep(step);
        }
    }

    private IEnumerator RunStep(CinematicStep step)
    {
        switch (step.Type)
        {
            case CinematicStepType.FreezePlayer:
                _player.SetFrozen(true);
                break;

            case CinematicStepType.UnfreezePlayer:
                _player.SetFrozen(false);
                break;

            case CinematicStepType.SwitchCamera:
                step.Camera.Priority = 999;
                break;

            case CinematicStepType.RestoreCamera:
                step.Camera.Priority = 5; // matches the resting priority you set on cutscene cams
                break;

            case CinematicStepType.Wait:
                yield return new WaitForSeconds(step.FloatA);
                break;

            case CinematicStepType.WalkPlayer:
                _player.StartCutsceneWalk(step.FloatA); // FloatA = direction, 1 or -1
                break;

            case CinematicStepType.StopWalk:
                _player.StopCutsceneWalk();
                break;

            case CinematicStepType.ZoomCamera:
                yield return ZoomCameraRoutine(step.Camera, step.FloatA, step.FloatB);
                break;

            case CinematicStepType.PlayDialogue:
                DialogueManager.Instance.StartDialogue(step.Dialogue);
                while (!DialogueManager.Instance.IsDialogueFinished)
                {
                    yield return null;
                }
                break;

            case CinematicStepType.Shake:
                step.ImpulseSource.GenerateImpulse();
                break;

            case CinematicStepType.OpenFakeWall:
                yield return step.Wall.Open();
                break;

            case CinematicStepType.StartCaveIn:
                step.CaveIn.StartCaveIn();
                break;
            case CinematicStepType.TurnPlayer:
                _player.ForceFacing(step.FloatA > 0);
                break;

            case CinematicStepType.Bark:
                DialogueManager.Instance.ShowBark(step.BarkSpeaker, step.BarkText, step.FloatA);
                break;

        }
    }

    private IEnumerator ZoomCameraRoutine(CinemachineCamera cam, float targetSize, float duration)
    {
        float startSize = cam.Lens.OrthographicSize;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            var lens = cam.Lens;
            lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, t / duration);
            cam.Lens = lens;
            yield return null;
        }
    }
}