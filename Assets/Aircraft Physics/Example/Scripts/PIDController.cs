﻿using UnityEngine;

namespace Aircraft_Physics.Example.Scripts
{
    public class PidController
    {
        private float _error = 0;

        public void SetConstants(float gainProportional, float gainIntegral, float gainDerivative, float outputMin,
            float outputMax)
        {
            this.GainDerivative = gainDerivative;
            this.GainIntegral = gainIntegral;
            this.GainProportional = gainProportional;
            this.OutputMax = outputMax;
            this.OutputMin = outputMin;
        }
        
        public float ControlVariable(float timeSinceLastUpdate)
        {
            Error = SetPoint - ProcessVariable;

            // integral term calculation
            IntegralTerm += (GainIntegral * _error * timeSinceLastUpdate);
            IntegralTerm = Mathf.Clamp(IntegralTerm, OutputMin, OutputMax);

            // derivative term calculation
            float dInput = Error - ErrorLast;
            float derivativeTerm = GainDerivative * (dInput / timeSinceLastUpdate);

            // proportional term calculation
            float proportionalTerm = GainProportional * _error;

            float output = proportionalTerm + IntegralTerm + derivativeTerm;

            output = Mathf.Clamp(output, OutputMin, OutputMax);;

            return output;
        }

        public void Reset()
        {
            IntegralTerm = 0f;
            ProcessVariable = 0f;
            Error = 0f;
            ErrorLast = 0f;
        }
        
        public float GainDerivative { get; set; } = 0;

        public float GainIntegral { get; set; } = 0;
        
        public float GainProportional { get; set; } = 0;
        
        public float OutputMax { get; private set; } = 0;

        public float OutputMin { get; private set; } = 0;

        public float IntegralTerm { get; private set; } = 0;
        public float Error
        {
            get { return _error; }
            set
            {
                ErrorLast = _error;
                _error = value;
            }
        }
        public float ErrorLast { get; private set; } = 0;
        public float SetPoint { get; set; } = 0;
        public float ProcessVariable { get; set; } = 0;
    }
}