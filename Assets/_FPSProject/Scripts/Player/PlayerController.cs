using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Photon.Pun;

[System.Serializable]
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
        public LayerMask m_wallMask;

        public AnimationCurve m_wallSpeedCurve;
        public float m_wallSpeedUpTime;
        public float m_maxWallRunSpeed;

        public float m_tiltSpeed;
        public float m_wallRunCameraMaxTilt;

        public int m_wallRidingRayCount;
        public float m_wallRaySpacing;
        public float m_wallRunRayLength;
        public float m_wallRunBufferTime;
        public Vector3 m_wallRunJumpVelocity;

        public float m_wallJumpBufferTime;
        public Vector3 m_wallJumpVelocity;
    }

    [Header("Wall Run Properties")]
    public WallRunProperties m_wallRunProperties;

    private float m_currentWallRunningSpeed;

    private float m_wallRunBufferTimer;
    private float m_wallJumpBufferTimer;

    private float m_tiltTarget;
    private float m_tiltSmoothingVelocity;

    private bool m_isWallRunning;
    private bool m_connectedWithWall;
    [HideInInspector]
    public bool m_holdingWallRideStick;

    private Vector3 m_wallNormal;
    private Vector3 m_wallVector;
    private Vector3 m_wallFacingVector;
    private Vector3 m_modelWallRunPos;

    private Coroutine m_wallJumpBufferCoroutine;
    private Coroutine m_wallRunBufferCoroutine;
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


    private float m_currentWallClimbSpeed;
    private bool m_isWallClimbing;
    [HideInInspector]
    public Vector3 m_localWallFacingVector;
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
    #endregion

    #region Slide Properties
    [System.Serializable]
    public struct SlideProperties
    {
        [Header("Basic Slide Properties")]
        public float m_slideSpeed;
        public float m_slideTime;
        public AnimationCurve m_slideCurve;

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

    private float m_currentPostClamberSpeedBoost;

    public AnimationCurve m_postClamberSpeedBoostDecay;

    public float m_postClamberSpeedBoost;
    public float m_postClamberSpeedBoostTime;

    private bool m_jumpedDuringClamber;

    #endregion

    [HideInInspector]
    public Vector2 m_movementInput;
    private Vector2 m_lookInput;

    private bool m_holdingJumpInput;

    [Space]

    public float m_wallRaySpacing;
    public float m_wallRayCount;
    public float m_wallRayLength;
    public LayerMask m_wallConnectMask;

    public float m_wallClimbTolerence;
    public float m_wallRunTolerenceMax;
    public float m_wallRunTolerenceMin;

    public float m_wallRunSpeed;
    public float m_wallRunSpeedUpTime;

    public float m_wallRunYVelocityStopTime;

    public AnimationCurve m_wallRunSpeedUpCurve;

    private void Start()
    {
        m_characterController = GetComponent<CharacterController>();

        CalculateJump();
        LockCursor();

        m_currentMovementSpeed = m_baseMovementProperties.m_baseMovementSpeed;
        m_jumpBufferTimer = m_jumpingProperties.m_jumpBufferTime;

        m_wallJumpBufferTimer = m_wallRunProperties.m_wallJumpBufferTime;
        m_wallRunBufferTimer = m_wallRunProperties.m_wallRunBufferTime;


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
        CalculateCurrentSpeed();
        CalculateVelocity();

        //CheckWall();

        //CheckWallRun();

        //CheckWallConnection();

        CheckWallConnection();

        m_characterController.Move(m_velocity * Time.deltaTime);

        SlopePhysics();

        ZeroOnGroundCeiling();
        CheckLanded();
        CheckOffLedge();

        CameraRotation();
        TiltLerp();
    }

    private void CheckWallConnection()
    {
        bool anyRayHit = false;

        if (HitSide())
        {
            #region Clamber and Climb
            bool collidedTop = false;
            bool collidedBottom = false;

            Vector3 top = m_characterController.transform.position + new Vector3(0, m_characterController.height / 2, 0);
            Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);

            RaycastHit hitTop;
            RaycastHit hitBottom;

            if (Physics.Raycast(top, transform.forward, out hitTop, Mathf.Infinity, m_wallClamberMask))
            {
                collidedTop = true;
            }

            if (Physics.Raycast(bottom, transform.forward, out hitBottom, Mathf.Infinity, m_wallClamberMask))
            {
                collidedBottom = true;
            }

            if (collidedBottom && !collidedTop)
            {
                if (!m_isClambering)
                {
                    if (m_hasJumped)
                    {
                        if (!m_isWallRunning)
                        {
                            Debug.Log("ran clamber");

                            StartCoroutine(RunWallClamber());

                            return;
                        }

                    }

                }
            }

            if (collidedBottom && collidedTop)
            {
                if (!m_isClimbing)
                {
                    if (m_canWallClimb)
                    {
                        if (m_hasJumped)
                        {
                            if (!m_isWallRunning)
                            {
                                Debug.Log("ran climb");

                                StartCoroutine(RunWallClimb());
                                return;
                            }
                        }
                    }

                }
            }
            #endregion

            #region Wall run
            float m_angleBetweenRays = 360f / 2;

            for (int i = 0; i < 2; i++)
            {
                Quaternion raySpaceQ = Quaternion.Euler(0, (i * m_angleBetweenRays) - (m_angleBetweenRays / 2), 0);
                RaycastHit hit;

                if (!anyRayHit)
                {
                    if (Physics.Raycast(m_characterController.transform.position, raySpaceQ * transform.forward, out hit, 10f, m_wallConnectMask))
                    {
                        if (Vector3.Dot(hit.normal, Vector3.up) == 0)
                        {
                            anyRayHit = true;

                            Vector3 wallVector = Vector3.Cross(hit.normal, Vector3.up);
                            Vector3 wallFacingVector = Vector3.Cross(wallVector, m_cameraProperties.m_camera.transform.forward);

                            float moveDir = -Mathf.Sign(Vector3.Cross(hit.normal, m_cameraProperties.m_camera.transform.forward).y);

                            Debug.DrawLine(m_characterController.transform.position, hit.point);

                            if (wallFacingVector.y < (m_wallRunTolerenceMin * -1))
                            {
                                if (wallFacingVector.y > (m_wallRunTolerenceMax * -1))
                                {
                                    Debug.Log("ran wall run");
                                    StartWallRun(-hit.normal, moveDir);
                                }
                            }
                        }
                    }
                }
            }
			#endregion
		}
	}

    private void StartWallRun(Vector3 p_dirToWallStart, float p_wallRunMovementDir)
    {
        if (!m_isWallRunning)
        {
            StartCoroutine(RunWallRun(p_dirToWallStart, p_wallRunMovementDir));
        }
    }

    private IEnumerator RunWallRun(Vector3 p_dirToWallStart, float p_wallRunMovementDir)
    {
        m_isWallRunning = true;
        m_states.m_gravityControllState = GravityState.GravityDisabled;
        m_states.m_movementControllState = MovementControllState.MovementDisabled;

        Vector3 dirToWall = p_dirToWallStart;

        float yVelStart = m_velocity.y;

        float speedStart = m_characterController.velocity.magnitude;

        float t = 0;

        while (m_isWallRunning)
        {
            t += Time.fixedDeltaTime;

            float speedProgress = m_wallRunSpeedUpCurve.Evaluate(t / m_wallRunSpeedUpTime);
            float currentWallRunSpeed = Mathf.Lerp(speedStart, m_wallRunSpeed, speedProgress);

            float yProgress = t / m_wallRunYVelocityStopTime;
            float yVelocity = Mathf.Lerp(yVelStart, 0, yProgress);

            float tiltResult = Mathf.Lerp(-m_wallRunProperties.m_wallRunCameraMaxTilt, m_wallRunProperties.m_wallRunCameraMaxTilt, -p_wallRunMovementDir);
            m_tiltTarget = tiltResult;

            RaycastHit hit;

            if (Physics.Raycast(m_characterController.transform.position, dirToWall, out hit, 10f, m_wallConnectMask))
            {
                if (Vector3.Dot(hit.normal, Vector3.up) == 0)
                {
                    transform.position = hit.point + hit.normal * m_characterController.radius;

                    Vector3 wallVector = Vector3.Cross(hit.normal, Vector3.up);
                    dirToWall = -hit.normal;

                    Vector3 wallRunVelocity = (wallVector * p_wallRunMovementDir) * currentWallRunSpeed;
                    m_velocity = new Vector3(wallRunVelocity.x, yVelocity, wallRunVelocity.z);
                }
                else
                {
                    m_isWallRunning = false;
                }
            }
            else
            {
                m_isWallRunning = false;
            }

            yield return new WaitForFixedUpdate();
        }

        //m_velocity = Vector3.zero;

        m_tiltTarget = 0f;

        m_states.m_movementControllState = MovementControllState.MovementEnabled;
        m_states.m_gravityControllState = GravityState.GravityEnabled;

        m_isWallRunning = false;
    }

    private IEnumerator InAirBoost(float p_yVelocity, float p_forwardSpeed, Vector3 p_movementDirection)
    {
        //m_velocity.y = p_yVelocity;

        JumpMaxMultiplied(p_yVelocity);

        while (!IsGrounded() && !m_isWallRunning)
        {
            Vector3 movementVelocity = p_movementDirection * p_forwardSpeed;

            m_velocity = new Vector3(movementVelocity.x, m_velocity.y, movementVelocity.z);

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator RunWallClamber()
    {
        m_states.m_gravityControllState = GravityState.GravityDisabled;

        m_jumpedDuringClamber = false;

        m_isClambering = true;

        bool isClamber = true;

        bool pressedJumpDuringClamber = false;

        while (isClamber)
        {
            Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);

            RaycastHit hitBottom;

            if (Physics.Raycast(bottom, transform.forward, out hitBottom, Mathf.Infinity, m_wallClamberMask))
            {
                isClamber = true;
            }
            else
            {
                isClamber = false;
            }

            if (!HitSide())
            {
                isClamber = false;
            }

            m_velocity.y = m_clamberSpeed;

            if (m_jumpedDuringClamber)
            {
                pressedJumpDuringClamber = true;

                m_jumpedDuringClamber = false;
            }

            yield return new WaitForFixedUpdate();

        }

        m_isClambering = false;

        m_states.m_gravityControllState = GravityState.GravityEnabled;

        if (pressedJumpDuringClamber)
        {
            //StartCoroutine(PostClamberSpeedBoost());
        }
    }

    private IEnumerator RunWallClimb()
    {
        m_isClimbing = true;

        float t = 0;

        while (t < m_climbTime)
        {
            t += Time.fixedDeltaTime;

            float progress = m_climbSpeedDecreaseCurve.Evaluate(t / m_climbTime);


            Vector3 top = m_characterController.transform.position + new Vector3(0, m_characterController.height / 2, 0);

            RaycastHit hitTop;


            if (!Physics.Raycast(top, transform.forward, out hitTop, Mathf.Infinity, m_wallClamberMask))
            {
                t = m_climbTime;

                StartCoroutine(RunWallClamber());
            }

            if (!HitSide())
            {
                t = m_climbTime;
            }


            float currentClimbSpeed = Mathf.Lerp(m_climbSpeed, 0, progress);

            m_velocity.y = currentClimbSpeed;

            yield return new WaitForFixedUpdate();
        }

        m_isClimbing = false;

        m_canWallClimb = false;
    }


    #region Wall Code

    private void CheckWallConnectionForVault()
    {
        bool collidedTop = false;
        bool collidedBottom = false;

        Vector3 top = m_characterController.transform.position + new Vector3(0, m_characterController.height / 2, 0);
        collidedTop = GetWallHitData(top);

        Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);
        collidedBottom = GetWallHitData(bottom);

        if (HitSide())
        {
            if (collidedBottom && !collidedTop)
            {
                if (!m_isClambering)
                {
                    if (m_hasJumped)
                    {
                        StartCoroutine(RunWallClamber());
                        return;
                    }
                }
            }

            if (m_holdingJumpInput)
            {
                if (m_localWallFacingVector.x >= m_wallClimbTolerence)
                {
                    if (collidedBottom && collidedTop)
                    {
                        if (!m_isClimbing)
                        {
                            if (m_canWallClimb)
                            {
                                if (m_hasJumped)
                                {
                                    StartCoroutine(RunWallClimb());

                                    return;
                                }
                            }
                        }
                    }
                }
            }

            if (collidedBottom)
            {
                if (Mathf.Abs(m_localWallFacingVector.y) >= m_wallRunTolerenceMax)
                {
                    if (!m_isWallRunning && (!m_isClimbing || m_isClambering))
                    {
                        StartCoroutine(WallRunning());
                        return;
                    }
                }
            }
        }
    }

    private bool GetWallHitData(Vector3 p_castOrigin)
    {
        float m_angleBetweenRays = m_wallRaySpacing / m_wallRayCount;
        bool anyRayHit = false;

        for (int i = 0; i < m_wallRayCount; i++)
        {
            Quaternion raySpaceQ = Quaternion.Euler(0, (i * m_angleBetweenRays) - (m_angleBetweenRays * (m_wallRayCount / 2)), 0);
            RaycastHit hit;

            if (Physics.Raycast(p_castOrigin, raySpaceQ * transform.forward, out hit, m_wallRayLength, m_wallConnectMask))
            {
                if (Vector3.Dot(hit.normal, Vector3.up) == 0)
                {
                    anyRayHit = true;

                    m_wallVector = Vector3.Cross(hit.normal, Vector3.up);
                    m_wallFacingVector = Vector3.Cross(hit.normal, m_cameraProperties.m_camera.transform.forward);
                    m_wallNormal = hit.normal;

                    m_localWallFacingVector = m_cameraProperties.m_camera.transform.InverseTransformDirection(m_wallFacingVector);

                    Debug.DrawLine(m_characterController.transform.position, hit.point);

                    return true;

                    //CheckWallMovementType();
                }


            }
        }


        if (!anyRayHit)
        {
            m_isWallRunning = false;
            m_isWallClimbing = false;
            m_connectedWithWall = false;
        }

        return false;
    }

    public void CheckWall()
    {
        if (HitSide())
        {
            bool collidedTop = false;
            bool collidedBottom = false;

            Vector3 top = m_characterController.transform.position + new Vector3(0, m_characterController.height / 2, 0);
            Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);

            RaycastHit hitTop;
            RaycastHit hitBottom;

            if (Physics.Raycast(top, transform.forward, out hitTop, Mathf.Infinity, m_wallClamberMask))
            {
                collidedTop = true;
            }

            if (Physics.Raycast(bottom, transform.forward, out hitBottom, Mathf.Infinity, m_wallClamberMask))
            {
                collidedBottom = true;
            }

            if (collidedBottom && !collidedTop)
            {
                if (!m_isClambering)
                {
                    if (m_hasJumped)
                    {
                        StartCoroutine(RunWallClamber());
                        return;
                    }

                }
            }

            if (collidedBottom && collidedTop)
            {
                if (!m_isClimbing)
                {
                    if (m_canWallClimb)
                    {
                        if (m_hasJumped)
                        {
                            StartCoroutine(RunWallClimb());
                        }
                    }

                }
            }
        }
    }

    private IEnumerator PostClamberSpeedBoost()
    {
        float t = 0;

        JumpMaxMultiplied(0.5f);

        m_currentPostClamberSpeedBoost = m_postClamberSpeedBoost;

        while (t < m_postClamberSpeedBoostTime)
        {
            t += Time.fixedDeltaTime;

            float progress = m_postClamberSpeedBoostDecay.Evaluate(t / m_postClamberSpeedBoostTime);

            m_currentPostClamberSpeedBoost = Mathf.Lerp(m_postClamberSpeedBoost, 0, progress);

            yield return new WaitForFixedUpdate();
        }

        m_currentPostClamberSpeedBoost = 0;
    }

    #endregion

    #region Wall Run Code

    private void CheckWallRun()
    {
        float m_angleBetweenRays = m_wallRunProperties.m_wallRaySpacing / m_wallRunProperties.m_wallRidingRayCount;
        bool anyRayHit = false;

        for (int i = 0; i < m_wallRunProperties.m_wallRidingRayCount; i++)
        {
            Quaternion raySpaceQ = Quaternion.Euler(0, (i * m_angleBetweenRays) - (m_angleBetweenRays * (m_wallRunProperties.m_wallRidingRayCount / 2)), 0);
            RaycastHit hit;

            if (Physics.Raycast(m_characterController.transform.position, raySpaceQ * transform.forward, out hit, m_wallRunProperties.m_wallRunRayLength, m_wallRunProperties.m_wallMask))
            {
                if (Vector3.Dot(hit.normal, Vector3.up) == 0)
                {
                    anyRayHit = true;

                    m_wallVector = Vector3.Cross(hit.normal, Vector3.up);
                    m_wallFacingVector = Vector3.Cross(hit.normal, m_cameraProperties.m_camera.transform.forward);
                    m_wallNormal = hit.normal;

                    m_localWallFacingVector = m_cameraProperties.m_camera.transform.InverseTransformDirection(m_wallFacingVector);

                    if (!m_connectedWithWall)
                    {
                        OnWallConnect();
                    }

                    CheckToStartWallRun();
                }

                Debug.DrawLine(m_characterController.transform.position, hit.point);
            }
        }

        if (!anyRayHit)
        {
            m_isWallRunning = false;
            m_isWallClimbing = false;
            m_connectedWithWall = false;
        }

    }

    private void OnWallConnect()
    {
        m_connectedWithWall = true;
        m_wallJumpBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_wallJumpBufferTimer = (x), m_wallRunProperties.m_wallJumpBufferTime));
    }

    private void TiltLerp()
    {
        m_cameraProperties.m_cameraTilt.localRotation = Quaternion.Slerp(m_cameraProperties.m_cameraTilt.localRotation, Quaternion.Euler(0, 0, m_tiltTarget), m_wallRunProperties.m_tiltSpeed);
    }

    private void OnWallRideRelease()
    {
        m_isWallRunning = false;
        m_isWallClimbing = false;
        m_wallRunBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_wallRunBufferTimer = (x), m_wallRunProperties.m_wallRunBufferTime));
    }

    private void CheckToStartWallRun()
    {
        if (m_holdingWallRideStick)
        {
            if (m_isWallClimbing)
            {
                return;
            }

            if (m_isWallRunning)
            {
                return;
            }

            if (m_localWallFacingVector.x >= m_wallClimbProperties.m_wallClimbFactor)
            {
                if (!m_isWallClimbing)
                {
                    if (CheckOverBuffer(ref m_wallRunBufferTimer, ref m_wallRunProperties.m_wallRunBufferTime, m_wallRunBufferCoroutine))
                    {
                        StartCoroutine(WallClimbing());
                        return;
                    }
                }
            }

            if (!m_isWallRunning)
            {
                if (CheckOverBuffer(ref m_wallRunBufferTimer, ref m_wallRunProperties.m_wallRunBufferTime, m_wallRunBufferCoroutine))
                {
                    StartCoroutine(WallRunning());
                    return;

                }
            }
        }

    }

    private IEnumerator WallClimbing()
    {
        m_events.m_onWallClimbBeginEvent.Invoke();

        m_isWallClimbing = true;

        m_states.m_gravityControllState = GravityState.GravityDisabled;
        m_states.m_movementControllState = MovementControllState.MovementDisabled;

        m_currentWallClimbSpeed = 0;

        float t = 0;

        while (m_isWallClimbing)
        {
            t += Time.deltaTime;


            m_velocity = Vector3.zero;

            m_velocity.y = m_localWallFacingVector.x * m_currentMovementSpeed;

            float progress = m_wallClimbProperties.m_wallClimbSpeedCurve.Evaluate(t / m_wallClimbProperties.m_wallClimbSpeedUpTime);
            m_currentWallClimbSpeed = Mathf.Lerp(0f, m_wallClimbProperties.m_maxWallClimbSpeed, progress);

            yield return null;
        }

        m_states.m_movementControllState = MovementControllState.MovementEnabled;
        m_states.m_gravityControllState = GravityState.GravityEnabled;

        m_currentWallClimbSpeed = 0;

        m_events.m_onWallClimbEndEvent.Invoke();
    }

    private IEnumerator WallRunning()
    {
        m_events.m_onWallRunBeginEvent.Invoke();

        m_isWallRunning = true;
        m_states.m_gravityControllState = GravityState.GravityDisabled;
        m_states.m_movementControllState = MovementControllState.MovementDisabled;

        m_currentWallRunningSpeed = 0;

        float t = 0;

        float yVelStart = m_velocity.y;

        while (m_isWallRunning)
        {
            t += Time.fixedDeltaTime;

            float result = Mathf.Lerp(-m_wallRunProperties.m_wallRunCameraMaxTilt, m_wallRunProperties.m_wallRunCameraMaxTilt, m_wallFacingVector.y);
            m_tiltTarget = result;

            Vector3 wallRunVelocity = (m_wallVector * -m_wallFacingVector.y) * m_currentMovementSpeed;

            float yProgress = t / 0.5f;

            float yVelocity = Mathf.Lerp(yVelStart, 0, yProgress);

            m_velocity = new Vector3(wallRunVelocity.x, yVelocity, wallRunVelocity.z);


            //m_velocity += (transform.right * m_wallFacingVector.y) * m_currentMovementSpeed;

            //m_velocity.y = 0;

            float progress = m_wallRunProperties.m_wallSpeedCurve.Evaluate(t / m_wallRunProperties.m_wallSpeedUpTime);
            m_currentWallRunningSpeed = Mathf.Lerp(0f, m_wallRunProperties.m_maxWallRunSpeed, progress);

            yield return new WaitForFixedUpdate();
        }

        m_states.m_movementControllState = MovementControllState.MovementEnabled;
        m_states.m_gravityControllState = GravityState.GravityEnabled;

        m_currentWallRunningSpeed = 0;

        m_tiltTarget = 0f;

        m_events.m_onWallRunEndEvent.Invoke();
    }

    #endregion

    #region Input Code
    public void SetMovementInput(Vector2 p_input)
    {
        m_movementInput = p_input;
    }

    public void SetLookInput(Vector2 p_input)
    {
        m_lookInput = p_input;
    }

    public void WallRideInputDown()
    {
        m_holdingWallRideStick = true;
    }

    public void WallRideInputUp()
    {
        m_holdingWallRideStick = false;
        OnWallRideRelease();
    }
    #endregion

    #region Camera Code
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
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

        float cameraXAng = m_cameraProperties.m_cameraMain.transform.eulerAngles.x;

        //Stops the camera from rotating, if it hits the resrictions
        if (cameraInput.x < 0 && cameraXAng > 360 - m_cameraProperties.m_maxCameraAng || cameraInput.x < 0 && cameraXAng < m_cameraProperties.m_maxCameraAng + 10)
        {
            m_cameraProperties.m_cameraMain.transform.Rotate(Vector3.right, cameraInput.x * (m_cameraProperties.m_mouseSensitivity));

        }
        else if (cameraInput.x > 0 && cameraXAng > 360 - m_cameraProperties.m_maxCameraAng - 10 || cameraInput.x > 0 && cameraXAng < m_cameraProperties.m_maxCameraAng)
        {
            m_cameraProperties.m_cameraMain.transform.Rotate(Vector3.right, cameraInput.x * (m_cameraProperties.m_mouseSensitivity));

        }

        if (m_cameraProperties.m_cameraMain.transform.eulerAngles.x < 360 - m_cameraProperties.m_maxCameraAng && m_cameraProperties.m_cameraMain.transform.eulerAngles.x > 180)
        {
            m_cameraProperties.m_cameraMain.transform.localEulerAngles = new Vector3(360 - m_cameraProperties.m_maxCameraAng, 0f, 0f);
        }
        else if (m_cameraProperties.m_camera.transform.eulerAngles.x > m_cameraProperties.m_maxCameraAng && m_cameraProperties.m_cameraMain.transform.eulerAngles.x < 180)
        {
            m_cameraProperties.m_cameraMain.transform.localEulerAngles = new Vector3(m_cameraProperties.m_maxCameraAng, 0f, 0f);
        }
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

    private void CalculateCurrentSpeed()
    {
        float speed = m_baseMovementProperties.m_baseMovementSpeed;

        speed += m_currentWallRunningSpeed;
        speed += m_currentWallClimbSpeed;
        speed += m_currentPostClamberSpeedBoost;
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

    private void OnLanded()
    {
        m_isLanded = true;
        m_hasJumped = false;

        m_canWallClimb = true;

        if (m_isCrouched)
        {
            OnSlideStart();
            return;
        }

        if (CheckBuffer(ref m_jumpBufferTimer, ref m_jumpingProperties.m_jumpBufferTime, m_jumpBufferCoroutine))
        {
            JumpMaxVelocity();
        }

        m_events.m_onLandedEvent.Invoke();
    }

    private void OnOffLedge()
    {
        m_offLedge = true;

        m_graceBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_graceTimer = (x), m_jumpingProperties.m_graceTime));

    }

    private void ZeroOnGroundCeiling()
    {
        if (IsGrounded() || HitCeiling())
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
            OnLanded();
        }
        if (!IsGrounded())
        {
            m_isLanded = false;
        }
    }

    private void CalculateVelocity()
    {
        if (m_states.m_gravityControllState == GravityState.GravityEnabled)
        {
            m_velocity.y += m_gravity * Time.deltaTime;
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
                    StartCoroutine(RunSlide());
                }
            }
        }
    }

    private IEnumerator RunSlide()
    {
        m_states.m_movementControllState = MovementControllState.MovementDisabled;
        m_isSliding = true;


        Vector3 slideDir = transform.forward;
        m_slideTimer = 0;

        bool hasBeenOnSlope = false;

        Vector3 slideSideShiftVelocity = Vector3.zero;
        Vector3 slideSideShiftVelocitySmoothing = Vector3.zero;

        while (m_slideTimer < m_slideProperties.m_slideTime)
        {
            m_slideTimer += Time.fixedDeltaTime;

            float progress = m_slideProperties.m_slideCurve.Evaluate(m_slideTimer / m_slideProperties.m_slideTime);

            float currentSlideSpeed = Mathf.Lerp(m_slideProperties.m_slideSpeed, m_baseMovementProperties.m_crouchMovementSpeed, progress);

            SlopeInfo slopeInfo = OnSlope();

            if (slopeInfo.m_onSlope)
            {
                hasBeenOnSlope = true;

                m_slideTimer = 0;

                float normalX = slopeInfo.m_slopeNormal.x > 0 ? slopeInfo.m_slopeNormal.x : slopeInfo.m_slopeNormal.x * -1;
                float normalZ = slopeInfo.m_slopeNormal.z > 0 ? slopeInfo.m_slopeNormal.z : slopeInfo.m_slopeNormal.z * -1;

                float slopeX = Mathf.Lerp(0, m_slideProperties.m_slideAngleBoostMax, normalX / m_slideProperties.m_slopeTolerence) * Mathf.Sign(slopeInfo.m_slopeNormal.x);
                float slopeZ = Mathf.Lerp(0, m_slideProperties.m_slideAngleBoostMax, normalZ / m_slideProperties.m_slopeTolerence) * Mathf.Sign(slopeInfo.m_slopeNormal.z);

                slideDir = new Vector3(slopeX, 0, slopeZ);
                Vector3 horizontalMovement = Vector3.SmoothDamp(m_velocity, slideDir, ref m_velocitySmoothing, m_slideProperties.m_slopeSlideAccelerationTime);

                m_slideProperties.m_slopeTransform.rotation = Quaternion.LookRotation(slideDir);

                Vector3 targetX = m_slideProperties.m_slopeTransform.right * m_movementInput.x * m_slideProperties.m_slideSideShiftMaxSpeed;
                Vector3 shiftVelX = Vector3.SmoothDamp(slideSideShiftVelocity, targetX, ref slideSideShiftVelocitySmoothing, m_slideProperties.m_slideSideShiftAcceleration);

                slideSideShiftVelocity = shiftVelX;

                m_velocity = new Vector3(horizontalMovement.x, m_velocity.y, horizontalMovement.z);

                m_velocity += slideSideShiftVelocity;
            }
            else if (!hasBeenOnSlope)
            {
                Vector3 slideVelocity = slideDir * currentSlideSpeed;
                m_velocity = new Vector3(slideVelocity.x, m_velocity.y, slideVelocity.z);
            }

            yield return new WaitForFixedUpdate();
        }

        m_isSliding = false;

        m_states.m_movementControllState = MovementControllState.MovementEnabled;
    }
    #endregion

    #region Crouch Code
    public void OnCrouchInputDown()
    {
        if (!m_isCrouched)
        {
            StartCoroutine(RunCrouchDown());
        }
        else
        {
            StartCoroutine(RunCrouchUp());
        }
    }

    private IEnumerator RunCrouchDown()
    {
        OnSlideStart();

        float t = 0;

        while (t < m_crouchProperties.m_crouchTime)
        {
            t += Time.fixedDeltaTime;

            float progress = t / m_crouchProperties.m_crouchTime;

            m_characterController.height = Mathf.Lerp(2, m_crouchProperties.m_crouchHeight, progress);

            yield return new WaitForFixedUpdate();
        }

        m_isCrouched = true;
    }

    private IEnumerator RunCrouchUp()
    {
        m_slideTimer = m_slideProperties.m_slideTime;

        m_isCrouched = false;

        float t = 0;

        while (t < m_crouchProperties.m_crouchTime)
        {
            t += Time.fixedDeltaTime;

            float progress = t / m_crouchProperties.m_crouchTime;

            m_characterController.height = Mathf.Lerp(m_crouchProperties.m_crouchHeight, 2, progress);

            yield return new WaitForFixedUpdate();
        }

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

        if (Physics.Raycast(bottom, Vector3.down, out hit, 0.5f))
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
                    m_velocity.x += (1f - hit.normal.y) * hit.normal.x * (m_baseMovementProperties.m_slopeFriction);
                    m_velocity.z += (1f - hit.normal.y) * hit.normal.z * (m_baseMovementProperties.m_slopeFriction);
                }

                m_characterController.Move(new Vector3(0, -(hit.distance), 0));
            }
        }
    }
    #endregion

    #region Jump Code
    public void OnJumpInputDown()
    {
        //m_holdingJumpInput = true;

        //m_jumpedDuringClamber = true;

        m_jumpBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_jumpBufferTimer = (x), m_jumpingProperties.m_jumpBufferTime));

        if (CheckBuffer(ref m_wallJumpBufferTimer, ref m_wallRunProperties.m_wallJumpBufferTime, m_wallJumpBufferCoroutine) && !m_isWallRunning)
        {
            //WallJump();
            return;
        }

        if (CheckBuffer(ref m_graceTimer, ref m_jumpingProperties.m_graceTime, m_graceBufferCoroutine) && !IsGrounded() && m_velocity.y <= 0f)
        {
            GroundJump();
            return;
        }

        if (m_isWallClimbing)
        {
            //WallRunningJump();
            return;
        }

        if (m_isWallRunning)
        {
            WallRunningJump();
            return;
        }

        if (IsGrounded())
        {
            GroundJump();
            return;
        }

    }

    public void OnJumpInputUp()
    {
        m_holdingJumpInput = false;

        if (m_velocity.y > m_minJumpVelocity)
        {
            JumpMinVelocity();
        }
    }

    private void CalculateJump()
    {
        m_gravity = -(2 * m_jumpingProperties.m_maxJumpHeight) / Mathf.Pow(m_jumpingProperties.m_timeToJumpApex, 2);
        m_maxJumpVelocity = Mathf.Abs(m_gravity) * m_jumpingProperties.m_timeToJumpApex;
        m_minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(m_gravity) * m_jumpingProperties.m_minJumpHeight);
    }

    private void WallJump()
    {
        m_events.m_onWallJumpEvent.Invoke();

        m_velocity.x = m_wallNormal.x * m_wallRunProperties.m_wallJumpVelocity.x;
        m_velocity.y = m_wallRunProperties.m_wallJumpVelocity.y;
        m_velocity.z = m_wallNormal.z * m_wallRunProperties.m_wallJumpVelocity.z;
    }

    private void WallRunningJump()
    {
        m_isWallRunning = false;

        StartCoroutine(InAirBoost(1f, 75f, transform.forward));
    }

    private void WallClimbingJump()
    {
        m_isWallClimbing = false;

        m_events.m_onWallClimbJumpEvent.Invoke();

        m_wallRunBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_wallRunBufferTimer = (x), m_wallRunProperties.m_wallRunBufferTime));

        m_velocity.x = m_wallNormal.x * m_wallClimbProperties.m_wallClimbJumpVelocity.x;
        m_velocity.y = m_wallClimbProperties.m_wallClimbJumpVelocity.y;
        m_velocity.z = m_wallNormal.z * m_wallClimbProperties.m_wallClimbJumpVelocity.z;
    }

    private void GroundJump()
    {
        m_events.m_onJumpEvent.Invoke();
        JumpMaxVelocity();
    }

    private void JumpMaxVelocity()
    {
        if (m_isCrouched)
        {
            StartCoroutine(RunCrouchUp());
        }

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