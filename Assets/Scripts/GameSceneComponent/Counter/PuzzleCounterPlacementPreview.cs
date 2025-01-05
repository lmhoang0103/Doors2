using HighlightPlus;
using UnityEngine;

public class PuzzleCounterPlacementPreview : MonoBehaviour, IItemObjectParent {
    private HighlightEffect previewHighlightEffect;

    private ItemObject itemObject;

    [SerializeField] private PuzzleCounter puzzleCounter;


    private void Awake() {
        previewHighlightEffect = GetComponent<HighlightEffect>();
        puzzleCounter = GetComponentInParent<PuzzleCounter>();
    }
    private void Start() {

        Player.Instance.OnSelectedInteractableObjectChanged += Player_OnSelectedInteractableObjectChanged;
        Hide();

    }

    private void Player_OnSelectedInteractableObjectChanged(object sender, Player.OnSelectedInteractableObjectChangedEventArgs e) {
        if (e.interactable == (IInteractable)puzzleCounter && Player.Instance.HasItemObject() && !puzzleCounter.HasItemObject()) {
            Show();
        } else {
            Hide();
        }

    }



    private void Show() {
        if (Player.Instance.HasItemObject()) {
            ItemObject.SpawnItemObjectSO(Player.Instance.GetItemObject().GetItemObjectSO(), this);
            if (previewHighlightEffect != null) {
                previewHighlightEffect.SetHighlighted(true);
            }
            ResetGameOBject();
        }

    }

    private void Hide() {
        if (this.HasItemObject()) {
            this.GetItemObject().DestroySelf();
        }
        if (previewHighlightEffect != null) {
            previewHighlightEffect.SetHighlighted(false);
        }


    }

    public Transform GetItemObjectFollowTransform() {
        return this.transform;
    }

    public void SetItemObject(ItemObject itemObject) {
        this.itemObject = itemObject;
    }

    public ItemObject GetItemObject() {
        return itemObject;
    }

    public void ClearItemObject() {
        itemObject = null;
    }

    public bool HasItemObject() {
        return itemObject != null;
    }

    private void ResetGameOBject() {
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

}
