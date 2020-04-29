using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class PlayerControllerEvent : UnityEvent { }

public class PlayerController : MonoBehaviour
{
    #region Player States
    public enum MovementControllState { MovementEnabled, MovementDisabled }
    public enum GravityState { GravityEnabled, GravityDisabled }

    [System.Serializable]
    public struct PlayerState
    {
        public MovementControllState m_movementControllState;
        public GravityState m_gravityControllState;
    }

    public PlayerState m_states;
    #endregion

    #region Player Events
    public PlayerControllerEvents m_events;
    [System.Serializable]
    public struct PlayerControllerEvents
    {
        [Header("Basic Events")]
        public PlayerControllerEvent m_onLandedEvent;
        public PlayerControllerEvent m_onJumpEvent;
        public PlayerControllerEvent m_onRespawnEvent;

        [Header("Wall Run Events")]
        public PlayerControllerEvent m_onWallRunBeginEvent;
        public PlayerControllerEvent m_onWallRunEndEvent;
        public PlayerControllerEvent m_onWallRunJumpEvent;

        [Header("Wall Climb Events")]
        public PlayerControllerEvent m_onWallClimbBeginEvent;
        public PlayerControllerEvent m_onWallClimbEndEvent;
        public PlayerControllerEvent m_onWallClimbJumpEvent;

        [Header("Wall Jump Events")]
        public PlayerControllerEvent m_onWallJumpEvent;

        [Header("Leap Events")]
        public PlayerControllerEvent m_onLeapEvent;

    }
    #endregion

    #region Camera Properties
    [System.Serializable]
    public struct CameraProperties
    {
        public float m_mouseSensitivity;
        public float m_maxCameraAng;
        public bool m_inverted;
        public Camera m_camera;
        public Transform m_cameraTilt;
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
        public float m_crouchMovementSpeed;
        public float m_accelerationTimeGrounded;
        public float m_accelerationTimeAir;
        public float m_slopeFriction;
    }

    [Header("Base Movement Properties")]
    public BaseMovementProperties m_baseMovementProperties;

    private float m_currentMovementSpeed;
    [HideInInspector]
    public Vector3 m_velocity;
    private Vector3 m_velocitySmoothing;
    private CharacterController m_characterController;
    private Coroutine m_jumpBufferCoroutine;
    private Coroutine m_graceBufferCoroutine;
    #endregion

    #region Jumping Properties
    [System.Serializable]
    public struct JumpingProperties
    {
        [Header("Jump Properties")]
        public float m_maxJumpHeight;
        public float m_minJumpHeight;
        public float m_timeToJumpApex;

        [Header("Jump Buffer Properties")]
        public float m_graceTime;
        public float m_jumpBufferTime;
    }

    [Header("Jumping Properties")]
    public JumpingProperties m_jumpingProperties;

    private bool m_hasJumped;

    private float m_graceTimer;
    private float m_jumpBufferTimer;

    private float m_gravity;
    private float m_maxJumpVelocity;
    private float m_minJumpVelocity;
    private bool m_isLanded;
    private bool m_offLedge;
    #endregion

    #region Wall Run Properties
    [System.Serializable]
    public struct WallRunProperties
    {
        [Header("Basic Wall Run Properties")]
        public float m_wallRunForwardSpeed;
        public float m_wallRunForwardAccelerationTime;

        public float m_wallRunTime;
        public float m_wallRunEndingDuration;

        public float m_wallRunStickSpeed;
        public float m_wallRunStickAccelerationTime;

        public float m_wallRunBufferTime;

        public Transform m_wallMovementTransform;

        public float m_wallMidpointAngle;

        public float m_wallRunYVelocityStopTime;

        public AnimationCurve m_wallYVelocitySlowdownCurve;

        [Header("Wall Run Jump Properties")]
        public Vector3 m_wallRunJumpForce;
        public float m_wallRunJumpGroundVelocityDecayTime;
        public AnimationCurve m_wallRunJumpGroundVelocityDecayAnimationCurve;

        [Header("Camera Tilt Properties")]
        public float m_tiltSpeed;
        public float m_wallRunCameraMaxTilt;
    }

    [Header("Wall Run Properties")]
    public WallRunProperties m_wallRunProperties;
    public LayerMask m_wallConnectMask;
    public float m_wallRaycastLength;

    private float m_tiltTarget;
    private bool m_isWallRunning;

    private Vector3 m_wallRunJumpVelocity;
    private Coroutine m_wallRunBufferCoroutine;
    private float m_wallRunBufferTimer;
    #endregion

    #region Wall Climb Properties
    [System.Serializable]
    public struct WallClimbProperties
    {
        public AnimationCurve m_wallClimbSpeedCurve;
        public float m_maxWallClimbSpeed;
        public float m_wallClimbSpeedUpTime;
        public float m_wallClimbFactor;
        public Vector3 m_wallClimbJumpVelocity;
    }

    [Header("Wall Climb Properties")]
    public WallClimbProperties m_wallClimbProperties;
    #endregion

    #region Crouch Properties
    [System.Serializable]
    public struct CrouchProperties
    {
        public float m_crouchTime;
        public float m_crouchHeight;
    }

    [Header("Crouch Properties")]
    public CrouchProperties m_crouchProperties;

    private bool m_isCrouched;
    private bool m_isCrouching;
    #endregion

    #region Slide Properties
    [System.Serializable]
    public struct SlideProperties
    {
        [Header("Basic Slide Properties")]
        public float m_slideSpeed;
        public float m_slideTime;
        public AnimationCurve m_slideCurve;
        public float m_slideCooldownTime;

