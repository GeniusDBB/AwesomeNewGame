using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float _interactRadius = 1.2f;
    [SerializeField] private LayerMask _interactableLayer;

    [SerializeField] private Vector3 _iconOffset = new Vector3(0f, 1f, 0f);

    private IInteractable _currentInteractable;
    private Transform _currentInteractableTransform;

    private void Update()
    {
        FindClosestInteractable();

        if (_currentInteractable != null)
        {
            UIManager.Instance.UpdateInteractIconPosition(_currentInteractableTransform.position + _iconOffset);
        }

        if (InputManager.InteractWasPressed && _currentInteractable != null)
        {
            _currentInteractable.Interact();
        }
    }

    private void FindClosestInteractable()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _interactRadius, _interactableLayer);

        IInteractable closest = null;
        Transform closestTransform = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IInteractable>(out var interactable))
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = interactable;
                    closestTransform = hit.transform;
                }
            }
        }

        if (closest != _currentInteractable)
        {
            _currentInteractable = closest;
            _currentInteractableTransform = closestTransform;

            if (_currentInteractable != null)
                UIManager.Instance.ShowInteractPrompt(closestTransform.position + _iconOffset);
            else
                UIManager.Instance.HideInteractPrompt();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _interactRadius);
    }
}