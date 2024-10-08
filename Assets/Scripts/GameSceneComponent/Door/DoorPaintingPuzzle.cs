using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorPaintingPuzzle : BaseDoor, ITriggerableByPlayer, IHintable
{
    public event EventHandler OnHintShow;
    public event EventHandler OnHintHidden;

    [SerializeField] private List<PaintingFrameHolder> paintingFrameHolderList;
    private int correctPuzzleItemsCount;


    private void Start()
    {
        correctPuzzleItemsCount = 0;
        foreach (PaintingFrameHolder paintingFrameHolder in paintingFrameHolderList)
        {
            paintingFrameHolder.OnPuzzleItemChecked += PuzzleCounter_OnPuzzleItemChecked;
        }
    }

    private void PuzzleCounter_OnPuzzleItemChecked(object sender, EventArgs e)
    {
        correctPuzzleItemsCount++;
        // Check if all puzzles are correctly solved
        if (correctPuzzleItemsCount == paintingFrameHolderList.Count)
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
