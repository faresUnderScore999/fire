using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Interactable))]
public class EmergencyPhone : MonoBehaviour
{
    [Serializable]
    public class Option
    {
        public string number;
        public string label;
        public bool correct;

        public Option(string number, string label, bool correct)
        {
            this.number = number;
            this.label = label;
            this.correct = correct;
        }
    }

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text feedbackText;
    [SerializeField] private Button[] buttons;                 // same order as options

    [Header("Numbers")]
    [SerializeField]
    private Option[] options =
    {
        new Option("197", "Police", false),
        new Option("198", "Protection civile", true),
        new Option("190", "SAMU", false)
    };

    [Header("Messages")]
    [SerializeField] private string question = "Quel numéro composez-vous ?";
    [SerializeField] private float callDuration = 2f;
    [SerializeField] private Color okColor = new Color(0.5f, 0.9f, 0.55f);
    [SerializeField] private Color wrongColor = new Color(1f, 0.47f, 0.4f);

    public int Errors { get; private set; }
    public bool CallCompleted_ { get; private set; }
    public event Action CallCompleted;
    public event Action<string> WrongNumberDialed;

    private Interactable interactable;
    private PlayerMove player;
    private FireManager fireManager;
    private bool isOpen;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.OnInteracted.AddListener(Open);

        player = FindAnyObjectByType<PlayerMove>();
        fireManager = FindAnyObjectByType<FireManager>();

        // Write the labels and hook up the buttons
        for (int i = 0; i < buttons.Length && i < options.Length; i++)
        {
            int index = i;   // captured by the lambda below
            TMP_Text label = buttons[i].GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = $"{options[i].number}  -  {options[i].label}";
            buttons[i].onClick.AddListener(() => Dial(index));
        }

        if (panel != null) panel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (interactable != null) interactable.OnInteracted.RemoveListener(Open);
    }

    private void Update()
    {
        if (!isOpen) return;

        // The end screen takes over: just hide the panel, don't give control back
        if (fireManager != null && fireManager.IsFinished)
        {
            isOpen = false;
            panel.SetActive(false);
            return;
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) Close();
    }

    public void Open()
    {
        if (isOpen || CallCompleted_) return;
        isOpen = true;

        player.ControlsEnabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SetButtonsInteractable(true);
        ShowFeedback(question, Color.white);
        panel.SetActive(true);
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        panel.SetActive(false);

        if (fireManager != null && fireManager.IsFinished) return;

        player.ControlsEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Dial(int index)
    {
        if (CallCompleted_) return;
        Option option = options[index];

        if (option.correct)
        {
            StartCoroutine(CompleteCall(option));
            return;
        }

        Errors++;
        WrongNumberDialed?.Invoke(option.number);
        ShowFeedback($"{option.number} ({option.label}) ne gère pas les incendies. Réessayez.", wrongColor);
        Debug.Log($"Wrong number dialed: {option.number} (errors: {Errors})");
    }

    private IEnumerator CompleteCall(Option option)
    {
        SetButtonsInteractable(false);
        ShowFeedback($"Appel au {option.number}...", Color.white);
        yield return new WaitForSeconds(callDuration);

        CallCompleted_ = true;
        interactable.CanInteract = false;     // the prompt disappears for good
        ShowFeedback($"{option.label} prévenue.", okColor);
        yield return new WaitForSeconds(1f);

        CallCompleted?.Invoke();
        Close();
    }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;
        feedbackText.text = message;
        feedbackText.color = color;
    }

    private void SetButtonsInteractable(bool value)
    {
        foreach (Button b in buttons) b.interactable = value;
    }
}