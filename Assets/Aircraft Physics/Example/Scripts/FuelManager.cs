using Aircraft_Physics.Core.Scripts;
using UnityEngine;

namespace Aircraft_Physics.Example.Scripts
{
    public class FuelManager : MonoBehaviour
    {
        [SerializeField]
        private float fuel;
        [SerializeField]
        private float maxFuel = 53f;
        [SerializeField]
        private float fuelPerMilePerThrust = -0.004608f;

        private int _fuelPercent;

        private AirplaneController _controller;
        // Start is called before the first frame update
        private void Start()
        {
            _controller = GetComponent<AirplaneController>();
            _fuelPercent = 100;
            VariableMassManager.Instance.PeopleMass(1);
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
            int newFuelPercent = FuelToPercent(fuel);
            if (newFuelPercent != _fuelPercent)
            {
                _fuelPercent = newFuelPercent;
                //change weight of the wings
                VariableMassManager.Instance.WingMass(fuel);
            }

            //start / stop plane
            if (fuel <= 0)
            {
                _controller.enabled = false;
                //VariableMassManager.Instance.PeopleMass(0); //if the plane is on, assume the player is on the plane
            } else if (Input.GetKeyDown(KeyCode.R)) //enable if has fuel, disabled, and want to start engine
            {
                _controller.enabled = !_controller.enabled;
                //VariableMassManager.Instance.PeopleMass(_controller.enabled ? 1 : 0); //if the plane is on, assume the player is on the plane
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
        
        private int FuelToPercent(float inputFuel)
        {
            return Mathf.RoundToInt(inputFuel / maxFuel * 100);
        }
    }
}
