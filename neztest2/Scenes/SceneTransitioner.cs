using Nez;

namespace NezTest2.Scenes
{
    public class SceneTransitioner : Component, IUpdatable
    {
        static bool transitioning = false;
        readonly string transitionTo;
        Entity player;
        BoxCollider playerCollider, collider;

        public SceneTransitioner(string sceneToTransitionTo)
        {
            transitionTo = sceneToTransitionTo;
        }

        public override void OnAddedToEntity()
        {
            player = Entity.Scene.FindEntity("player");
            collider = Entity.GetComponent<BoxCollider>();
            playerCollider = player.GetComponent<BoxCollider>();
        }

        public void Update()
        {
            if (!transitioning && collider.CollidesWith(playerCollider, out CollisionResult res))
            {
                NezTest2.UpdatePlayer();
                Core.StartSceneTransition(NezTest2.Scenes[transitionTo]);
                transitioning = true;
            }
            else
                transitioning = false;
        }

        public void OnTriggerExit(Collider other, Collider local) { }
    }
}
