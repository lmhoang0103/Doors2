using UnityEngine;

public class RandomBookInit : MonoBehaviour
{
    [SerializeField] private BookModelsSO bookModelsSO;
    [SerializeField] private Transform bookCombinationVisual;

    private void Awake()
    {
        int randomIndex = Random.Range(0, bookModelsSO.bookModels.Count);
        Transform choosenBookVisual = bookModelsSO.bookModels[randomIndex];
        Instantiate(choosenBookVisual, bookCombinationVisual);
    }
}
