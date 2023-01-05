using Aircraft_Physics.Core.Scripts;
using Aircraft_Physics.Core.Scripts.CenterOfMass;
using ColliderAddon;
using NaughtyAttributes;
using UnityEngine;
using System;

namespace Aircraft_Physics.Example.Scripts
{
    [RequireComponent(typeof(MassChanger))]
    public class FuelManager : MonoBehaviour
    {
        [BoxGroup("Fuel")]
        [SerializeField]
        [ReadOnly]
        [Label("Fuel in Gallons")]
        private float fuel;
        [BoxGroup("Fuel")]
        [SerializeField]
        private float maxFuel = 53f;
        [BoxGroup("Fuel")]
        [SerializeField]
        private float fuelPerMilePerThrust = -0.004608f;
        
        [BoxGroup("Weight Constants")]
        [SerializeField]
        [Label("Kg / Gallon of Fuel")]
        private float kgPerGallonOfFuel = 2.4528f;
        [BoxGroup("Weight Constants")]
        [SerializeField]
        [Label("Kg / Person")]
        private float kgPerPerson = 75f;

        private int _fuelPercent;

        private AirplaneController _controller;
        private MassChanger _massChanger;
        
        // Start is called before the first frame update
        private void Start()
        {
            _massChanger = GetComponent<MassChanger>();
            _controller = GetComponent<AirplaneController>();
        }

        // Update is called once per frame
        private void Update()
        {
            //fuel calculations
            fuel += CalculateFuel(_controller.thrust);
            if (Input.GetKey(KeyCode.LeftControl))
            {
                fuel += 0.5f;
            }
            fuel = Mathf.Clamp(fuel, 0f, maxFuel);
            //add fuel mass
            var newFuelPercent = FuelToPercent();
            var oldFuelPercent = _fuelPercent;
            if (newFuelPercent != oldFuelPercent)
            {
                _fuelPercent = newFuelPercent;
                //change weight of the wings
                _massChanger.SetMassAboveMinimum(AirplaneColliderType.Wings, fuel * kgPerGallonOfFuel);
            }

            //start / stop plane
            if (fuel <= 0 && _controller.enabled) //stop plane
            {
                _controller.enabled = false;
                _massChanger.SetMassAboveMinimum(AirplaneColliderType.FuselageFront, 0);
            } else if (Input.GetKeyDown(KeyCode.R)) //enable if has fuel, disabled, and want to start engine
            {
                var controllerEnabled = _controller.enabled;
                controllerEnabled = !controllerEnabled;
                _controller.enabled = controllerEnabled;
                _massChanger.SetMassAboveMinimum(AirplaneColliderType.FuselageFront,
                    controllerEnabled ? kgPerPerson : 0);
            }
        }

        /// <summary>
        /// based on engine input calculate how much fuel was lost
        /// </summary>
        /// <param name="thrust"></param>
        /// <returns></returns>
        private float CalculateFuel(float thrust)
        {
            return thrust * fuelPerMilePerThrust * Time.deltaTime;
        }
        
        private int FuelToPercent()
        {
            return Mathf.RoundToInt((fuel / maxFuel) * 100f);
        }
    }
}
