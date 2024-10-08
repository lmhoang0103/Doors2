using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ItemObject/BookComponentSO")]
public class BookComponentSO : ScriptableObject
{
    public List<Transform> bookModels;
}
