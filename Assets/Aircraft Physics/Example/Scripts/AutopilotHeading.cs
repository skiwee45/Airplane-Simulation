using UnityEngine;
using UnityEngine.UI;

namespace Aircraft_Physics.Example.Scripts
{
    public class AutopilotHeading : MonoBehaviour
    {
        //Calculation parameters
        [SerializeField]
        private float _targetHeading;
        [SerializeField]
        private float _targetTurningSpeed;
        [SerializeField] 
        private float _currentTurnSpeed;
        [SerializeField]
        private float _targetPlaneRoll;
        [SerializeField]
        private float _currentRoll;
        [SerializeField]
        private float _aileronOutput;

        // private Quaternion Rotation
        // {
        //     get => currentRotation;
        //     set
        //     {
        //         RotationLast = currentRotation;
        //         currentRotation = value;
        //     }
        // }
        //
        // private Quaternion RotationLast { get; set; }
        
        //PID parameters
        [SerializeField] private PidConfig rollPidConfig, aileronPidConfig;

        //PID object
        private PidController _aileronPidController, _rollPid;

        //Other references
        private AirplaneController _controller;
        private Rigidbody _aircraft;
        [SerializeField] private Slider slider;
        
        private void Awake()
        {
            //setup PID
            _rollPid = new PidController();
            _rollPid.SetConstants(aileronPidConfig.gainProportional, aileronPidConfig.gainIntegral, aileronPidConfig.gainDerivative, aileronPidConfig.outputMin, aileronPidConfig.outputMax);
            _aileronPidController = new PidController();
            _aileronPidController.SetConstants(rollPidConfig.gainProportional, rollPidConfig.gainIntegral, rollPidConfig.gainDerivative, rollPidConfig.outputMin, rollPidConfig.outputMax);

            //get other references
            _controller = GetComponent<AirplaneController>();
            _aircraft = GetComponent<Rigidbody>();
            
            //disable
            enabled = false;
        }

        private void OnEnable()
        {
            _aileronPidController.Reset();
        }

        private void Update()
        {
            _targetHeading = slider.value;
        }

        private void FixedUpdate()
        {
            //update parameters (for tuning)
            _aileronPidController.SetConstants(aileronPidConfig.gainProportional, aileronPidConfig.gainIntegral, aileronPidConfig.gainDerivative, aileronPidConfig.outputMin, aileronPidConfig.outputMax);
            _rollPid.SetConstants(rollPidConfig.gainProportional, rollPidConfig.gainIntegral, rollPidConfig.gainDerivative, rollPidConfig.outputMin, rollPidConfig.outputMax);

            //calculations
            _targetTurningSpeed = CalcTargetTurnSpeed(CalcHeadingError(_targetHeading, GETHeading()));

            //run double PID controllers
            _targetPlaneRoll = PID_Update(_rollPid, _targetTurningSpeed, GETTurnSpeed(), Time.fixedDeltaTime);
            _aileronOutput = PID_Update(_aileronPidController, _targetPlaneRoll, GETRoll(), Time.fixedDeltaTime);
            
            //put final output into controller
            _controller.roll = _aileronOutput;
        }

        /// <summary>
        /// Runs any PID controller
        /// </summary>
        /// <param name="controller">Controller to run on</param>
        /// <param name="setPoint"></param>
        /// <param name="processVariable"></param>
        /// <param name="deltaTime"></param>
        /// <returns>output</returns>
        private static float PID_Update(PidController controller, float setPoint, float processVariable, float deltaTime)
        {
            controller.SetPoint = setPoint;
            controller.ProcessVariable = processVariable;
            return controller.ControlVariable(deltaTime);
        }

        /// <summary>
        /// Calculates turn speed using quadratic equation with restrictive clamp
        /// Last if statement is to make sure it is accurate when headingError is negative
        /// </summary>
        /// <param name="headingError"></param>
        /// <returns>target turn speed in degrees per second: between 6 d/s and 1 d/s</returns>
        private static float CalcTargetTurnSpeed(float headingError)
        {
            var targetSpeed = 0.01f * Mathf.Pow(headingError, 2);
            targetSpeed = Mathf.Clamp(targetSpeed, 1, 6);
            return (headingError >= 0 ? targetSpeed : -targetSpeed);
        }

        /// <summary>
        /// Calculates headingError from euler angles to normalized angles
        /// </summary>
        /// <param name="targetHeading"></param>
        /// <param name="currentHeading"></param>
        /// <returns>error between current and target heading, - is left, + is right. Between -180 and 180</returns>
        private static float CalcHeadingError(float targetHeading, float currentHeading)
        {
            var turnAngle = 0f;

            //get the delta of targetHeading and currentHeading
            var deltaAngle = targetHeading - currentHeading;
            
            //find out to go left or right
            if (deltaAngle < -180)
            {
                turnAngle = deltaAngle + 360;
            }
            else if (deltaAngle >= -180 && deltaAngle <= 180)
            {
                turnAngle = deltaAngle;
            }
            else if (deltaAngle > 180)
            {
                turnAngle = deltaAngle - 360;
            }
            return turnAngle;
        }

        public float GETHeading()
        {
            return _aircraft.rotation.eulerAngles.y;
        }
        
        public float GETTurnSpeed()
        {
            _currentTurnSpeed = _aircraft.angularVelocity.y * Mathf.Rad2Deg;
            return (float) System.Math.Round(_currentTurnSpeed, 2); //rounds it to a float with 2 decimal points
        }
        
        public float GETRoll()
        {
            _currentRoll = NormalizeEulerAngle(transform.eulerAngles.z);
            return _currentRoll;
        }

        public float NormalizeEulerAngle(float angle)
        {
            var normalizedAngle = -angle;
            if (angle > 180)
            {
                normalizedAngle = 360 - angle;
            }
            return normalizedAngle;
        }
    }
}