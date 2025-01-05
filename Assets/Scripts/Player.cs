using MEC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : SingletonDestroy<Player>, IItemObjectParent {

    private const float SPEED_BUFF_TIMER = 4f;
    private const string SPEED_BUFF_COROUTINE_TAG = "SpeedBuffCoroutineTag";

    public event EventHandler<OnSelectedInteractableObjectChangedEventArgs> OnSelectedInteractableObjectChanged;

    public class OnSelectedInteractableObjectChangedEventArgs : EventArgs {
        public IInteractable interactable;
    }

    public event EventHandler<OnInteractProgressChangedEventArgs> OnInteractProgressChanged;
    public class OnInteractProgressChangedEventArgs : EventArgs {
        public float progressNormalized;
    }

    public event EventHandler<OnPlayerItemHoldingChangedEventArgs> OnPlayerItemHoldingChanged;

    public class OnPlayerItemHoldingChangedEventArgs : EventArgs {
        public ItemObject itemObject;
    }


    [SerializeField] private Transform itemObjectHoldPoint;
    [SerializeField] private Transform playerCameraHolder;
    [SerializeField] private LayerMask interactablesLayerMask;
    [SerializeField] private LayerMask triggerLayerMask;
    [SerializeField] private LayerMask triggerObstacleLayerMask;
    [SerializeField] private float baseSpeed = 7f;
    [SerializeField] private float playerSizeRadius = 1f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private float triggerDistance = 2f;
    [SerializeField] private float interactTimerMax = 2f;
    [SerializeField] private float slippingTimerMax = 1f;
    [SerializeField] private float delayInteractMax = 1f;

    [Range(0.1f, 9f)][SerializeField] private float sensitivity = 2f;
    public float Sensitivity {
        get { return sensitivity; }
        set { sensitivity = value; }
    }
    [Range(0f, 90f)][SerializeField] private float yRotationLimit = 70f;

    private float moveSpeed = 1f;
    private Vector2 storedRotation = Vector2.zero;
    private IInteractable selectedInteractable;
    private float interactTimer;
    private float slippingTimer;
    private ItemObject holdingItemObject;
    private bool isCannotMove = false;
    private bool isRunning;
    private bool isKnockDown = false;
    private CoroutineHandle coroutineHandle;

    protected override void Awake() {
        base.Awake();
    }
    private void Start() {
        moveSpeed = baseSpeed;
    }

    private void Update() {
        HandleMovement();
        HandleInteraction();
        HandleTrigger();
        HandleTriggerOnFloor();
    }

    private void HandleMovement() {
        if (isCannotMove || DoorGameManager.Instance.IsGamePaused()) {
        } else {
            Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
            Vector3 moveDir = transform.forward * inputVector.y + transform.right * inputVector.x;
            isRunning = moveDir != Vector3.zero;
            float moveDistance = moveSpeed * Time.deltaTime;

            bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSizeRadius, moveDir, moveDistance);
            if (!canMove) {
                //Cannot move in this direction but attemp x movement
                Vector3 moveDirX = new(moveDir.x, 0f, 0f);

                canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSizeRadius, moveDirX, moveDistance);
                if (canMove) {
                    moveDir = moveDirX;
                } else {
                    //Cannot move in X direction, attempt Z movement
                    Vector3 moveDirZ = new(0f, 0f, moveDir.z);

                    canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSizeRadius, moveDirZ, moveDistance);
                    if (canMove) {
                        moveDir = moveDirZ;
                    } else {
                        //Cannnot move in any direction 
                    }

                }
            }
            if (canMove) {
                transform.position += moveDir * moveDistance;
            }


            storedRotation.x += GameInput.Instance.GetRotationVector().x * sensitivity;
            storedRotation.y += GameInput.Instance.GetRotationVector().y * sensitivity;

            //Convert angle to the (-180, 180) format
            storedRotation.x = Mathf.Repeat(storedRotation.x, 360f);
            if (storedRotation.x > 180f)
                storedRotation.x -= 360f;
            storedRotation.y = Mathf.Clamp(storedRotation.y, -yRotationLimit, yRotationLimit);

            var xQuat = Quaternion.AngleAxis(storedRotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(storedRotation.y, Vector3.left);
            transform.localRotation = xQuat;
            playerCameraHolder.transform.localRotation = yQuat;

        }
    }
    private void HandleInteraction() {
        Vector3 cameraDir = Camera.main.transform.forward;

        //Try to get the interactableObject before camera
        if (Physics.Raycast(Camera.main.transform.position, cameraDir, out RaycastHit rayCastHit, interactDistance, interactablesLayerMask)) {
            if (rayCastHit.transform.TryGetComponent(out IInteractable interactable)) {

                if (!interactable.IsInteractable(this)) {

                } else {
                    //Look at an object for required time to finish interaction
                    if (interactable != this.selectedInteractable) {
                        //different object
                        SetSelectedInteractable(interactable);
                    } else {
                        //same object
                        interactTimer += Time.deltaTime;
                        if (interactTimer > interactTimerMax) {
                            InteractWithSelectedInteractable();


                        }
                    }
                }

            } else {
                SetSelectedInteractable(null);
            }
        } else {
            SetSelectedInteractable(null);
        }

        OnInteractProgressChanged?.Invoke(this, new OnInteractProgressChangedEventArgs {
            progressNormalized = interactTimer / interactTimerMax
        });
    }
    private void HandleTrigger() {
        Vector3 cameraDir = Camera.main.transform.forward;
        if (Physics.Raycast(Camera.main.transform.position, cameraDir, out RaycastHit rayCastHit, triggerDistance, triggerLayerMask)) {
            if (rayCastHit.transform.TryGetComponent(out ITriggerableByPlayer triggerableByPlayer)) {
                //Hit a trigger
                triggerableByPlayer.Trigger(this);
            } else {

            }
        } else {
        }

    }

    private void HandleTriggerOnFloor() {
        float sphereRadius = 1f;
        Collider[] triggerColliders = Physics.OverlapSphere(transform.position, sphereRadius, triggerObstacleLayerMask);
        foreach (var triggerObject in triggerColliders) {
            if (triggerObject.transform.TryGetComponent(out ITriggerableByPlayer triggerableByPlayer)) {
                //hit a trigger
                triggerableByPlayer.Trigger(this);
            } else {

            }
        }

    }
    private void SetSelectedInteractable(IInteractable selectedInteractable) {
        interactTimer = 0f;
        this.selectedInteractable = selectedInteractable;
        OnSelectedInteractableObjectChanged?.Invoke(this, new OnSelectedInteractableObjectChangedEventArgs {
            interactable = selectedInteractable,
        });
    }

    private void InteractWithSelectedInteractable() {
        if (selectedInteractable != null) {
            selectedInteractable.Interact(this);
            OnSelectedInteractableObjectChanged?.Invoke(this, new OnSelectedInteractableObjectChangedEventArgs {
                interactable = selectedInteractable,
            });
        }
        interactTimer = -delayInteractMax;
    }

    public Vector3 GetPlayerPosition() {
        return transform.position;
    }

    public void OnSlippingMovement() {
        if (!isCannotMove) {
            float rotationAngle = 70f;
            float randomX = UnityEngine.Random.Range(-rotationAngle, rotationAngle);
            float randomY = UnityEngine.Random.Range(-rotationAngle, rotationAngle);
            var xQuat = Quaternion.AngleAxis(randomX, Vector3.up);
            var yQuat = Quaternion.AngleAxis(randomY, Vector3.left);

            Timing.RunCoroutine(_Slipping(xQuat, yQuat));
        }
    }


    private IEnumerator<float> _Slipping(Quaternion xQuat, Quaternion yQuat) {
        isKnockDown = true;
        isCannotMove = true;
        slippingTimer = 0f;
        var originalPlayerBodyRotation = transform.localRotation;
        var originalPlayerHeadRotation = playerCameraHolder.transform.localRotation;
        while (slippingTimer < slippingTimerMax) {
            slippingTimer += Time.deltaTime;
            float interpolationFactor = slippingTimer / slippingTimerMax;

            // Interpolate the rotations using Slerp
            Quaternion newRotation = Quaternion.Slerp(originalPlayerBodyRotation, xQuat, interpolationFactor);
            Quaternion newRotationCameraHolder = Quaternion.Slerp(originalPlayerHeadRotation, yQuat, interpolationFactor);

            // Apply the new rotations
            transform.localRotation = newRotation;
            playerCameraHolder.transform.localRotation = newRotationCameraHolder;

            yield return Timing.WaitForOneFrame; // Skip to the next frame
        }

        storedRotation = new Vector2(transform.localRotation.eulerAngles.y, -playerCameraHolder.transform.localRotation.eulerAngles.x);
        //Convert angle to the (-180, 180) format
        storedRotation.y %= 360f;
        if (storedRotation.y < -180f)
            storedRotation.y += 360f;
        else if (storedRotation.y > 180f)
            storedRotation.y -= 360f;
        storedRotation.x = Mathf.Repeat(storedRotation.x, 360f);
        if (storedRotation.x > 180f)
            storedRotation.x -= 360f;

        isKnockDown = false;
        yield return Timing.WaitForSeconds(0.5f);
        isCannotMove = false;
    }

    public void ApplySpeedUp(int speedPercentageIncrease) {
        //Run ApplySpeedUp function
        coroutineHandle = Timing.RunCoroutineSingleton(_SpeedUpCoroutine(speedPercentageIncrease), coroutineHandle, SingletonBehavior.Overwrite);

    }

    private IEnumerator<float> _SpeedUpCoroutine(float speedPercentageIncrease) {
        moveSpeed = baseSpeed * (1 + (speedPercentageIncrease / 100f));
        //Wait for certain time
        yield return Timing.WaitForSeconds(SPEED_BUFF_TIMER);
        moveSpeed = baseSpeed;
    }


    public bool IsRunning() {
        return isRunning;
    }
    public bool IsKnockDown() { return isKnockDown; }

    public Transform GetItemObjectFollowTransform() {
        return itemObjectHoldPoint;
    }

    public void SetItemObject(ItemObject itemObject) {
        this.holdingItemObject = itemObject;
        OnPlayerItemHoldingChanged?.Invoke(this, new OnPlayerItemHoldingChangedEventArgs { itemObject = holdingItemObject });
    }

    public ItemObject GetItemObject() {
        return holdingItemObject;
    }

    public void ClearItemObject() {
        holdingItemObject = null;

        OnPlayerItemHoldingChanged?.Invoke(this, new OnPlayerItemHoldingChangedEventArgs { itemObject = holdingItemObject });
    }

    public bool HasItemObject() {
        return holdingItemObject != null;
    }
}

