using UnityEngine;


[CreateAssetMenu(menuName = "ItemObject/ItemObjectSO")]
public class ItemObjectSO : ScriptableObject {
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
    public ItemType itemType;
    public bool isStackable;
}
