using UnityEngine;
using UnityEngine.UI;

namespace Aircraft_Physics.Example.Scripts
{
    public class AutopilotHeading : MonoBehaviour
    {
        //Calculation parameters
        private float _targetHeading;
        private float _targetTurningSpeed;
        private float _targetPlaneRoll;
        private float _currentTurnSpeed;
        private float _aileronOutput;
        private float TurnSpeed
        {
            get => _currentTurnSpeed;
            set
            {
                TurnSpeedLast = _currentTurnSpeed;
                _currentTurnSpeed = value;
            }
        }

        private float TurnSpeedLast { get; set; } = 0;
        
        //PID parameters
        [SerializeField] private PIDConfig turnSpeedPidConfig, rollPidConfig;

        //PID object
        private PidController _controlSurfacePid, _rollPid;

        //Other references
        private AirplaneController _controller;
        private Rigidbody _aircraft;
        [SerializeField] private Slider slider;
        
        private void Awake()
        {
            //setup PID
            _controlSurfacePid = new PidController();
            _controlSurfacePid.SetConstants(turnSpeedPidConfig.gainProportional, turnSpeedPidConfig.gainIntegral, turnSpeedPidConfig.gainDerivative, turnSpeedPidConfig.outputMin, turnSpeedPidConfig.outputMax);
            _rollPid = new PidController();
            _rollPid.SetConstants(rollPidConfig.gainProportional, rollPidConfig.gainIntegral, rollPidConfig.gainDerivative, rollPidConfig.outputMin, rollPidConfig.outputMax);

            //get other references
            _controller = GetComponent<AirplaneController>();
            _aircraft = GetComponent<Rigidbody>();
            
            //disable
            enabled = false;
        }

        private void OnEnable()
        {
            _controlSurfacePid.Reset();
        }

        private void Update()
        {
            _targetHeading = slider.value;
        }

        private void FixedUpdate()
        {
            //update parameters (for tuning)
            _controlSurfacePid.SetConstants(turnSpeedPidConfig.gainProportional, turnSpeedPidConfig.gainIntegral, turnSpeedPidConfig.gainDerivative, turnSpeedPidConfig.outputMin, turnSpeedPidConfig.outputMax);
            _rollPid.SetConstants(rollPidConfig.gainProportional, rollPidConfig.gainIntegral, rollPidConfig.gainDerivative, rollPidConfig.outputMin, rollPidConfig.outputMax);

            //calculations
            _targetTurningSpeed = CalcTargetTurnSpeed(CalcHeadingError(_targetHeading, GETHeading()));

            //run double PID controllers
            _targetPlaneRoll = PID_Update(_rollPid, _targetTurningSpeed, GETTurnSpeed(), Time.fixedDeltaTime);
            _aileronOutput = -PID_Update(_controlSurfacePid, _targetPlaneRoll, GETRoll(), Time.fixedDeltaTime) / 10f;
            
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
            TurnSpeed = GETHeading();
            float temp = CalcAngularVelocity(TurnSpeed, TurnSpeedLast); //this is its y axis turning angular velocity in degrees per second
            return (float) System.Math.Round(temp * 60f, 2); //rounds it to a float with 1 decimal point
        }
        
        public float GETRoll()
        {
            return _aircraft.rotation.eulerAngles.z;
        }
        
        public static float CalcAngularVelocity(float currentAngle, float lastAngle)
        {
            float deltaRotation = currentAngle - lastAngle;
            float velocity = deltaRotation / Time.fixedDeltaTime;

            return velocity;
        }
    }
}