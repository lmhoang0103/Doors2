using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemObject/PaintingTypeSO")]
public class PaintingTypeSO : ScriptableObject
{

    public PaintingFrameType frameType;
    public List<PaintingObjectSO> paintingList;
}
