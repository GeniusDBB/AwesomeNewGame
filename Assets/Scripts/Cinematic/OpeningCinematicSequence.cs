using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// Starts the opening Timeline only when New Game requested it.  Once Timeline
/// ends, it runs the normal in-game dialogue and then enables the scene music.
/// Attach this to an object in the Intro scene, not to the persistent prefab.
/// </summary>
public class OpeningCinematicSequence : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private PlayableDirector _director;
    [Tooltip("A fullscreen Canvas shown only while the opening Timeline is playing.")]
    [SerializeField] private GameObject _cinematicCanvas;

    [Header("Opening Bench")]
    [SerializeField] private BenchCheckpoint _openingBench;

    [Header("Dialogue after the Timeline")]
    [SerializeField] private List<DialogueLine> _benchDialogue = new();

    [Header("Music after the bench dialogue")]
    [SerializeField] private SceneMusic _sceneMusic;

    private PlayerMovement _player;
    private bool _started;

    private void Start()
    {
        bool isNewGameLaunch =
            NewGameLaunchContext.TryConsume(out bool shouldPlay);

        // This covers Continue, opening Intro directly in the editor, and the
        // main-menu testing toggle that skips the whole opening.
        if (!isNewGameLaunch || !shouldPlay)
        {
            PauseMenu.SetPauseAllowed(true);
            _sceneMusic?.Play();
            return;
        }

        PauseMenu.SetPauseAllowed(false);

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        _player = playerObject != null
            ? playerObject.GetComponent<PlayerMovement>()
            : null;

        // The transition already freezes the player.  Keeping this lock here
        // prevents a one-frame input window before the Timeline begins.
        _player?.SetOpeningCinematicFrozen(
            true,
            ignoreGravity: true);
        _cinematicCanvas?.SetActive(true);

        // Do this behind the cinematic canvas so the player is fully seated
        // by the time the opening ends.
        _openingBench?.BeginOpeningSeat();
        StartCoroutine(BeginAfterSceneTransition());
    }

    private IEnumerator BeginAfterSceneTransition()
    {
        SceneTransitionManager transitionManager =
            SceneTransitionManager.Instance;

        if (transitionManager == null || !transitionManager.IsTransitioning)
        {
            StartTimeline();
            yield break;
        }

        // Start at the beginning of the existing fade-from-black rather than
        // after it. This lets the fade reveal Timeline's first frame directly.
        transitionManager.SceneRevealStarting += StartTimeline;

        while (!_started && transitionManager != null &&
               transitionManager.IsTransitioning)
        {
            yield return null;
        }

        if (_started)
            yield break;

        // Safe fallback for an unusual transition that finishes without the
        // reveal event (for example, a custom transition added later).
        transitionManager.SceneRevealStarting -= StartTimeline;
        StartTimeline();
    }

    private void StartTimeline()
    {
        if (_started)
            return;

        _started = true;

        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.SceneRevealStarting -= StartTimeline;

        if (_director == null)
        {
            Debug.LogError("Opening cinematic needs a PlayableDirector reference.", this);
            _cinematicCanvas?.SetActive(false);
            StartCoroutine(PlayBenchDialogueThenReleasePlayer());
            return;
        }

        _director.time = 0d;
        _director.Evaluate();
        _director.stopped += OnTimelineStopped;
        _director.Play();
    }

    private void OnTimelineStopped(PlayableDirector stoppedDirector)
    {
        if (stoppedDirector != _director)
            return;

        _director.stopped -= OnTimelineStopped;
        _cinematicCanvas?.SetActive(false);
        StartCoroutine(PlayBenchDialogueThenReleasePlayer());
    }

    private IEnumerator PlayBenchDialogueThenReleasePlayer()
    {
        if (_benchDialogue.Count > 0 && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(_benchDialogue);
            // DialogueManager normally freezes with gravity enabled. A seated
            // player needs the bench's no-gravity lock until they get up.
            if (_openingBench != null && _openingBench.IsSitting)
                _player?.SetFrozen(true, ignoreGravity: true);

            yield return new WaitUntil(() => DialogueManager.Instance.IsDialogueFinished);
        }

        _sceneMusic?.Play();

        // Stay seated after the dialogue. BenchCheckpoint will release this
        // normal movement lock through its existing get-up animation when the
        // player moves. The opening-specific lock can now safely be released.
        if (_openingBench != null && _openingBench.IsSitting)
            _player?.SetFrozen(true, ignoreGravity: true);
        else
            _player?.SetFrozen(false);

        _player?.SetOpeningCinematicFrozen(false);
        _openingBench?.ReleaseOpeningSeat();
        PauseMenu.SetPauseAllowed(true);
    }

    private void OnDestroy()
    {
        if (SceneTransitionManager.Instance != null)
            SceneTransitionManager.Instance.SceneRevealStarting -= StartTimeline;

        if (_director != null)
            _director.stopped -= OnTimelineStopped;

        _player?.SetOpeningCinematicFrozen(false);
        PauseMenu.SetPauseAllowed(true);
    }
}
