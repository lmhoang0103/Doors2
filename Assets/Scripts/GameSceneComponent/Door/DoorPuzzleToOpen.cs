using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorPuzzleToOpen : BaseDoor, ITriggerableByPlayer, IHintable
{
    public event EventHandler OnHintShow;
    public event EventHandler OnHintHidden;

    [SerializeField] private List<BasePuzzleCounter> puzzleCountersList;
    private int correctPuzzleItemsCount;


    private void Start()
    {
        correctPuzzleItemsCount = 0;
        foreach (BasePuzzleCounter puzzleCounter in puzzleCountersList)
        {
            puzzleCounter.OnPuzzleItemChecked += PuzzleCounter_OnPuzzleItemChecked;
        }
    }

    private void PuzzleCounter_OnPuzzleItemChecked(object sender, EventArgs e)
    {
        correctPuzzleItemsCount++;
        // Check if all puzzles are correctly solved
        if (correctPuzzleItemsCount == puzzleCountersList.Count)
        {
            OpenDoor();
            OnHintHidden?.Invoke(this, EventArgs.Empty);

        }
    }


    public void Trigger(Player player)
    {
        OnHintShow?.Invoke(this, EventArgs.Empty);
    }
}

