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

    private string currentTargetPhrase;
    private int currentPhraseIndex;
    private float timer;
    private bool hasValidated = false;

    public override void Init()
    {
        base.Init();

        timer = timeLimit;
        hasValidated = false;

        currentPhraseIndex = Random.Range(0, possiblePhrases.Count);
        currentTargetPhrase = possiblePhrases[currentPhraseIndex];

        SetupUI();

        Debug.Log($"Couleur affichée correspond à : {currentTargetPhrase}");
    }

    public override void Clear()
    {
        base.Clear();

        if (inputField != null)
        {
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
            inputField.text = "";
            inputField.interactable = true;
            inputField.Select();
            inputField.ActivateInputField();
            inputField.onSubmit.AddListener(OnSubmit);
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

    void OnDestroy()
    {
        if (inputField != null)
        {
            inputField.onSubmit.RemoveListener(OnSubmit);
        }
    }
}
