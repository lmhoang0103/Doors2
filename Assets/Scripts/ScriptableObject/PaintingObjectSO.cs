using UnityEngine;

[CreateAssetMenu(menuName = "ItemObject/PaintingObjectSO")]
public class PaintingObjectSO : ItemObjectSO
{

    public PaintingFrameType frameType;

    private void Reset()
    {
        itemType = ItemType.Painting;
    }
}
