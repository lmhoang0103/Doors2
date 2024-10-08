using UnityEngine;

public class PaintingFrameHolder : BasePuzzleCounter
{
    [SerializeField] private PaintingFrameType requiredPaintingFrameType;

    public override bool IsInteractable(Player player)
    {
        if (!HasItemObject() && !player.HasItemObject())
        {
            return false;
        } else if (player.HasItemObject())
        {
            if (player.GetItemObject().GetItemObjectSO().itemType != ItemType.Painting)
            {
                return false;
            }
        }
        return true;
    }
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
        if (!(itemObjectSO is PaintingObjectSO paintingObjectSO))
        {
            return false;
        } else if (paintingObjectSO.frameType == requiredPaintingFrameType)
        {
            return true;
        }
        return false;
    }
}
