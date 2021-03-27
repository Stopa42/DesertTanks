using Mirror;
using UnityEngine;

namespace RTSTutorial
{
    public class Targeter : NetworkBehaviour
    {
        public ITargetable Target { get; private set; }

        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        private void ServerHandleGameOver()
        {
            ServerClearTarget();
        }

        [Command]
        public void CmdTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent(out ITargetable target)) return;
            Target = target;
        }

        [Command]
        public void CmdClearTarget() => ServerClearTarget();

        [Server]
        public void ServerClearTarget() => Target = null;
    }
}