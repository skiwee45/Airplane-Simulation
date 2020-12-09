using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Aircraft_Physics.Example.Scripts
{
    public class AirplaneController : MonoBehaviour
    {
        [SerializeField]
        List<AeroSurface> controlSurfaces = null;
        [SerializeField]
        List<WheelCollider> wheels = null;
    
        //Sensitivites
        [SerializeField] 
        float thrustControlSensitivity;
        [SerializeField] 
        float flapControlSensitivity;
        [SerializeField]
        float rollControlSensitivity = 0.2f;
        [SerializeField]
        float pitchControlSensitivity = 0.2f;
        [SerializeField]
        float yawControlSensitivity = 0.2f;

        //runtime variables
        [Range(-1, 1)]
        public float Pitch;
        [Range(-1, 1)]
        public float Yaw;
        [Range(-1, 1)]
        public float Roll;
        [Range(0, 1)]
        public float Flap;
        [FormerlySerializedAs("thrustPercent")] [Range(0, 1)]
        public float Thrust;
        public float brakesTorque;
    
        //other refs
        [SerializeField]
        Text displayText = null;

        AircraftPhysics aircraftPhysics;
        Rigidbody rb;
        private AutopilotAltitude autopilotAltitude;

        private void Start()
        {
            aircraftPhysics = GetComponent<AircraftPhysics>();
            rb = GetComponent<Rigidbody>();
            autopilotAltitude = GetComponent<AutopilotAltitude>();
        }

        private void OnDisable()
        {
            Thrust = 0f;
        }

        private void Update()
        {
            if (!autopilotAltitude.enabled)
            {
                Pitch = Input.GetAxis("Vertical");
            }
            Roll = Input.GetAxis("Horizontal");
            Yaw = Input.GetAxis($"Yaw");

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
                Thrust += thrustControlSensitivity;
                Thrust = Mathf.Clamp01(Thrust);
            }
        
            if (Input.GetKeyDown(KeyCode.F))
            {
                Flap += flapControlSensitivity;
                Flap = Mathf.Clamp01(Flap);
            }
        
            if (Input.GetKeyDown(KeyCode.V))
            {
                autopilotAltitude.enabled = !autopilotAltitude.enabled; //flips it
            }

            displayText.text = "V: " + ((int)rb.velocity.magnitude).ToString("D3") + " m/s\n"
            + "Alt: " + ((int)transform.position.y).ToString("D4") + " m\n"
            + "T: " + (int)(Thrust * 100) + "%\n"
            + (brakesTorque > 0 ? "B: ON\n" : "B: OFF\n")
            + (autopilotAltitude.enabled ? "AP: ON" : "AP: OFF")
            + "  " + Mathf.Round(autopilotAltitude.GetVerticalSpeed());
        }

        private void FixedUpdate()
        {
            SetControlSurfacesAngles(Pitch, Roll, Yaw, Flap);
            aircraftPhysics.SetThrustPercent(Thrust);
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
                if (surface == null || !surface.IsControlSurface) continue;
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
                SetControlSurfacesAngles(Pitch, Roll, Yaw, Flap);
        }
    }
}
