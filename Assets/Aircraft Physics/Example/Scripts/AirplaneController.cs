using System.Collections.Generic;
using Aircraft_Physics.Core.Scripts;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Aircraft_Physics.Example.Scripts
{
    public class AirplaneController : MonoBehaviour
    {
        [SerializeField] private List<AeroSurface> controlSurfaces = null;
        [SerializeField] private List<WheelCollider> wheels = null;

        //Sensitivites
        [SerializeField] private float thrustControlSensitivity;
        [SerializeField] private float flapControlSensitivity;
        [SerializeField] private float rollControlSensitivity = 0.2f;
        [SerializeField] private float pitchControlSensitivity = 0.2f;
        [SerializeField] private float yawControlSensitivity = 0.2f;

        //runtime variables
        [Range(-1, 1)] public float pitch;
        [Range(-1, 1)] public float yaw;
        [Range(-1, 1)] public float roll;
        [Range(0, 1)] public float flap;
        [Range(0, 1)] public float thrust;
        public float brakesTorque;

        //other refs
        [SerializeField] private Text displayText = null;

        private AircraftPhysics _aircraftPhysics;
        private Rigidbody _rb;
        private AutopilotAltitude _autopilotAltitude;
        private AutopilotHeading _autopilotHeading;

        private void Start()
        {
            _aircraftPhysics = GetComponent<AircraftPhysics>();
            _rb = GetComponent<Rigidbody>();
            _autopilotAltitude = GetComponent<AutopilotAltitude>();
            _autopilotHeading = GetComponent<AutopilotHeading>();
        }

        private void OnDisable()
        {
            thrust = 0f;
            _autopilotAltitude.enabled = false;
            _autopilotHeading.enabled = false;
        }

        private void Update()
        {
            if (!_autopilotAltitude.enabled)
            {
                pitch = Input.GetAxis("Vertical");
            }

            if (!_autopilotHeading.enabled)
            {
                roll = Input.GetAxis("Horizontal");
            }

            yaw = Input.GetAxis($"Yaw");

            if (Input.GetKeyDown(KeyCode.B))
            {
                brakesTorque = brakesTorque > 0 ? 0 : 100f;
            }

            //reverses thrust and flap control
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftShift))
            {
                flapControlSensitivity *= -1;
                thrustControlSensitivity *= -1;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                thrust += thrustControlSensitivity;
                thrust = Mathf.Clamp01(thrust);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                flap += flapControlSensitivity;
                flap = Mathf.Clamp01(flap);
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                _autopilotAltitude.enabled = !_autopilotAltitude.enabled; //flips it
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                _autopilotHeading.enabled = !_autopilotHeading.enabled; //flips it
            }

            displayText.text = "V: " + ((int) _rb.velocity.magnitude).ToString("D3") + " m/s\n"
                               + "Alt: " + ((int) transform.position.y).ToString("D4") + " m\n"
                               + "T: " + (int) (thrust * 100) + "%\n"
                               + (brakesTorque > 0 ? "B: ON\n" : "B: OFF\n")
                               + (_autopilotAltitude.enabled ? "A: ON" : "A: OFF")
                               + "  " + Mathf.Round(_autopilotAltitude.GetVerticalSpeed())
                               + (_autopilotHeading.enabled ? "H: ON" : "H: OFF")
                               + "  " + Mathf.Round(_autopilotHeading.GETHeading());
        }

        private void FixedUpdate()
        {
            SetControlSurfacesAngles(pitch, roll, yaw, flap);
            _aircraftPhysics.SetThrustPercent(thrust);
            foreach (var wheel in wheels)
            {
                wheel.brakeTorque = brakesTorque;
                // small torque to wake up wheel collider
                wheel.motorTorque = 0.01f;
            }
        }

        /// <summary>
        /// Sets the control surface angles
        /// </summary>
        /// <param name="pitch">elevators</param>
        /// <param name="roll">ailerons</param>
        /// <param name="yaw">rudder</param>
        /// <param name="flap">flaps</param>
        public void SetControlSurfacesAngles(float pitch, float roll, float yaw, float flap)
        {
            foreach (var surface in controlSurfaces)
            {
                if (surface is null || !surface.IsControlSurface) continue;
                switch (surface.InputType)
                {
                    case ControlInputType.Pitch:
                        surface.SetFlapAngle(pitch * pitchControlSensitivity * surface.InputMultiplyer);
                        break;
                    case ControlInputType.Roll:
                        surface.SetFlapAngle(roll * rollControlSensitivity * surface.InputMultiplyer);
                        break;
                    case ControlInputType.Yaw:
                        surface.SetFlapAngle(yaw * yawControlSensitivity * surface.InputMultiplyer);
                        break;
                    case ControlInputType.Flap:
                        surface.SetFlapAngle(flap * surface.InputMultiplyer);
                        break;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                SetControlSurfacesAngles(pitch, roll, yaw, flap);
        }
    }
}