using UnityEngine;

public class PuzzleCounter : BasePuzzleCounter
{
    [SerializeField] private ItemObjectSO requiredPuzzleItemObjectSO;
    public override void Interact(Player player)
    {
        if (!HasItemObject())
        {
            //There are no objects here
            if (player.HasItemObject())
            {
                //Player is carrying something
                player.GetItemObject().SetItemObjectParent(this);
                if (IsCorrectPuzzleItemPlaced(GetItemObject().GetItemObjectSO()))
                {
                    HandlePuzzleCounterOnCorrect();
                }
            } else
            {

            }
        } else
        {
            //There is an object here
            if (player.HasItemObject())
            {
                //This object is incorrect, because if it's correct, then player cannot interact with it
                //Player swap an item in
                ItemObject.SwapItemObject(player, this);
                if (IsCorrectPuzzleItemPlaced(GetItemObject().GetItemObjectSO()))
                {
                    //Player swap correct item in
                    HandlePuzzleCounterOnCorrect();
                } else
                {
                }

            } else
            {
                GetItemObject().SetItemObjectParent(player);
            }
        }
    }


    protected override bool IsCorrectPuzzleItemPlaced(ItemObjectSO itemObjectSO)
    {
        return (itemObjectSO == requiredPuzzleItemObjectSO);
    }
}
