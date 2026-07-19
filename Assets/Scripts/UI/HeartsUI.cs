using UnityEngine;
using System.Collections.Generic;

public class HeartsUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private GameObject _heartPrefab; // a UI Image prefab
    [SerializeField] private Transform _heartsContainer; // horizontal layout group parent
    [SerializeField] private Sprite _fullHeart;
    [SerializeField] private Sprite _emptyHeart;

    private readonly List<UnityEngine.UI.Image> _heartImages = new();

    private void OnEnable()
    {
        _playerHealth.OnHealthChanged += UpdateHearts;
    }

    private void OnDisable()
    {
        _playerHealth.OnHealthChanged -= UpdateHearts;
    }

    private void UpdateHearts(int current, int max)
    {
        // build hearts if count changed (first time, or max health changed)
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