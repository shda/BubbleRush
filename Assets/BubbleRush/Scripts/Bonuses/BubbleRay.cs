using System;
using UnityEngine;

namespace BubbleRush.Scripts.Bonuses
{
    public class BubbleRay : MonoBehaviour
    {
        [SerializeField] private BubbleSimulationBody bubbleSimulationBody;
        [SerializeField] private Transform rootRotate;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private int maxIterations = 49;
        [SerializeField] private float simulationStep = 0.04f;

        private Vector3[] _completePointsRay = Array.Empty<Vector3>();
        private Vector2 _velocity;
        private bool _isPlaySimulate;

        private void Awake()
        {
            StopDrawLine();
        }

        public void StartDrawLine()
        {
            _isPlaySimulate = true;
            SwitchSimulations();
        }

        public void StopDrawLine()
        {
            _isPlaySimulate = false;
            SwitchSimulations();
        }

        public void SetVelocity(Vector2 velocity)
        {
            _velocity = velocity;
        }

        private void SwitchSimulations()
        {
            bubbleSimulationBody.gameObject.SetActive(_isPlaySimulate);
            lineRenderer.enabled = _isPlaySimulate;
            bubbleSimulationBody.transform.SetParent(null);
            
            Physics2D.simulationMode = _isPlaySimulate ? SimulationMode2D.Script : SimulationMode2D.FixedUpdate;
        }
        
        public void Update()
        {
            if(!_isPlaySimulate)
                return;

            bubbleSimulationBody.Rigidbody.position = transform.position;
            bubbleSimulationBody.Rigidbody.linearVelocity = _velocity;
            bubbleSimulationBody.Rigidbody.angularVelocity = 0f;
            bubbleSimulationBody.IsCollision = false;

            lineRenderer.positionCount = maxIterations;
            lineRenderer.SetPosition(0, transform.position);
            
            if(_completePointsRay.Length != maxIterations)
                _completePointsRay = new Vector3[maxIterations];

            var count = 0;
            for (var i = 0; i < maxIterations; i++)
            {
                Physics2D.Simulate(simulationStep);
                _completePointsRay[i] = bubbleSimulationBody.transform.position;
                count++;
                
                if(bubbleSimulationBody.IsCollision)
                    break;
            }

            lineRenderer.positionCount = count;
            lineRenderer.SetPositions(_completePointsRay);
        }
    }
}