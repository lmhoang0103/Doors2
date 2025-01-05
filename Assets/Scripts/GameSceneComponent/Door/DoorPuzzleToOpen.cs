using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorPuzzleToOpen : BaseDoor, ITriggerableByPlayer, IHintable {
    public event EventHandler OnHintShow;
    public event EventHandler OnHintHidden;

    [SerializeField] private List<PuzzleCounter> puzzleCountersList;
    private int correctPuzzleItemsCount;

    protected override void Awake() {
        base.Awake();
    }

    private void Start() {
        correctPuzzleItemsCount = 0;


        foreach (PuzzleCounter puzzleCounter in puzzleCountersList) {
            puzzleCounter.OnCorrectPuzzleItem += PuzzleCounter_OnCorrectPuzzleItem;
        }
    }

    private void PuzzleCounter_OnCorrectPuzzleItem(object sender, EventArgs e) {
        correctPuzzleItemsCount++;
        // Check if all puzzles are correctly solved
        if (correctPuzzleItemsCount == puzzleCountersList.Count) {
            OpenDoor();
            OnHintHidden?.Invoke(this, EventArgs.Empty);

        }
    }


    public void Trigger(Player player) {
        OnHintShow?.Invoke(this, EventArgs.Empty);
    }
}

