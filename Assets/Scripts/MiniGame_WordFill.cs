using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame_WordFill : Minigame
{
    [Header("UI References")]
    [SerializeField] private Image colorHintImage;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI timerText;

    [SerializeField] private Slider timerSlider;

    [Header("Game Settings")]
    [SerializeField] private float timeLimit = 15f;
    [SerializeField] private bool caseSensitive = false;

    [Header("Color Hints")]
    [SerializeField]
    private List<Color> phraseColors = new List<Color>
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta
    };

    [Header("Phrases")]
    [SerializeField]
    private List<string> possiblePhrases = new List<string>
    {
        "que fait l'ane au lac",
        "l'abeille coule",
        "la vague lave le quai",
        "la moule mord le mât",
        "la seiche sèche"
    };

    [Header("Typing SFX")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> typingClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> captainTalk = new List<AudioClip>();
    [SerializeField, Range(0.5f, 2f)] private float minPitch = 0.95f;
    [SerializeField, Range(0.5f, 2f)] private float maxPitch = 1.05f;

    private string currentTargetPhrase;
    private int currentPhraseIndex;
    private float timer;
    private bool hasValidated = false;

    private int lastInputLength = 0;

    public override void Init()
    {
        base.Init();

        timer = timeLimit;
        hasValidated = false;

        currentPhraseIndex = Random.Range(0, possiblePhrases.Count);
        currentTargetPhrase = possiblePhrases[currentPhraseIndex];

        SetupUI();
        AudioClip playedClip = captainTalk[Random.Range(0, captainTalk.Count)];
        audioSource.PlayOneShot(playedClip);
        Debug.Log($"Couleur affichée correspond à : {currentTargetPhrase}");
    }

    public override void Clear()
    {
        base.Clear();

        if (inputField != null)
        {
            inputField.onSubmit.RemoveListener(OnSubmit);
            inputField.onValueChanged.RemoveListener(OnInputChanged);

            inputField.text = "";
            inputField.interactable = false;
        }
    }

    private void SetupUI()
    {
        if (colorHintImage != null && currentPhraseIndex < phraseColors.Count)
        {
            colorHintImage.color = phraseColors[currentPhraseIndex];
        }

        if (inputField != null)
        {
            // Préparer l'input sans déclencher de SFX
            inputField.onSubmit.RemoveListener(OnSubmit);
            inputField.onValueChanged.RemoveListener(OnInputChanged);

            inputField.text = "";
            lastInputLength = 0;

            inputField.interactable = true;
            inputField.Select();
            inputField.ActivateInputField();

            inputField.onSubmit.AddListener(OnSubmit);
            inputField.onValueChanged.AddListener(OnInputChanged);
        }
    }

    void Update()
    {
        if (!isGameActive) return;

        timer -= Time.deltaTime;

        if (timerText != null)
        {
            timerText.text = $"Temps restant : {timer:F1}s";
        }

        if (timerSlider != null)
        {
            timerSlider.value = timer / timeLimit;
        }

        if (timer <= 0 && !hasValidated)
        {
            CompleteMinigame(false);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ValidateInput();
        }
    }

    private void OnSubmit(string submittedText)
    {
        ValidateInput();
    }

    public void ValidateInput()
    {
        if (!isGameActive || hasValidated) return;

        hasValidated = true;

        string userInput = inputField != null ? inputField.text : "";
        bool isCorrect = CompareStrings(userInput, currentTargetPhrase);

        Debug.Log($"Input: '{userInput}' | Target: '{currentTargetPhrase}' | Correct: {isCorrect}");

        CompleteMinigame(isCorrect);
    }

    private bool CompareStrings(string input, string target)
    {
        if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(target))
            return false;

        string processedInput = input;
        string processedTarget = target;

        if (!caseSensitive)
        {
            processedInput = processedInput.ToLower();
            processedTarget = processedTarget.ToLower();
        }

        return processedInput.Equals(processedTarget);
    }

    private void OnInputChanged(string newValue)
    {
        if (!isGameActive) { lastInputLength = newValue?.Length ?? 0; return; }
        if (string.IsNullOrEmpty(newValue))
        {
            lastInputLength = 0;
            return;
        }

        int newLen = newValue.Length;

        if (newLen > lastInputLength)
        {
            char lastChar = newValue[newLen - 1];

            if (char.IsLetter(lastChar) && audioSource != null && typingClips != null && typingClips.Count > 0)
            {
                var clip = typingClips[Random.Range(0, typingClips.Count)];
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.PlayOneShot(clip);
            }
        }

        lastInputLength = newLen;
    }

    void OnDestroy()
    {
        if (inputField != null)
        {
            inputField.onSubmit.RemoveListener(OnSubmit);
            inputField.onValueChanged.RemoveListener(OnInputChanged);
        }
    }
}
