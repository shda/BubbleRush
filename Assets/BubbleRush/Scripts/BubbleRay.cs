using System;
using UnityEngine;
using UnityEngine.Pool;

namespace BubbleRush.Scripts
{
    public class BubbleRay : MonoBehaviour
    {
        [SerializeField] private SimulationBody simulationBody;
        [SerializeField] private Transform rootRotate;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int maxIterations = 49;
        [SerializeField] private float simulationStep = 0.04f;

        private Vector3[] _points = Array.Empty<Vector3>();
        private Vector2 _velocity;
        private bool _isSimulate;

        public void StartSimulate()
        {
            //_velocity = velocity;
            _isSimulate = true;
            SwitchSimulations();
        }

        public void StopSimulate()
        {
            _isSimulate = false;
            SwitchSimulations();
        }

        public void UpdateVelocity(Vector2 velocity)
        {
            _velocity = velocity;
        }

        private void SwitchSimulations()
        {
            simulationBody.gameObject.SetActive(_isSimulate);
            lineRenderer.enabled = _isSimulate;
            simulationBody.transform.SetParent(null);
            
            Physics2D.simulationMode = _isSimulate ? SimulationMode2D.Script : SimulationMode2D.FixedUpdate;
        }
        
        public void Update()
        {
            if(!_isSimulate)
                return;

            simulationBody.Rigidbody.position = transform.position;
            simulationBody.Rigidbody.linearVelocity = _velocity;
            simulationBody.Rigidbody.angularVelocity = 0f;
            simulationBody.IsCollision = false;

            lineRenderer.positionCount = maxIterations;
            lineRenderer.SetPosition(0, transform.position);
            
            if(_points.Length != maxIterations)
                _points = new Vector3[maxIterations];

            var count = 0;
            for (var i = 0; i < maxIterations; i++)
            {
                Physics2D.Simulate(simulationStep);
                _points[i] = simulationBody.transform.position;
                count++;
                
                if(simulationBody.IsCollision)
                    break;
            }

            lineRenderer.positionCount = count;
            lineRenderer.SetPositions(_points);
        }
    }
}