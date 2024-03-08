namespace Lis.Core
{
    public interface IGrabbable
    {
        public abstract bool CanBeGrabbed();
        public abstract void Grabbed();
        public abstract void Released();
    }
}
