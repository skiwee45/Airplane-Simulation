using UnityEngine;

namespace Aircraft_Physics.Example.Scripts
{
    public class FuelManager : MonoBehaviour
    {
        [SerializeField]
        private float fuel;

        private AirplaneController _controller;
    
        // Start is called before the first frame update
        private void Start()
        {
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
        
            fuel = Mathf.Clamp(fuel, 0f, 53f);

            //start / stop plane
            if (fuel <= 0)
            {
                _controller.enabled = false;
            } else if (Input.GetKey(KeyCode.R)) //enable if has fuel, disabled, and want to start engine
            {
                _controller.enabled = true;
            }
            
        }

        /// <summary>
        /// based on engine input calculate how much fuel was lost
        /// </summary>
        /// <param name="thrust"></param>
        /// <returns></returns>
        private static float CalculateFuel(float thrust)
        {
            return thrust * -0.4608f * Time.deltaTime;
        }
    }
}
