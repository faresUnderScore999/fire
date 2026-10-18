using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private string prompt = "Appuyer sur E";
    [SerializeField] private bool singleUse = true;

    public UnityEvent OnInteracted = new UnityEvent();

    public string Prompt => prompt;
    public bool CanInteract { get; private set; } = true;

    public void Interact()
    {
        if (!CanInteract) return;
        if (singleUse) CanInteract = false;
        OnInteracted.Invoke();
    }
}