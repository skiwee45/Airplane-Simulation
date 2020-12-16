﻿using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Aircraft_Physics.Example.Scripts
{
    public class AutopilotAltitude : MonoBehaviour
    {
        //PID parameters
        [SerializeField] private PidConfig pidConfig;
        [Range(-1000, 1000)]
        [SerializeField] private float setPoint;
        [SerializeField] private float output;

        //PID object
        private PidController _pid;

        //Other references
        private AirplaneController _controller;
        private Rigidbody _aircraft;
        [SerializeField] private Slider slider;
        
        private void Awake()
        {
            //setup PID
            _pid = new PidController();
            _pid.SetConstants(pidConfig.gainProportional, pidConfig.gainIntegral, pidConfig.gainDerivative, pidConfig.outputMin, pidConfig.outputMax);
            
            //get other references
            _controller = GetComponent<AirplaneController>();
            _aircraft = GetComponent<Rigidbody>();
            
            //disable
            enabled = false;
        }

        private void OnEnable()
        {
            _pid.Reset();
        }

        private void Update()
        {
            setPoint = slider.value * 100f;
        }

        private void FixedUpdate()
        {
            //update parameters (for tuning)
            _pid.SetConstants(pidConfig.gainProportional, pidConfig.gainIntegral, pidConfig.gainDerivative, pidConfig.outputMin, pidConfig.outputMax);
            
            //run PID controller
            output = -PID_Update(setPoint, GetVerticalSpeed(), Time.fixedDeltaTime) / 10f;
            _controller.pitch = output;
        }

        private float PID_Update(float setPoint, float processVariable, float deltaTime)
        {
            //set the goal to whatever the slider is
            _pid.SetPoint = setPoint;

            //set the process variable
            _pid.ProcessVariable = processVariable;

            //call the function to run PID
            return _pid.ControlVariable(deltaTime);
        }

        public float GetVerticalSpeed()
        {
            float temp = _aircraft.velocity.y * 196.85f; //m/s converted to ft/min (standard aviation unit in the US)
            return temp;
        }
    }
}