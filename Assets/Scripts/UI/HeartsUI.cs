using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class HeartsUI : MonoBehaviour
{
    [SerializeField] private GameObject _heartPrefab;
    [SerializeField] private Transform _heartsContainer;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Sprite _fullHeart;
    [SerializeField] private Sprite _emptyHeart;

    private PlayerHealth _playerHealth;
    private readonly List<UnityEngine.UI.Image> _heartImages = new();

    private void Awake()
    {
        SetVisible(SceneManager.GetActiveScene().name != "MainMenu");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isMainMenu = scene.name == "MainMenu";
        SetVisible(!isMainMenu);

        if (isMainMenu)
        {
            _playerHealth = null;
        }
    }

    private void SetVisible(bool visible)
    {
        _canvasGroup.alpha = visible ? 1f : 0f;
        _canvasGroup.interactable = visible;
        _canvasGroup.blocksRaycasts = visible;
    }

    private void Update()
    {
        if (_playerHealth == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj == null) return;

            if (playerObj.TryGetComponent<PlayerHealth>(out var health))
            {
                _playerHealth = health;
                _playerHealth.OnHealthChanged += UpdateHearts;
                UpdateHearts(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
            }
        }
    }

    private void UpdateHearts(int current, int max)
    {
        if (_heartImages.Count != max)
        {
            foreach (var heart in _heartImages) Destroy(heart.gameObject);
            _heartImages.Clear();

            for (int i = 0; i < max; i++)
            {
                GameObject heartObj = Instantiate(_heartPrefab, _heartsContainer);
                _heartImages.Add(heartObj.GetComponent<UnityEngine.UI.Image>());
            }
        }

        for (int i = 0; i < _heartImages.Count; i++)
        {
            _heartImages[i].sprite = i < current ? _fullHeart : _emptyHeart;
        }
    }
}