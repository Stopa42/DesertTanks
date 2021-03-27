using Mirror;
using UnityEngine;

namespace RTSTutorial
{
    public class ResourceGenerator : NetworkBehaviour
    {
        [SerializeField] private int _resourcesPerInterval = 10;
        [SerializeField] private float _interval = 2f;

        private float _timer;
        private ResourceHandler _resourceHandler;

        public override void OnStartServer()
        {
            _timer = _interval;
            _resourceHandler = connectionToClient.identity.GetComponent<ResourceHandler>();

            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        private void ServerHandleGameOver()
        {
            enabled = false;
        }

        [ServerCallback]
        private void Update()
        {
            TryGatherResources();
        }

        private void TryGatherResources()
        {
            _timer -= Time.deltaTime;

            if (!(_timer <= 0)) return;
            _resourceHandler.AddResources(_resourcesPerInterval);

            _timer += _interval;
        }
    }
}