        [Header("Slope Slide Properties")]
        public float m_slideAngleBoostMax;
        public float m_slopeSlideAccelerationTime;
        public float m_slopeTolerence;

        [Header("Slope Side Shift Properties")]
        public float m_slideSideShiftMaxSpeed;
        public float m_slideSideShiftAcceleration;
        public Transform m_slopeTransform;
    }

    [Header("Slide Properties")]
    public SlideProperties m_slideProperties;

    private bool m_isSliding;
    private float m_slideTimer;
    private Vector3 m_slideVelocity;
    private Vector3 m_slopeVelocity;
    private Vector3 m_slopeShiftVelocity;

    private Coroutine m_slideCooldownCoroutine;
    private float m_slideCooldownTimer;
    #endregion

    #region Grapple Properties
    [System.Serializable]
    public struct GrappleProperties
    {
        [Header("Basic Grapple Properties")]
        public LayerMask m_grappleWallMask;
        public float m_maxGrappleDistance;
        public float m_grappleSpeedMax;
        public float m_grappleSpeedupTime;
        public AnimationCurve m_grappleSpeedupCurve;

        [Header("Grapple Shift Properties")]
        public float m_grappleShiftAcceleration;
        public float m_grappleShiftMaxSpeed;
        public float m_grappleTolerence;
        public Transform m_grappleTransform;

        [Header("Grapple Visual Properties")]
        public LineRenderer m_grappleRopeVisual;
        public float m_grappleVisualTravelTime;
        public AnimationCurve m_grappleVisualTravelCurve;
    }

    [Header("Grapple Properties")]
    public GrappleProperties m_grappleProperties;

    private bool m_isGrappling;
    private float m_currentPostGrappleSpeedBoost;
    #endregion

    #region Clamber Properties
    public LayerMask m_wallClamberMask;

    public float m_clamberSpeed;

    private bool m_isClambering;

    public float m_climbSpeed;
    public float m_climbTime;

    public AnimationCurve m_climbSpeedDecreaseCurve;

    private bool m_isClimbing;

    private bool m_canWallClimb;

    public AnimationCurve m_postClamberSpeedBoostDecay;

    public float m_postClamberSpeedBoost;
    public float m_postClamberSpeedBoostTime;
    #endregion

    [HideInInspector]
    public Vector2 m_movementInput;
    private Vector2 m_lookInput;

    private bool m_maintainSpeed;

    public Vector2 m_slideJumpForce;

    private Vector3 m_wallJumpDir;

    private bool m_crouchOnLanding;

    private ControllerColliderHit m_lastWallHit;



    private Vector3 m_wallVelocity;
    private Vector3 m_wallStickVelocity;

    
    private Vector3 m_wallJumpVector;

