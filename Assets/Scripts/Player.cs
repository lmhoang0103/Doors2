using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;
public class Player : SingletonDestroy<Player>, IItemObjectParent
{

    public event EventHandler<OnSelectedInteractableObjectChangedEventArgs> OnSelectedInteractableObjectChanged;

    public class OnSelectedInteractableObjectChangedEventArgs : EventArgs
    {
        public IInteractable interactable;
    }

    public event EventHandler<OnInteractProgressChangedEventArgs> OnInteractProgressChanged;
    public class OnInteractProgressChangedEventArgs : EventArgs
    {
        public float progressNormalized;
    }


    [SerializeField] private Transform itemObjectHoldPoint;
    [SerializeField] private Transform playerCameraHolder;
    [SerializeField] private LayerMask interactablesLayerMask;
    [SerializeField] private LayerMask triggerLayerMask;
    [SerializeField] private LayerMask triggerObstacleLayerMask;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float playerSizeRadius = 1f;
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private float triggerDistance = 2f;
    [SerializeField] private float interactTimerMax = 2f;
    [SerializeField] private float slippingTimerMax = 1f;
    [SerializeField] private float delayInteractMax = 1f;

    [Range(0.1f, 9f)][SerializeField] private float sensitivity = 2f;
    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }
    [Range(0f, 90f)][SerializeField] private float yRotationLimit = 70f;

    private Vector2 storedRotation = Vector2.zero;
    private IInteractable selectedInteractable;
    private float interactTimer;
    private float slippingTimer;
    private ItemObject itemObject;
    private bool isSlipping = false;

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
        HandleTrigger();
        HandleTriggerOnFloor();
    }

    private void HandleMovement()
    {
        if (isSlipping)
        {
        } else
        {
            Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
            Vector3 moveDir = transform.forward * inputVector.y + transform.right * inputVector.x;
            float moveDistance = moveSpeed * Time.deltaTime;

            bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSizeRadius, moveDir, moveDistance);
            if (!canMove)
            {
                //Cannot move in this direction but attemp x movement
                Vector3 moveDirX = new(moveDir.x, 0f, 0f);

                canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSizeRadius, moveDirX, moveDistance);
                if (canMove)
                {
                    moveDir = moveDirX;
                } else
                {
                    //Cannot move in X direction, attempt Z movement
                    Vector3 moveDirZ = new(0f, 0f, moveDir.z);

                    canMove = moveDir.z != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerSizeRadius, moveDirZ, moveDistance);
                    if (canMove)
                    {
                        moveDir = moveDirZ;
                    } else
                    {
                        //Cannnot move in any direction 
                    }

                }
            }
            if (canMove)
            {
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
    private void HandleInteraction()
    {
        Vector3 cameraDir = playerCameraHolder.forward;

        if (Physics.Raycast(playerCameraHolder.position, cameraDir, out RaycastHit rayCastHit, interactDistance, interactablesLayerMask))
        {
            if (rayCastHit.transform.TryGetComponent(out IInteractable interactable))
            {

                if (!interactable.IsInteractable(this))
                {

                } else
                {
                    if (interactable != this.selectedInteractable)
                    {
                        //different object
                        interactTimer = 0f;
                        SetSelectedInteractable(interactable);
                    } else
                    {
                        //same object
                        interactTimer += Time.deltaTime;
                        SetSelectedInteractable(interactable);
                        if (interactTimer > interactTimerMax)
                        {
                            InteractWithSelectedInteractable();


                        }
                    }
                }

            } else
            {
                interactTimer = 0f;
                SetSelectedInteractable(null);
            }
        } else
        {
            interactTimer = 0f;

            SetSelectedInteractable(null);
        }

        OnInteractProgressChanged?.Invoke(this, new OnInteractProgressChangedEventArgs
        {
            progressNormalized = interactTimer / interactTimerMax
        });
    }
    private void HandleTrigger()
    {
        Vector3 cameraDir = playerCameraHolder.forward;
        if (Physics.Raycast(playerCameraHolder.position, cameraDir, out RaycastHit rayCastHit, triggerDistance, triggerLayerMask))
        {
            if (rayCastHit.transform.TryGetComponent(out ITriggerableByPlayer triggerableByPlayer))
            {
                //Hit a trigger
                triggerableByPlayer.Trigger(this);
            } else
            {

            }
        } else
        {
        }

    }

    private void HandleTriggerOnFloor()
    {
        float sphereRadius = 1f;
        Collider[] triggerColliders = Physics.OverlapSphere(transform.position, sphereRadius, triggerObstacleLayerMask);
        foreach (var triggerObject in triggerColliders)
        {
            if (triggerObject.transform.TryGetComponent(out ITriggerableByPlayer triggerableByPlayer))
            {
                //hit a trigger
                triggerableByPlayer.Trigger(this);
            } else
            {

            }
        }

    }
    private void SetSelectedInteractable(IInteractable selectedInteractable)
    {
        this.selectedInteractable = selectedInteractable;
        OnSelectedInteractableObjectChanged?.Invoke(this, new OnSelectedInteractableObjectChangedEventArgs
        {
            interactable = selectedInteractable,
        });
    }

    private void InteractWithSelectedInteractable()
    {
        if (selectedInteractable != null)
        {
            selectedInteractable.Interact(this);
        }
        interactTimer = -delayInteractMax;
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public void OnSlippingMovement()
    {
        if (!isSlipping)
        {
            float rotationAngle = 70f;
            float randomX = UnityEngine.Random.Range(-rotationAngle, rotationAngle);
            float randomY = UnityEngine.Random.Range(-rotationAngle, rotationAngle);
            var xQuat = Quaternion.AngleAxis(randomX, Vector3.up);
            var yQuat = Quaternion.AngleAxis(randomY, Vector3.left);

            Timing.RunCoroutine(_Slipping(xQuat, yQuat));
        }
    }

    private IEnumerator<float> _Slipping(Quaternion xQuat, Quaternion yQuat)
    {
        isSlipping = true;
        slippingTimer = 0f;
        var originalPlayerBodyRotation = transform.localRotation;
        var originalPlayerHeadRotation = playerCameraHolder.transform.localRotation;
        while (slippingTimer < slippingTimerMax)
        {
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

        yield return Timing.WaitForSeconds(0.5f);
        isSlipping = false;
    }

    public Transform GetItemObjectFollowTransform()
    {
        return itemObjectHoldPoint;
    }

    public void SetItemObject(ItemObject itemObject)
    {
        this.itemObject = itemObject;
    }

    public ItemObject GetItemObject()
    {
        return itemObject;
    }

    public void ClearItemObject()
    {
        itemObject = null;
    }

    public bool HasItemObject()
    {
        return itemObject != null;
    }
}

