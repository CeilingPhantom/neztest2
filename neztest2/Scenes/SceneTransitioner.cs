using Nez;
using NezTest2.Units;

namespace NezTest2.Scenes
{
    public class SceneTransitioner : Component, IUpdatable
    {
        readonly string transitionTo;
        Player player;
        BoxCollider playerCollider, collider;

        public SceneTransitioner(string sceneToTransitionTo)
        {
            transitionTo = sceneToTransitionTo;
        }

        public override void OnAddedToEntity()
        {
            collider = Entity.GetComponent<BoxCollider>();
            Entity playerEntity = Entity.Scene.FindEntity("player");
            player = playerEntity.GetComponent<Player>();
            playerCollider = playerEntity.GetComponent<BoxCollider>();
        }

        public void Update()
        {
            if (collider.CollidesWith(playerCollider, out CollisionResult res))
            {
                NezTest2.UpdatePlayer();
                Core.StartSceneTransition(new FadeTransition(NezTest2.Scenes[transitionTo]));
                Entity.Destroy();
            }
        }

        public void OnTriggerExit(Collider other, Collider local) { }
    }
}