    private void Start()
    {
        m_characterController = GetComponent<CharacterController>();

        CalculateJump();

        m_currentMovementSpeed = m_baseMovementProperties.m_baseMovementSpeed;
        m_jumpBufferTimer = m_jumpingProperties.m_jumpBufferTime;

        m_wallRunBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_wallRunBufferTimer = (x), m_wallRunProperties.m_wallRunBufferTime));
        m_slideCooldownCoroutine = StartCoroutine(RunBufferTimer((x) => m_slideCooldownTimer = (x), m_slideProperties.m_slideCooldownTime));
    }

    private void OnValidate()
    {
        CalculateJump();
    }

    private void FixedUpdate()
    {
        PerformController();
    }

    public void PerformController()
    {
        CalculateVelocity();
        //CheckWallConnection();

        CaculateTotalVelocity();

        CheckLanded();
        SlopePhysics();

        ZeroOnGroundCeiling();
        
        CameraRotation();
        TiltLerp();

        if (!HitSide() || IsGrounded())
        {
            EndWallRunNew();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (HitSide())
        {
            if (hit.normal.y < 0.1f)
            {
                float wallDotProduct = Vector3.Dot(hit.normal, Vector3.up);

                if (wallDotProduct < 30)
                {
                    Vector3 wallVector = Vector3.Cross(hit.normal, Vector3.up);

                    m_wallRunProperties.m_wallMovementTransform.rotation = Quaternion.LookRotation(wallVector, Vector3.up);

                    if (m_lastWallHit == null)
                    {
                        m_lastWallHit = hit;
                    }

                    if (hit.collider != m_lastWallHit.collider)
                    {
                        m_lastWallHit = hit;
                    }

                    StartWallRunNew();
                }
            }
        }
    }

    private void StartWallRunNew()
    {
        if (!m_isWallRunning)
        {
            if (m_hasJumped)
            {
                if (CheckOverBuffer(ref m_wallRunBufferTimer, ref m_wallRunProperties.m_wallRunBufferTime, m_wallRunBufferCoroutine))
                {
                    StartCoroutine(RunWallRunNew());
                }
            }
        }
    }

    private void EndWallRunNew()
    {
        if (m_isWallRunning)
        {
            m_isWallRunning = false;
            m_wallRunBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_wallRunBufferTimer = (x), m_wallRunProperties.m_wallRunBufferTime));
        }
    }

    private void JumpOffWall()
    {
        EndWallRunNew();
        StartCoroutine(InAirBoost(m_wallRunProperties.m_wallRunJumpForce.y, m_wallRunProperties.m_wallRunJumpForce.x, m_wallJumpVector));
    }

    private IEnumerator RunWallRunNew()
    {
        m_isWallRunning = true;
        m_states.m_gravityControllState = GravityState.GravityDisabled;
        m_states.m_movementControllState = MovementControllState.MovementDisabled;

        float angleIntoWallStart = Vector3.Cross(m_wallRunProperties.m_wallMovementTransform.forward, transform.forward).y;

        if (angleIntoWallStart > -m_wallRunProperties.m_wallMidpointAngle)
        {
            m_velocity.y += 5f;
        }
        else
        {
            //m_wallVelocity.y += 20f;
            JumpMaxVelocity();
        }

        m_wallVelocity = Vector3.up * m_velocity.y;
        m_velocity = Vector3.zero;

        Vector3 wallForwardSmoothingVelocity = Vector3.zero;
        Vector3 wallStickSmoothingVelocity = Vector3.zero;

        float t = 0;
        float cameraDisconnectTimer = 0;
        float yVelocityStopTimer = 0;
        float yVelocityOnSlowdownStart = m_wallVelocity.y;
        bool yVelocityGoingUp = true;

        if (yVelocityOnSlowdownStart < 0)
        {
            yVelocityGoingUp = false;
        }

        m_wallStickVelocity = m_wallRunProperties.m_wallMovementTransform.right * -10;

        while (t < m_wallRunProperties.m_wallRunTime && m_isWallRunning)
        {
            t += Time.fixedDeltaTime;

            m_velocity.y = 0;

			#region Wall Movement
			Vector3 forwardMovement = transform.forward * m_movementInput.y;
            Vector3 rightMovement = transform.right * m_movementInput.x;
            Vector3 playerInput = Vector3.ClampMagnitude(forwardMovement + rightMovement, 1f);

            float wallInputRight = Vector3.Cross(playerInput, -m_wallRunProperties.m_wallMovementTransform.forward).y;
            float currentWallStickSpeed = Mathf.Lerp(-m_wallRunProperties.m_wallRunStickSpeed, m_wallRunProperties.m_wallRunStickSpeed, wallInputRight);
            Vector3 targetWallStickMovement = m_wallRunProperties.m_wallMovementTransform.right * currentWallStickSpeed;
            Vector3 wallStickMovement = Vector3.SmoothDamp(m_wallStickVelocity, targetWallStickMovement, ref wallStickSmoothingVelocity, m_wallRunProperties.m_wallRunStickAccelerationTime);
            m_wallStickVelocity = wallStickMovement;

            float wallInputForward = Vector3.Cross(playerInput, m_wallRunProperties.m_wallMovementTransform.right).y;
            float currentWallForwardSpeed = Mathf.Lerp(0, m_wallRunProperties.m_wallRunForwardSpeed, Mathf.Abs(wallInputForward));
            Vector3 targetWallForwardMovement = (m_wallRunProperties.m_wallMovementTransform.forward * Mathf.Sign(wallInputForward)) * currentWallForwardSpeed;
            Vector3 wallForwardMovement = Vector3.SmoothDamp(m_wallVelocity, targetWallForwardMovement, ref wallForwardSmoothingVelocity, m_wallRunProperties.m_wallRunForwardAccelerationTime);
            m_wallVelocity = new Vector3(wallForwardMovement.x, m_wallVelocity.y, wallForwardMovement.z);
			#endregion

			#region Wall Jump
			Vector3 offWallVector = m_wallRunProperties.m_wallMovementTransform.right;
            Vector3 forwardDir = m_wallRunProperties.m_wallMovementTransform.forward * wallInputForward;

            if (Vector3.Cross(m_wallRunProperties.m_wallMovementTransform.forward, playerInput).y < 0)
            {
                forwardDir = Vector3.Reflect(forwardDir, m_wallRunProperties.m_wallMovementTransform.right);
            }

            Vector3 jumpVector = (offWallVector * 15)  + (forwardDir * 25);
            m_wallJumpVector = jumpVector;
			#endregion

			#region Camera Calculation
			float lookDirCross = Mathf.Abs(Vector3.Cross(m_wallRunProperties.m_wallMovementTransform.right, transform.forward).y);
            float angleIntoWall = Vector3.Cross(m_wallRunProperties.m_wallMovementTransform.forward, transform.forward).y;

            float lookDir = Mathf.Sign(Vector3.Cross(m_wallRunProperties.m_wallMovementTransform.right, transform.forward).y);

            if (angleIntoWall > -m_wallRunProperties.m_wallMidpointAngle)
            {
                if (t < (m_wallRunProperties.m_wallRunTime * m_wallRunProperties.m_wallRunEndingDuration))
                {
                    float tiltResult = Mathf.Lerp(0, m_wallRunProperties.m_wallRunCameraMaxTilt * lookDir, lookDirCross);
                    m_tiltTarget = tiltResult;
                }
                else
                {
                    cameraDisconnectTimer += Time.fixedDeltaTime;
                    float cameraDisconnectProgress = cameraDisconnectTimer / 1;
                    float targetCameraAngle = Mathf.Lerp(0, m_wallRunProperties.m_wallRunCameraMaxTilt * lookDir, lookDirCross);
                    float cameraResetAngle = Mathf.Lerp(targetCameraAngle, 0, cameraDisconnectProgress);
                    m_tiltTarget = cameraResetAngle;
                }

                if (angleIntoWall < 0.1)
                {
                    transform.Rotate(Vector3.up * -lookDir, 1f); ;
                }
            }
            else
            {
                m_tiltTarget = 0;
            }
			#endregion

			#region Y Velocity Calculation
			if (yVelocityGoingUp)
            {
                m_wallVelocity.y += m_gravity * Time.fixedDeltaTime;

                if (m_wallVelocity.y <= 0)
                {
                    yVelocityGoingUp = false;
                    yVelocityOnSlowdownStart = 0;
                    yVelocityStopTimer = 0;
                }
            }
            else
            {
                if (wallInputRight < -0.5 || m_movementInput.y > 0)
                {
                    yVelocityStopTimer += Time.fixedDeltaTime;
                    float yStopProgress = m_wallRunProperties.m_wallYVelocitySlowdownCurve.Evaluate(yVelocityStopTimer / m_wallRunProperties.m_wallRunYVelocityStopTime);
                    m_wallVelocity.y = Mathf.Lerp(yVelocityOnSlowdownStart, 0, yStopProgress);
                }
                else
                {
                    yVelocityStopTimer = 0;
                    m_wallVelocity.y += (m_gravity / 2) * Time.fixedDeltaTime;
                    yVelocityOnSlowdownStart = m_wallVelocity.y;
                }
            }
			#endregion

			yield return new WaitForFixedUpdate();
        }

        if (t >= m_wallRunProperties.m_wallRunTime)
        {
            m_velocity = m_wallRunProperties.m_wallMovementTransform.right * 10;
        }

        m_wallVelocity = Vector3.zero;
        m_wallStickVelocity = Vector3.zero;
        m_tiltTarget = 0;

        m_states.m_gravityControllState = GravityState.GravityEnabled;
        m_states.m_movementControllState = MovementControllState.MovementEnabled;
        m_isWallRunning = false;
    }

    private IEnumerator InAirBoost(float p_yVelocity, float p_forwardSpeed, Vector3 p_wallJumpVelocity)
    {
        JumpMaxMultiplied(p_yVelocity);

        while (!IsGrounded() && !m_isWallRunning && !HitCeiling())
        {
            //Vector3 movementVelocity = m_wallJumpDir * p_forwardSpeed;
            Vector3 movementVelocity = p_wallJumpVelocity;

            m_wallRunJumpVelocity = new Vector3(movementVelocity.x, 0, movementVelocity.z);

            yield return new WaitForFixedUpdate();
        }

        if (IsGrounded() && !m_isWallRunning && !HitCeiling())
        {
            //StartCoroutine(OnGroundBoost(p_forwardSpeed));
        }

        m_wallRunJumpVelocity = Vector3.zero;
    }

    private IEnumerator OnGroundBoost(float p_forwardSpeed)
    {
        float t = 0;

        while (t < m_wallRunProperties.m_wallRunJumpGroundVelocityDecayTime && !m_isWallRunning && !HitSide())
        {
            if (IsGrounded() && !m_maintainSpeed)
            {
                t += Time.fixedDeltaTime;
            }

            float speedProgress = m_wallRunProperties.m_wallRunJumpGroundVelocityDecayAnimationCurve.Evaluate(t / m_wallRunProperties.m_wallRunJumpGroundVelocityDecayTime);
            float currentSpeed = Mathf.Lerp(p_forwardSpeed, 0, speedProgress);

            Vector3 movementVelocity = m_wallJumpDir * currentSpeed;
            m_wallRunJumpVelocity = movementVelocity;

            yield return new WaitForFixedUpdate();
        }

        m_wallRunJumpVelocity = Vector3.zero;
    }

    #region Camera Code
    public void AddRecoil(float p_recoilAmountX, float p_recoilAmountY, float p_fireRate)
    {
        StartCoroutine(RecoilKickX(p_recoilAmountX, p_fireRate));
        StartCoroutine(RecoilKickY(p_recoilAmountY, p_fireRate));
    }

    private IEnumerator RecoilKickY(float p_recoilAmount, float p_fireRate)
    {
        float amountOfFixedUpdatesToBeRun = 50f * p_fireRate;
        float deltaRcoil = p_recoilAmount / amountOfFixedUpdatesToBeRun;
        float totalCount = 0;

        float dir = Mathf.Sign(p_recoilAmount);
        Vector3 rotDir = dir * Vector3.up;

        if (dir < 0)
        {
            deltaRcoil *= -1f;
            p_recoilAmount *= -1f;
        }

        while (totalCount < p_recoilAmount)
        {
            transform.Rotate(rotDir, deltaRcoil);
            yield return new WaitForFixedUpdate();
            totalCount += deltaRcoil;
        }
    }

    private IEnumerator RecoilKickX(float p_recoilAmount, float p_fireRate)
    {
        float amountOfFixedUpdatesToBeRun = 50f * p_fireRate;

        float deltaRcoil = p_recoilAmount / amountOfFixedUpdatesToBeRun;

        float totalCount = 0;

        while (totalCount < p_recoilAmount)
        {
            RotateCameraAxisX(-deltaRcoil, m_cameraProperties.m_cameraMain, m_cameraProperties.m_maxCameraAng);
            yield return new WaitForFixedUpdate();

            totalCount += deltaRcoil;
        }
    }



    public void ResetCamera()
    {
        m_cameraProperties.m_cameraMain.rotation = Quaternion.identity;
        m_cameraProperties.m_cameraTilt.rotation = Quaternion.identity;
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

    public void RotateCameraAxisY(float p_rotateAmount, Transform p_targetTransform, float p_maxAngleOnAxis)
    {
        float cameraAngY = p_targetTransform.localEulerAngles.y;

        //Stops the camera from rotating, if it hits the resrictions
        if (p_rotateAmount < 0 && cameraAngY > 360 - p_maxAngleOnAxis || p_rotateAmount < 0 && cameraAngY < p_maxAngleOnAxis + 10)
        {
            p_targetTransform.Rotate(Vector3.up, p_rotateAmount);

        }
        else if (p_rotateAmount > 0 && cameraAngY > 360 - p_maxAngleOnAxis - 10 || p_rotateAmount > 0 && cameraAngY < p_maxAngleOnAxis)
        {
            p_targetTransform.Rotate(Vector3.up, p_rotateAmount);
        }

        if (p_targetTransform.localEulerAngles.y < 360 - p_maxAngleOnAxis && p_targetTransform.localEulerAngles.y > 180)
        {
            p_targetTransform.localEulerAngles = new Vector3(0f, 360 - p_maxAngleOnAxis, 0f);
        }
        else if (p_targetTransform.localEulerAngles.y > p_maxAngleOnAxis && p_targetTransform.localEulerAngles.y < 180)
        {
            p_targetTransform.localEulerAngles = new Vector3(0f, p_maxAngleOnAxis, 0f);
        }
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
    #endregion

    #region Wall Clamber Code
    private void StartWallClamber()
    {
        if (m_hasJumped)
        {
            if (!m_isWallRunning)
            {
                if (!m_isClambering)
                {
                    StartCoroutine(RunWallClamber());
                }
            }
        }
    }

    private void StopClamber()
    {
        m_isClambering = false;
    }

    private IEnumerator RunWallClamber()
    {
        m_isClambering = true;

        m_crouchOnLanding = false;

        while (m_isClambering)
        {
            Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);

            RaycastHit hitBottom;

            if (Physics.Raycast(bottom, transform.forward, out hitBottom, Mathf.Infinity, m_wallClamberMask))
            {
                m_isClambering = true;
            }
            else
            {
                StopClamber();
            }

            if (!HitSide())
            {
                StopClamber();
            }

            m_velocity.y = m_clamberSpeed;

            yield return new WaitForFixedUpdate();

        }

        m_isClambering = false;
    }
	#endregion

	#region Misc Wall Code
	private void TiltLerp()
    {
        m_cameraProperties.m_cameraTilt.localRotation = Quaternion.Slerp(m_cameraProperties.m_cameraTilt.localRotation, Quaternion.Euler(0, 0, m_tiltTarget), m_wallRunProperties.m_tiltSpeed);
    }
	#endregion

	#region Input Code
	public void SetMovementInput(Vector2 p_input)
    {
        m_movementInput = p_input;
    }

    public void SetLookInput(Vector2 p_input, float p_sensitivity)
    {
        m_lookInput = p_input;
        m_cameraProperties.m_mouseSensitivity = p_sensitivity;
    }
    #endregion

    #region Input Buffering Code

    private bool CheckBuffer(ref float p_bufferTimer, ref float p_bufferTime, Coroutine p_bufferTimerRoutine)
    {
        if (p_bufferTimer < p_bufferTime)
        {
            if (p_bufferTimerRoutine != null)
            {
                StopCoroutine(p_bufferTimerRoutine);
            }

            p_bufferTimer = p_bufferTime;

            return true;
        }
        else if (p_bufferTimer >= p_bufferTime)
        {
            return false;
        }

        return false;
    }

    private bool CheckOverBuffer(ref float p_bufferTimer, ref float p_bufferTime, Coroutine p_bufferTimerRoutine)
    {
        if (p_bufferTimer >= p_bufferTime)
        {
            p_bufferTimer = p_bufferTime;

            return true;
        }

        return false;
    }

    //Might want to change this so it does not feed the garbage collector monster
    private IEnumerator RunBufferTimer(System.Action<float> m_bufferTimerRef, float p_bufferTime)
    {
        float t = 0;

        while (t < p_bufferTime)
        {
            t += Time.deltaTime;
            m_bufferTimerRef(t);
            yield return null;
        }

        m_bufferTimerRef(p_bufferTime);
    }
    #endregion

    #region Physics Calculation Code

    private void CaculateTotalVelocity()
    {
        Vector3 velocity = Vector3.zero;

        velocity += m_velocity;
        velocity += m_wallRunJumpVelocity;
        velocity += m_slideVelocity;
        velocity += m_slopeVelocity;
        velocity += m_slopeShiftVelocity;
        velocity += m_wallVelocity;
        velocity += m_wallStickVelocity;

        m_characterController.Move(velocity * Time.fixedDeltaTime);
    }

    private void CalculateCurrentSpeed()
    {
        float speed = m_baseMovementProperties.m_baseMovementSpeed;

        speed += m_currentPostGrappleSpeedBoost;

        m_currentMovementSpeed = speed;
    }

    public bool IsGrounded()
    {
        if (m_characterController.isGrounded)
        {
            return true;
        }

        return false;
    }

    public bool HitCeiling()
    {
        if ((m_characterController.collisionFlags & CollisionFlags.Above) != 0)
        {
            return true;
        }

        return false;
    }

    public bool HitSide()
    {
        if ((m_characterController.collisionFlags & CollisionFlags.Sides) != 0)
        {
            return true;
        }

        return false;
    }

    private void OnOffLedge()
    {
        m_offLedge = true;

        m_graceBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_graceTimer = (x), m_jumpingProperties.m_graceTime));

    }

    private void ZeroOnGroundCeiling()
    {
        if (IsGrounded())
        {
            m_velocity.y = 0;
        }

        if (HitCeiling() && m_velocity.y > 0)
        {
            m_velocity.y = 0;
        }
    }

    private void CheckOffLedge()
    {
        if (!IsGrounded() && !m_offLedge)
        {
            OnOffLedge();
        }
        if (IsGrounded())
        {
            m_offLedge = false;
        }
    }

    private void CheckLanded()
    {
        if (IsGrounded() && !m_isLanded)
        {
            if (!m_isCrouching)
            {
                OnLanded();
            }
        }
        if (!IsGrounded())
        {
            m_isLanded = false;
        }
    }

    private void OnLanded()
    {
        m_isLanded = true;
        m_hasJumped = false;

        m_canWallClimb = true;

        if (m_crouchOnLanding)
        {
            StartCoroutine(RunCrouchDown());
            m_crouchOnLanding = false;
        }

        if (CheckBuffer(ref m_jumpBufferTimer, ref m_jumpingProperties.m_jumpBufferTime, m_jumpBufferCoroutine))
        {
            JumpMaxVelocity();
        }

        m_events.m_onLandedEvent.Invoke();
    }

    private void CalculateVelocity()
    {
        if (m_states.m_gravityControllState == GravityState.GravityEnabled)
        {
            m_velocity.y += m_gravity * Time.fixedDeltaTime;
        }

        if (m_states.m_movementControllState == MovementControllState.MovementEnabled)
        {
            Vector3 forwardMovement = transform.forward * m_movementInput.y;
            Vector3 rightMovement = transform.right * m_movementInput.x;

            Vector3 targetHorizontalMovement = Vector3.zero;

            if (!m_isCrouched)
            {
                targetHorizontalMovement = Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * m_currentMovementSpeed;
            }
            else
            {
                targetHorizontalMovement = Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * m_baseMovementProperties.m_crouchMovementSpeed;
            }

            Vector3 horizontalMovement = Vector3.SmoothDamp(m_velocity, targetHorizontalMovement, ref m_velocitySmoothing, IsGrounded() ? m_baseMovementProperties.m_accelerationTimeGrounded : m_baseMovementProperties.m_accelerationTimeAir);

            m_velocity = new Vector3(horizontalMovement.x, m_velocity.y, horizontalMovement.z);
        }

    }

    public void PhysicsSeekTo(Vector3 p_targetPosition)
    {
        Vector3 deltaPosition = p_targetPosition - transform.position;
        m_velocity = deltaPosition / Time.fixedDeltaTime;
    }
    #endregion

    #region Slide Code
    private void OnSlideStart()
    {
        if (!m_isSliding)
        {
            if (IsGrounded() || OnSlope().m_onSlope)
            {
                if (m_movementInput.y > 0)
                {
                    if (CheckOverBuffer(ref m_slideCooldownTimer, ref m_slideProperties.m_slideCooldownTime, m_slideCooldownCoroutine))
                    {
                        StartCoroutine(RunSlide());
                    }
                }
            }
        }
    }

    private void StopSlide()
    {
        m_slideTimer = m_slideProperties.m_slideTime;
    }

    private IEnumerator RunSlide()
    {
        m_isSliding = true;

        Vector3 slideDir = transform.forward;
        m_wallJumpDir = slideDir;
        
        m_slideTimer = 0;
        m_maintainSpeed = true;

        Vector3 slopeVelocitySmoothing = Vector3.zero;
        Vector3 slopeShiftVelocitySmoothing = Vector3.zero;

        float slideSpeedTimer = 0;

        while (m_slideTimer < m_slideProperties.m_slideTime)
        {
            m_slideTimer += Time.fixedDeltaTime;

            slideSpeedTimer += Time.fixedDeltaTime;
            float progress = m_slideProperties.m_slideCurve.Evaluate(slideSpeedTimer / m_slideProperties.m_slideTime);
            float currentSlideSpeed = Mathf.Lerp(m_slideProperties.m_slideSpeed, m_baseMovementProperties.m_crouchMovementSpeed, progress);
            Vector3 calculatedSlideVelocity = slideDir * currentSlideSpeed;

            #region Slope Slide Code
            
            SlopeInfo slopeInfo = OnSlope();

            if (slopeInfo.m_onSlope)
            {


                m_slideTimer = 0;

                float normalX = slopeInfo.m_slopeNormal.x > 0 ? slopeInfo.m_slopeNormal.x : slopeInfo.m_slopeNormal.x * -1;
                float normalZ = slopeInfo.m_slopeNormal.z > 0 ? slopeInfo.m_slopeNormal.z : slopeInfo.m_slopeNormal.z * -1;

                float slopeX = Mathf.Lerp(0, m_slideProperties.m_slideAngleBoostMax, normalX / m_slideProperties.m_slopeTolerence) * Mathf.Sign(slopeInfo.m_slopeNormal.x);
                float slopeZ = Mathf.Lerp(0, m_slideProperties.m_slideAngleBoostMax, normalZ / m_slideProperties.m_slopeTolerence) * Mathf.Sign(slopeInfo.m_slopeNormal.z);

                Vector3 slopeDir = new Vector3(slopeX, 0, slopeZ);
                Vector3 slopeMovement = Vector3.SmoothDamp(m_slopeVelocity, slopeDir, ref slopeVelocitySmoothing, m_slideProperties.m_slopeSlideAccelerationTime);
                m_slopeVelocity = new Vector3(slopeMovement.x, 0, slopeMovement.z);
                m_slideProperties.m_slopeTransform.rotation = Quaternion.LookRotation(m_slopeVelocity);

                Vector3 targetShiftVelocity = m_slideProperties.m_slopeTransform.right * m_movementInput.x * m_slideProperties.m_slideSideShiftMaxSpeed;
                Vector3 shiftMovement = Vector3.SmoothDamp(m_slopeShiftVelocity, targetShiftVelocity, ref slopeShiftVelocitySmoothing, m_slideProperties.m_slideSideShiftAcceleration);
                m_slopeShiftVelocity = shiftMovement;

                Debug.Log(Vector3.Angle(m_slideProperties.m_slopeTransform.forward, transform.forward));

            }
            #endregion

            if (slideSpeedTimer < m_slideProperties.m_slideTime)
            {
                m_slideVelocity = new Vector3(calculatedSlideVelocity.x, 0, calculatedSlideVelocity.z);
            }
            else
            {
                m_slideVelocity = Vector3.zero;
            }

            yield return new WaitForFixedUpdate();
        }

        m_slopeVelocity = Vector3.zero;
        m_slideVelocity = Vector3.zero;
        m_slopeShiftVelocity = Vector3.zero;

        m_maintainSpeed = false;

        m_slideCooldownCoroutine = StartCoroutine(RunBufferTimer((x) => m_slideCooldownTimer = (x), m_slideProperties.m_slideCooldownTime));

        m_isSliding = false;
    }
    #endregion

    #region Crouch Code
    public void OnCrouchInputDown()
    {
        EvaluateCrouch();
    }

    private void EvaluateCrouch()
    {
        if (m_isWallRunning)
        {
            StartCrouch();
            return;
        }
        else if (m_isClimbing)
        {
            StartCrouch();
            return;
        }
        else if (m_isClambering)
        {
            StopClamber();
            StartCrouch();
            return;
        }

        if (!m_isCrouched)
        {
            StartCrouch();
        }
        else
        {
            EndCrouch();
        }
    }

    private void StartCrouch()
    {
        if (IsGrounded() || OnSlope().m_onSlope)
        {
            StartCoroutine(RunCrouchDown());
            return;
        }

        if (!IsGrounded() && !m_isClimbing && !m_isClambering)
        {
            m_crouchOnLanding = true;
            return;
        }
    }

    private void EndCrouch()
    {
        StartCoroutine(RunCrouchUp());
    }

    private IEnumerator RunCrouchDown()
    {
        OnSlideStart();

        m_isCrouching = true;

        float t = 0;

        float lastHeight;

        while (t < m_crouchProperties.m_crouchTime)
        {
            t += Time.fixedDeltaTime;

            float progress = t / m_crouchProperties.m_crouchTime;
            float currentHeight = Mathf.Lerp(2, m_crouchProperties.m_crouchHeight, progress);

            lastHeight = m_characterController.height;
            m_characterController.height = currentHeight;

            m_characterController.Move(Vector3.up * (m_characterController.height - lastHeight) / 2);

            yield return new WaitForFixedUpdate();
        }

        m_isCrouching = false;
        m_isCrouched = true;
    }

    private IEnumerator RunCrouchUp()
    {
        m_isCrouched = false;

        StopSlide();

        m_isCrouching = true;

        float t = 0;

        float lastHeight;

        while (t < m_crouchProperties.m_crouchTime)
        {
            t += Time.fixedDeltaTime;
            float progress = t / m_crouchProperties.m_crouchTime;
            float currentHeight = Mathf.Lerp(m_crouchProperties.m_crouchHeight, 2, progress);

            lastHeight = m_characterController.height;

            m_characterController.height = currentHeight;

            m_characterController.Move(Vector3.up * (m_characterController.height - lastHeight) / 2);



            yield return new WaitForFixedUpdate();
        }

        m_isCrouching = false;
    }
    #endregion

    #region Slope Code
    private struct SlopeInfo
    {
        public bool m_onSlope;

        public float m_slopeAngle;

        public Vector3 m_slopeNormal;
    }

    private SlopeInfo OnSlope()
    {
        SlopeInfo slopeInfo = new SlopeInfo { };

        RaycastHit hit;

        Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);

        if (Physics.Raycast(bottom, Vector3.down, out hit, 1f))
        {
            if (hit.normal != Vector3.up)
            {
                slopeInfo.m_onSlope = true;
                slopeInfo.m_slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
                slopeInfo.m_slopeNormal = hit.normal;

                return slopeInfo;
            }
        }

        return slopeInfo;
    }

    private void SlopePhysics()
    {
        SlopeInfo slopeInfo = OnSlope();

        if (slopeInfo.m_onSlope == true)
        {
            if (m_velocity.y > 0)
            {
                return;
            }

            if (m_hasJumped)
            {
                return;
            }

            RaycastHit hit;

            Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);

            if (Physics.Raycast(bottom, Vector3.down, out hit))
            {
                if (slopeInfo.m_slopeAngle > m_characterController.slopeLimit)
                {
                    //m_velocity.x += (1f - hit.normal.y) * hit.normal.x * (m_baseMovementProperties.m_slopeFriction);
                    //m_velocity.z += (1f - hit.normal.y) * hit.normal.z * (m_baseMovementProperties.m_slopeFriction);
                }

                m_characterController.Move(new Vector3(0, -(hit.distance), 0));
            }
        }
    }
    #endregion

    #region Jump Code
    public void OnJumpInputDown()
    {
        m_jumpBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_jumpBufferTimer = (x), m_jumpingProperties.m_jumpBufferTime));

        if (m_isCrouched)
        {
            if (m_isSliding)
            {
                SlideJump();
            }
            else
            {
                CrouchJump();
            }

            return;
        }

        if (CheckBuffer(ref m_graceTimer, ref m_jumpingProperties.m_graceTime, m_graceBufferCoroutine) && !IsGrounded() && m_velocity.y <= 0f)
        {
            //GroundJump();
            //return;
        }

        if (m_isWallRunning)
        {
            WallRunningJump();
            return;
        }

        if (IsGrounded() || OnSlope().m_onSlope)
        {
            GroundJump();
            return;
        }

    }

    public void OnJumpInputUp()
    {
        if (m_velocity.y > m_minJumpVelocity)
        {
            //JumpMinVelocity();
        }
    }

    private void CalculateJump()
    {
        m_gravity = -(2 * m_jumpingProperties.m_maxJumpHeight) / Mathf.Pow(m_jumpingProperties.m_timeToJumpApex, 2);
        m_maxJumpVelocity = Mathf.Abs(m_gravity) * m_jumpingProperties.m_timeToJumpApex;
        m_minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(m_gravity) * m_jumpingProperties.m_minJumpHeight);
    }

    private void SlideJump()
    {
        //StartCoroutine(InAirBoost(m_slideJumpForce.y, m_slideJumpForce.x));

        EndCrouch();
        m_hasJumped = true;
    }

    private void CrouchJump()
    {
        EndCrouch();
        JumpMaxVelocity();
        m_hasJumped = true;
    }

    private void WallRunningJump()
    {
        JumpOffWall();
        m_hasJumped = true;
    }

    private void GroundJump()
    {
        m_events.m_onJumpEvent.Invoke();
        JumpMaxVelocity();
    }

    private void JumpMaxVelocity()
    {
        m_hasJumped = true;
        m_velocity.y = m_maxJumpVelocity;
    }

    private void JumpMinVelocity()
    {
        m_velocity.y = m_minJumpVelocity;
    }

    private void JumpMaxMultiplied(float p_force)
    {
        m_velocity.y = m_maxJumpVelocity * p_force;
    }

    #endregion

    #region Grapple Code
    public void OnGrappleInputDown()
    {
        RaycastHit hit;

        if (Physics.Raycast(m_cameraProperties.m_camera.transform.position, m_cameraProperties.m_camera.transform.forward, out hit, Mathf.Infinity, m_grappleProperties.m_grappleWallMask))
        {
            StartCoroutine(RunGrapple(hit));
        }
    }

    public void OnGrappleInputUp()
    {
        m_isGrappling = false;
    }

    private IEnumerator RunGrapple(RaycastHit p_grappleHit)
    {
        m_grappleProperties.m_grappleRopeVisual.enabled = true;
        Vector3 grapplePoint = p_grappleHit.point;

        m_states.m_movementControllState = MovementControllState.MovementDisabled;
        m_states.m_gravityControllState = GravityState.GravityDisabled;
        m_isGrappling = true;

        m_grappleProperties.m_grappleRopeVisual.SetPosition(0, grapplePoint);

        float startVelocity = m_velocity.magnitude;
        float grappleSpeedUpTimer = 0;

        Vector3 grappleShiftVelocity = Vector3.zero;
        Vector3 grappleShiftVelocitySmoothing = Vector3.zero;

        float grappleSpeedForEnd = 0;

        while (m_isGrappling)
        {
            grappleSpeedUpTimer += Time.fixedDeltaTime;

            float progress = m_grappleProperties.m_grappleSpeedupCurve.Evaluate(grappleSpeedUpTimer / m_grappleProperties.m_grappleSpeedupTime);

            float currentSpeed = Mathf.Lerp(startVelocity, m_grappleProperties.m_grappleSpeedMax, progress);

            grappleSpeedForEnd = currentSpeed;

            Vector3 dirToPoint = (grapplePoint - transform.position).normalized;
            m_velocity = dirToPoint * currentSpeed;

            m_grappleProperties.m_grappleTransform.rotation = Quaternion.LookRotation(dirToPoint);

            Vector3 rightMovement = m_grappleProperties.m_grappleTransform.right * m_movementInput.x;
            Vector3 targetHorizontalMovement = Vector3.ClampMagnitude(rightMovement, 1.0f) * m_grappleProperties.m_grappleShiftMaxSpeed;
            Vector3 horizontalMovement = Vector3.SmoothDamp(grappleShiftVelocity, targetHorizontalMovement, ref grappleShiftVelocitySmoothing, m_grappleProperties.m_grappleShiftAcceleration);

            grappleShiftVelocity = horizontalMovement;

            m_velocity += grappleShiftVelocity;

            m_grappleProperties.m_grappleRopeVisual.SetPosition(1, transform.position);

            Vector3 pointFacingVector = Vector3.Cross(dirToPoint, m_cameraProperties.m_camera.transform.forward);
            Vector3 localPointFacingVector = m_cameraProperties.m_camera.transform.InverseTransformDirection(pointFacingVector);

            if (Mathf.Abs(localPointFacingVector.y) > m_grappleProperties.m_grappleTolerence)
            {
                m_isGrappling = false;
            }

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(PostGrappleSpeedBoost(grappleSpeedForEnd));

        m_grappleProperties.m_grappleRopeVisual.enabled = false;

        m_states.m_movementControllState = MovementControllState.MovementEnabled;
        m_states.m_gravityControllState = GravityState.GravityEnabled;
    }

    private IEnumerator PostGrappleSpeedBoost(float p_exitSpeed)
    {
        while (!IsGrounded())
        {
            m_currentPostGrappleSpeedBoost = p_exitSpeed;

            yield return new WaitForFixedUpdate();
        }

        m_currentPostGrappleSpeedBoost = 0;
    }

    #endregion

    public bool CheckCollisionLayer(LayerMask p_layerMask, GameObject p_object)
    {
        if (p_layerMask == (p_layerMask | (1 << p_object.layer)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}