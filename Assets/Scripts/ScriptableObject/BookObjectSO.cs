using UnityEngine;

[CreateAssetMenu(menuName = "ItemObject/BookObjectSO")]
public class BookObjectSO : ItemObjectSO
{
    public BookCoverType bookCoverType;
    private void Reset()
    {
        itemType = ItemType.Book;
    }
}
