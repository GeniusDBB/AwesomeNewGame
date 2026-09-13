using UnityEngine;

public interface IInteractable
{
    void Interact();
}

public interface IInteractionAvailability
{
    bool CanInteract { get; }
}