#if GODOT
using Godot;

namespace Svelto.Context
{
    public abstract partial class GodotContext : Node
    {

    }

    //a Godot context is a platform specific context wrapper. Within a godot scene, creates a Node and attach this script
    //to it (usually the scene's root node).
    //The Node will become one composition root holder. OnContextCreated is called during the _EnterTree of this Node
    //OnContextInitialized is called one frame after the Node enters the scene tree.
    //OnContextDestroyed is called when the Node is removed from the sceneTree, or QueueFreed
    public partial class GodotContext<T> : GodotContext where T : class, ICompositionRoot, new()
    {
        public override void _EnterTree()
        {
            _applicationRoot = new T();

            _applicationRoot.OnContextCreated(this);
        }


        public override void _ExitTree()
        {
            _applicationRoot?.OnContextDestroyed(_hasBeenInitialised);
        }

        public override void _Ready()
        {
            WaitForFrameInit();
        }


        private async void WaitForFrameInit()
        {
            //let's wait until the end of the frame, so we are sure that all the awake and starts are called
            await ToSignal(GetTree(), "process_frame");  // <- Equivalent to Unity's WaitForEndOfFrame();

            _hasBeenInitialised = true;

            _applicationRoot.OnContextInitialized(this);
        }

        T _applicationRoot;
        bool _hasBeenInitialised;
    }
}
#endif