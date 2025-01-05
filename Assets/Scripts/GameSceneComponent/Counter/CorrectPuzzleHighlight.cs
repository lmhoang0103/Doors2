using HighlightPlus;
using Sirenix.OdinInspector;
using UnityEngine;

public class CorrectPuzzleHighlight : MonoBehaviour {
    [ShowInInspector] private HighlightEffect correctPuzzleCounterHighlightEffect;
    [ShowInInspector] private PuzzleCounter puzzleCounter;

    private void Awake() {
        correctPuzzleCounterHighlightEffect = GetComponent<HighlightEffect>();
        puzzleCounter = transform.parent.GetComponent<PuzzleCounter>();

    }
    private void Start() {
        puzzleCounter.OnCorrectPuzzleItem += PuzzleCounter_OnCorrectPuzzleItem;
        Hide();
    }

    private void PuzzleCounter_OnCorrectPuzzleItem(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        correctPuzzleCounterHighlightEffect.SetHighlighted(true);
        ResetObject();
    }

    private void Hide() {
        correctPuzzleCounterHighlightEffect.SetHighlighted(false);

    }

    private void ResetObject() {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
