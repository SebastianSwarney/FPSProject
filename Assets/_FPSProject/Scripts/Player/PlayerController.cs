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
        [Header("Basic Wall Run Properties")]
        public float m_wallRunTolerenceMax;
        public float m_wallRunTolerenceMin;

        public float m_wallRunSpeed;
        public float m_wallRunSpeedUpTime;
        public AnimationCurve m_wallRunSpeedUpCurve;

        public float m_wallRunYVelocityStopTime;

        public float m_wallRunBufferTime;

        [Header("Wall Run Jump Properties")]
        public Vector2 m_wallRunJumpForce;
        public float m_wallRunJumpGroundVelocityDecayTime;
        public AnimationCurve m_wallRunJumpGroundVelocityDecayAnimationCurve;

        [Header("Camera Tilt Properties")]
        public float m_tiltSpeed;
        public float m_wallRunCameraMaxTilt;
    }

    [Header("Wall Run Properties")]
    public WallRunProperties m_wallRunProperties;
    public LayerMask m_wallConnectMask;

    private float m_tiltTarget;
    private bool m_isWallRunning;

    private Vector3 m_wallRunJumpVelocity;
    private Coroutine m_wallRunBufferCoroutine;
    private float m_wallRunBufferTimer;
    private float m_wallJumpOffFactor;
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
    private Vector3 m_slideVelocity;

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

    private bool m_isLongJumping;

    private bool m_maintainSpeed;

    public Vector2 m_slideJumpForce;

    private Vector3 m_explosionVelocity;

    public float m_explosionForce;
    public float m_explosionDecayTime;
    public AnimationCurve m_explosionDecayCurve;

    public float m_recoilRecoverSpeed;
    public float m_recoilMovementSpeed;

    public Transform m_recoilMovementY;
    public Transform m_recoilTargetY;
    public Transform m_recoilRecoverY;

    public Transform m_recoilMovementX;
    public Transform m_recoilTargetX;
    public Transform m_recoilRecoverX;

    public Transform m_shootingContainer;

    public float m_recoilLimit;

    private bool m_isShooting;

    private Quaternion m_endRotation;

    private bool m_recoilReset;

    public Transform m_finalPosContainer;

    private void Start()
    {
        m_characterController = GetComponent<CharacterController>();

        CalculateJump();
        LockCursor();

        m_currentMovementSpeed = m_baseMovementProperties.m_baseMovementSpeed;
        m_jumpBufferTimer = m_jumpingProperties.m_jumpBufferTime;

        m_wallRunBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_wallRunBufferTimer = (x), m_wallRunProperties.m_wallRunBufferTime));
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

        CheckWallConnection();

        //m_characterController.Move(m_velocity * Time.deltaTime);


        RecoilLerp(m_recoilMovementX, m_recoilTargetX, m_recoilRecoverX);
        //RecoilLerp(m_recoilMovementY, m_recoilTargetY, m_recoilRecoverY);

        CaculateTotalVelocity();

        SlopePhysics();

        ZeroOnGroundCeiling();
        CheckLanded();
        CheckOffLedge();

        CameraRotation();
        TiltLerp();
    }

    private void CaculateTotalVelocity()
    {
        Vector3 velocity = Vector3.zero;

        velocity += m_velocity;
        velocity += m_wallRunJumpVelocity;
        velocity += m_slideVelocity;
        velocity += m_explosionVelocity;

        m_characterController.Move(velocity * Time.fixedDeltaTime);
    }

    private IEnumerator RunExplosion(Vector3 p_explosionDir, float p_explosionForce, float p_explosionDecayTime)
    {
        float t = 0;

        while (t < p_explosionDecayTime)
        {
            t += Time.fixedDeltaTime;

            float decayProgress = m_explosionDecayCurve.Evaluate(t / p_explosionDecayTime);
            float currentExplosionForce = Mathf.Lerp(p_explosionForce, 0, decayProgress);

            Vector3 currentExplosionVelocity = p_explosionDir * currentExplosionForce;
            m_explosionVelocity = new Vector3(currentExplosionVelocity.x, currentExplosionVelocity.y, currentExplosionVelocity.z);

            yield return new WaitForFixedUpdate();
        }

        m_explosionVelocity = Vector3.zero;
    }

    public void OnShootInputDown()
    {
        m_isShooting = true;
    }

    public void OnShootInputUp()
    {
        RecoilReset();

        m_isShooting = false;
    }

	#region Wall Code
	private void CheckWallConnection()
    {
        bool anyRayHit = false;

        if (HitSide())
        {
            bool collidedTop = false;
            bool collidedBottom = false;

            Vector3 top = m_characterController.transform.position + new Vector3(0, m_characterController.height / 2, 0);
            Vector3 bottom = m_characterController.transform.position - new Vector3(0, m_characterController.height / 2, 0);

            RaycastHit hitTop;
            RaycastHit hitBottom;

            if (Physics.Raycast(top, transform.forward, out hitTop, 1f, m_wallClamberMask))
            {
                collidedTop = true;
            }

            if (Physics.Raycast(bottom, transform.forward, out hitBottom, 1f, m_wallClamberMask))
            {
                collidedBottom = true;
            }

            if (collidedBottom && !collidedTop)
            {
                //StartLongJump();
                //return;
            }

            if (collidedBottom && !collidedTop)
            {
                StartWallClamber();
                return;
            }

            if (collidedBottom && collidedTop)
            {
                StartWallClimb();
                return;
            }

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

                            if (wallFacingVector.y < (m_wallRunProperties.m_wallRunTolerenceMin * -1))
                            {
                                if (wallFacingVector.y > (m_wallRunProperties.m_wallRunTolerenceMax * -1))
                                {
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

    private void StartLongJump()
    {
        if (m_velocity.y > 0)
        {
            if (!m_isClambering)
            {
                if (!m_isLongJumping)
                {
                    StartCoroutine(RunLongJump());
                }
            }
        }
    }

    private IEnumerator RunLongJump()
    {
        JumpMaxMultiplied(0.5f);

        Vector3 movementDir = transform.forward;

        float t = 0;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime;

            Vector3 movementVelocity = movementDir * 30f;

            m_velocity = new Vector3(movementVelocity.x, m_velocity.y, movementVelocity.z);

            yield return new WaitForFixedUpdate();
        }
    }
	#endregion

	#region Wall Run Code
	private void StartWallRun(Vector3 p_dirToWallStart, float p_wallRunMovementDir)
    {
        if (!m_isWallRunning)
        {
            if (!IsGrounded())
            {
                if (CheckOverBuffer(ref m_wallRunBufferTimer, ref m_wallRunProperties.m_wallRunBufferTime, m_wallRunBufferCoroutine))
                {
                    StartCoroutine(RunWallRun(p_dirToWallStart, p_wallRunMovementDir));
                }
            }
        }
    }

    private void StopWallRun()
    {
        m_isWallRunning = false;
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

            float speedProgress = m_wallRunProperties.m_wallRunSpeedUpCurve.Evaluate(t / m_wallRunProperties.m_wallRunSpeedUpTime);
            float currentWallRunSpeed = Mathf.Lerp(speedStart, m_wallRunProperties.m_wallRunSpeed, speedProgress);

            float yProgress = t / m_wallRunProperties.m_wallRunYVelocityStopTime;
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
                    m_velocity = new Vector3(wallRunVelocity.x, wallRunVelocity.y, wallRunVelocity.z);

                    Vector3 wallFacingVector = Vector3.Cross(wallVector, m_cameraProperties.m_camera.transform.forward);
                    m_wallJumpOffFactor = wallFacingVector.y;
                }
                else
                {
                    StopWallRun();
                }
            }
            else
            {
                StopWallRun();
            }

            yield return new WaitForFixedUpdate();
        }

        m_tiltTarget = 0f;

        m_states.m_movementControllState = MovementControllState.MovementEnabled;
        m_states.m_gravityControllState = GravityState.GravityEnabled;

        m_wallRunBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_wallRunBufferTimer = (x), m_wallRunProperties.m_wallRunBufferTime));

        m_isWallRunning = false;
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

        m_states.m_gravityControllState = GravityState.GravityDisabled;

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

        m_states.m_gravityControllState = GravityState.GravityEnabled;

        m_isClambering = false;
    }
	#endregion

	#region Wall Climb Code
    private void StartWallClimb()
    {
        if (m_hasJumped)
        {
            if (!m_isWallRunning)
            {
                if (!m_isClimbing)
                {
                    if (m_canWallClimb)
                    {
                        StartCoroutine(RunWallClimb());
                    }
                }
            }
        }
    }

    private void StopWallClimb()
    {
        m_isClimbing = false;
    }

	private IEnumerator RunWallClimb()
    {
        m_isClimbing = true;

        m_states.m_gravityControllState = GravityState.GravityDisabled;

        float t = 0;

        while (t < m_climbTime && m_isClimbing)
        {
            t += Time.fixedDeltaTime;

            float progress = m_climbSpeedDecreaseCurve.Evaluate(t / m_climbTime);

            Vector3 top = m_characterController.transform.position + new Vector3(0, m_characterController.height / 2, 0);

            RaycastHit hitTop;

            if (!Physics.Raycast(top, transform.forward, out hitTop, Mathf.Infinity, m_wallClamberMask))
            {
                t = m_climbTime;

                StartWallClimb();
            }

            if (!HitSide())
            {
                t = m_climbTime;
            }


            float currentClimbSpeed = Mathf.Lerp(m_climbSpeed, 0, progress);

            m_velocity.y = currentClimbSpeed;

            yield return new WaitForFixedUpdate();
        }

        m_states.m_gravityControllState = GravityState.GravityEnabled;

        m_canWallClimb = false;

        m_isClimbing = false;
    }
	#endregion

	#region Misc Wall Code
	private void TiltLerp()
    {
        m_cameraProperties.m_cameraTilt.localRotation = Quaternion.Slerp(m_cameraProperties.m_cameraTilt.localRotation, Quaternion.Euler(0, 0, m_tiltTarget), m_wallRunProperties.m_tiltSpeed);
    }

    private IEnumerator InAirBoost(float p_yVelocity, float p_forwardSpeed, Vector3 p_movementDirection)
    {
        JumpMaxMultiplied(p_yVelocity);

        while (!IsGrounded() && !m_isWallRunning)
        {
            Vector3 movementVelocity = p_movementDirection * p_forwardSpeed;

            m_wallRunJumpVelocity = new Vector3(movementVelocity.x, 0, movementVelocity.z);

            yield return new WaitForFixedUpdate();
        }

        if (IsGrounded() && !m_isWallRunning)
        {
            StartCoroutine(OnGroundBoost(p_forwardSpeed, p_movementDirection));
        }

        m_wallRunJumpVelocity = Vector3.zero;
    }

    private IEnumerator OnGroundBoost(float p_forwardSpeed, Vector3 p_movementDirection)
    {
        float t = 0;

        while (t < m_wallRunProperties.m_wallRunJumpGroundVelocityDecayTime)
        {
            if (IsGrounded() && !m_maintainSpeed)
            {
                t += Time.fixedDeltaTime;
            }

            float speedProgress = m_wallRunProperties.m_wallRunJumpGroundVelocityDecayAnimationCurve.Evaluate(t / m_wallRunProperties.m_wallRunJumpGroundVelocityDecayTime);
            float currentSpeed = Mathf.Lerp(p_forwardSpeed, 0, speedProgress);

            Vector3 movementVelocity = p_movementDirection * currentSpeed;
            m_wallRunJumpVelocity = movementVelocity;

            yield return new WaitForFixedUpdate();
        }

        m_wallRunJumpVelocity = Vector3.zero;
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
    #endregion

    #region Camera Code
    public void AddRecoil(float p_recoilAmountX, float p_recoilAmountY)
    {
        RotateCameraAxisX(p_recoilAmountX, m_recoilTargetX, m_cameraProperties.m_maxCameraAng);
        RotateCameraAxisY(p_recoilAmountY, m_recoilTargetY, m_cameraProperties.m_maxCameraAng);
    }

    private void RecoilLerp(Transform p_recoilMovement, Transform p_recoilTarget, Transform p_recoilRest)
    {
        if (!m_isShooting)
        {
            //p_recoilTarget.rotation = p_recoilRest.rotation;
            p_recoilMovement.rotation = Quaternion.Slerp(p_recoilMovement.rotation, p_recoilRest.rotation, m_recoilRecoverSpeed * Time.fixedDeltaTime);
        }
        else
        {
            p_recoilMovement.rotation = Quaternion.Slerp(p_recoilMovement.rotation, p_recoilTarget.rotation, m_recoilMovementSpeed * Time.fixedDeltaTime);
        }
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ResetCamera()
    {
        m_cameraProperties.m_cameraMain.rotation = Quaternion.identity;
        m_cameraProperties.m_cameraTilt.rotation = Quaternion.identity;
    }

    private void RecoilReset()
    {
        m_cameraProperties.m_cameraMain.rotation = m_shootingContainer.rotation;
        
        if (m_recoilReset)
        {
            m_recoilMovementX.rotation = m_shootingContainer.rotation;
        }
        else
        {
            m_recoilMovementX.rotation = m_finalPosContainer.rotation;
        }

        m_recoilTargetX.rotation = m_recoilRecoverX.rotation;

        //m_recoilTargetX.rotation = m_shootingContainer.rotation;
    }

    private void CameraRotation()
    {
        //Get the inputs for the camera
        Vector2 cameraInput = new Vector2(m_lookInput.y * ((m_cameraProperties.m_inverted) ? -1 : 1), m_lookInput.x);

        //Rotate the player on the y axis (left and right)
        transform.Rotate(Vector3.up, cameraInput.y * (m_cameraProperties.m_mouseSensitivity));

        float xRotateAmount = cameraInput.x * m_cameraProperties.m_mouseSensitivity;


        m_finalPosContainer.rotation = m_recoilMovementX.rotation;

        if (m_isShooting)
        {
            RotateCameraAxisX(xRotateAmount, m_recoilTargetX, m_cameraProperties.m_maxCameraAng);

            if (xRotateAmount > 0)
            {
                m_recoilReset = true;
                m_shootingContainer.rotation = Quaternion.LookRotation(m_cameraProperties.m_camera.transform.forward);
            }
            else
            {
                m_recoilReset = false;
            }
        }
        else
        {
            RotateCameraAxisX(xRotateAmount, m_cameraProperties.m_cameraMain, m_cameraProperties.m_maxCameraAng);
        }
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

        m_maintainSpeed = true;

        m_velocity = Vector3.zero;

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
                Vector3 calculatedSlideVelocity = slideDir * currentSlideSpeed;
                m_slideVelocity = new Vector3(calculatedSlideVelocity.x, 0, calculatedSlideVelocity.z);
            }

            yield return new WaitForFixedUpdate();
        }

        m_slideVelocity = Vector3.zero;

        m_maintainSpeed = false;

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
        StopWallRun();

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
        m_holdingJumpInput = true;

        m_jumpedDuringClamber = true;

        m_jumpBufferCoroutine = StartCoroutine(RunBufferTimer((x) => m_jumpBufferTimer = (x), m_jumpingProperties.m_jumpBufferTime));

        if (m_isSliding)
        {
            SlideJump();
        }

        if (CheckBuffer(ref m_graceTimer, ref m_jumpingProperties.m_graceTime, m_graceBufferCoroutine) && !IsGrounded() && m_velocity.y <= 0f)
        {
            GroundJump();
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

    private void SlideJump()
    {
        StartCoroutine(InAirBoost(m_slideJumpForce.y, m_slideJumpForce.x, transform.forward));

        StartCoroutine(RunCrouchUp());

        m_hasJumped = true;
    }

    private void WallRunningJump()
    {
        if (m_wallJumpOffFactor > 0.1f)
        {
            StopWallRun();
            StartCoroutine(InAirBoost(m_wallRunProperties.m_wallRunJumpForce.y, m_wallRunProperties.m_wallRunJumpForce.x, transform.forward));
        }
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