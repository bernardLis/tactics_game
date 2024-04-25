namespace Lis.Core
{
    public interface IGrabbable
    {
        public bool CanBeGrabbed();
        public void Grabbed();
        public void Released();
    }
}