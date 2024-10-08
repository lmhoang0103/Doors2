using HighlightPlus;
using UnityEngine;

public class CorrectPuzzleHighlight : MonoBehaviour
{
    [SerializeField] private HighlightEffect correctPuzzleCounterHighlightEffect;
    [SerializeField] private BasePuzzleCounter basePuzzleCounter;
    private void Start()
    {
        basePuzzleCounter.OnPuzzleItemChecked += PuzzleCounter_OnPuzzleItemChecked;
        Hide();
    }

    private void PuzzleCounter_OnPuzzleItemChecked(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        correctPuzzleCounterHighlightEffect.SetHighlighted(true);
        ResetObject();
    }

    private void Hide()
    {
        correctPuzzleCounterHighlightEffect.SetHighlighted(false);

    }

    private void ResetObject()
    {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
