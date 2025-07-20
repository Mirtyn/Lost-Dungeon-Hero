using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput
{
    private PlayerInputActions _playerInputActions;
    private bool _inputEnabled;

    public bool InputEnabled
    {
        get
        {
            return _inputEnabled;
        }
        private set
        {
            if (_inputEnabled != value)
            {
                if (value)
                {
                    _playerInputActions.Enable();
                }
                else
                {
                    _playerInputActions.Disable();
                }

                _inputEnabled = value;
            }
        }
    }

    public PlayerInput()
    {
        _playerInputActions = new PlayerInputActions();
        SetInputAvailability(true);
    }

    public void SetInputAvailability(bool enable)
    {
        InputEnabled = enable;
        //_playerInputActions.Player.Movement.performed += Movement_performed;
    }

    private void Movement_performed(InputAction.CallbackContext obj)
    {
        throw new System.NotImplementedException();
    }

    public Vector2 GetMouseScreenSpacePosition()
    {
        return Input.mousePosition;
        //return Mouse.current.position.value;
    }

    public Vector2 GetMouseWorldSpacePosition2D(Vector2 screenPos)
    {
        return GameManager.Instance.MainCamera.ScreenToWorldPoint(screenPos);
    }

    public bool GetMouseWorldSpacePosition3D(out Vector3 hitPosition, Vector2 screenPos, float maxDistance, int layerMask)
    {
        if (GetMouseWorldSpacePosition3D(out RaycastHit hitInfo, screenPos, maxDistance, layerMask))
        {
            hitPosition = hitInfo.point;
            return true;
        }

        hitPosition = Vector3.zero;
        return false;
    }

    public bool GetMouseWorldSpacePosition3D(out RaycastHit hitInfo, Vector2 screenPos, float maxDistance, int layerMask)
    {
        var ray = GameManager.Instance.MainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
        {
            hitInfo = hit;
            return true;
        }

        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red, 1f);

        hitInfo = new RaycastHit();
        return false;
    }

    //public bool GetLeftMouseButtonDown()
    //{
    //    return _playerInputActions.Player.LeftMouseButton.WasPressedThisFrame();
    //}

    //public bool GetLeftMouseButton()
    //{
    //    return _playerInputActions.Player.LeftMouseButton.IsInProgress();
    //}

    //public bool GetLeftMouseButtonUp()
    //{
    //    return _playerInputActions.Player.LeftMouseButton.WasReleasedThisFrame();
    //}

    //public void Enable()
    //{
    //    _playerInputActions.Enable();
    //}
    //public void Disable()
    //{
    //    _playerInputActions.Disable();
    //}

    //public void DisposeInput()
    //{
    //    _playerInputActions.Dispose();
    //}
}
