using System;
using UnityEngine;

public class BasePuzzleCounter : BaseCounter
{
    private const string UNINTERACTABLE_LAYER_NAME = "Default";
    public event EventHandler OnPuzzleItemChecked;

    [SerializeField] private ItemObjectSO itemObjectSOToSpawn;

    private void Awake()
    {
        if (itemObjectSOToSpawn != null)
        {
            ItemObject.SpawnItemObjectSO(itemObjectSOToSpawn, this);
        }
    }
    public override bool IsInteractable(Player player)
    {
        if (!HasItemObject() && !player.HasItemObject()) return false;
        return true;
    }


    protected virtual bool IsCorrectPuzzleItemPlaced(ItemObjectSO itemObjectSO)
    {
        Debug.LogError("Not Implement Check Correct Item of Puzzle Counter");
        return false;
    }

    protected void HandlePuzzleCounterOnCorrect()
    {
        OnPuzzleItemChecked?.Invoke(this, EventArgs.Empty);
        gameObject.layer = LayerMask.NameToLayer(UNINTERACTABLE_LAYER_NAME);
    }
}
