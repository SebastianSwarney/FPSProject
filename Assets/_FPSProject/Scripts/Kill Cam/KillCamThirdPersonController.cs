using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCamThirdPersonController : MonoBehaviour
{
    #region Camera Properties
    [System.Serializable]
    public struct CameraProperties
    {
        public float m_mouseSensitivity;
        public float m_maxCameraAng;
        public bool m_inverted;
        public Camera m_camera;
        public Transform m_cameraMain;
    }

    [Header("Camera Properties")]
    public CameraProperties m_cameraProperties;
    #endregion

    #region Base Movement Properties
    [System.Serializable]
    public struct BaseMovementProperties
    {
        public float m_baseMovementSpeed;
        public float m_accelerationTime;
    }

    [Header("Base Movement Properties")]
    public BaseMovementProperties m_baseMovementProperties;

    private Vector3 m_velocity;
    private Vector3 m_velocitySmoothing;
    private CharacterController m_characterController;
    #endregion

    private Vector3 m_movementInput;
    private Vector2 m_lookInput;

    public bool m_rotateLook;

    public Transform m_lookRotateTransform;

    private void Start()
    {
        LockCursor();

        m_characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PerformController();
    }

    private void LateUpdate()
    {
        if (!m_rotateLook)
        {
            CameraRotation();
        }
        else
        {
            RotateLook();
        }
    }

    public void PerformController()
    {
        CalculateVelocity();

        m_characterController.Move(m_velocity * Time.deltaTime);
    }

    private void RotateLook()
    {
        Vector3 dirToTarget = (m_lookRotateTransform.position - transform.position);

        m_cameraProperties.m_camera.transform.rotation = Quaternion.LookRotation(dirToTarget);
    }

    public void SetMovementInput(Vector3 p_input)
    {
        m_movementInput = p_input;
    }

    public void SetLookInput(Vector2 p_input)
    {
        m_lookInput = p_input;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void CameraRotation()
    {
        //Get the inputs for the camera
        Vector2 cameraInput = new Vector2(m_lookInput.y * ((m_cameraProperties.m_inverted) ? -1 : 1), m_lookInput.x);

        //Rotate the player on the y axis (left and right)
        transform.Rotate(Vector3.up, cameraInput.y * (m_cameraProperties.m_mouseSensitivity));

        float xRotateAmount = cameraInput.x * m_cameraProperties.m_mouseSensitivity;

        RotateCameraAxisX(xRotateAmount, m_cameraProperties.m_cameraMain, m_cameraProperties.m_maxCameraAng);
    }

    public void RotateCameraAxisX(float p_rotateAmount, Transform p_targetTransform, float p_maxAngleOnAxis)
    {
        float cameraXAng = p_targetTransform.eulerAngles.x;

        //Stops the camera from rotating, if it hits the resrictions
        if (p_rotateAmount < 0 && cameraXAng > 360 - p_maxAngleOnAxis || p_rotateAmount < 0 && cameraXAng < p_maxAngleOnAxis + 10)
        {
            p_targetTransform.Rotate(Vector3.right, p_rotateAmount);

        }
        else if (p_rotateAmount > 0 && cameraXAng > 360 - p_maxAngleOnAxis - 10 || p_rotateAmount > 0 && cameraXAng < p_maxAngleOnAxis)
        {
            p_targetTransform.Rotate(Vector3.right, p_rotateAmount);
        }

        if (p_targetTransform.localEulerAngles.x < 360 - p_maxAngleOnAxis && p_targetTransform.localEulerAngles.x > 180)
        {
            p_targetTransform.localEulerAngles = new Vector3(360 - p_maxAngleOnAxis, 0f, 0f);
        }
        else if (p_targetTransform.localEulerAngles.x > p_maxAngleOnAxis && p_targetTransform.localEulerAngles.x < 180)
        {
            p_targetTransform.localEulerAngles = new Vector3(p_maxAngleOnAxis, 0f, 0f);
        }
    }

    private void CalculateVelocity()
    {
        Vector3 forwardMovement = m_cameraProperties.m_camera.transform.forward * m_movementInput.y;
        Vector3 rightMovement = m_cameraProperties.m_camera.transform.right * m_movementInput.x;
        Vector3 verticalMovement = Vector3.up * m_movementInput.z;

        Vector3 targetHorizontalMovement = Vector3.zero;

        targetHorizontalMovement = Vector3.ClampMagnitude(forwardMovement + rightMovement + verticalMovement, 1.0f) * m_baseMovementProperties.m_baseMovementSpeed;

        Vector3 horizontalMovement = Vector3.SmoothDamp(m_velocity, targetHorizontalMovement, ref m_velocitySmoothing, m_baseMovementProperties.m_accelerationTime);

        m_velocity = horizontalMovement;

    }

}
