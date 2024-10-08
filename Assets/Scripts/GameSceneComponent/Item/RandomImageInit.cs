using UnityEngine;
using UnityEngine.UI;

public class RandomImageInit : HCMonoBehaviour
{

    [SerializeField] private PaintingImageSO paintingImageSO;
    [SerializeField] private Image image;


    private void Awake()
    {
        int randomIndex = Random.Range(0, paintingImageSO.sourceImages.Count);
        Sprite choosenImage = paintingImageSO.sourceImages[randomIndex];
        image.sprite = choosenImage;
    }

}
