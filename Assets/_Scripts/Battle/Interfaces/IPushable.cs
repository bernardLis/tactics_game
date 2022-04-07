public interface IPushable<Vector3, T, Y>
{
    public void GetPushed(Vector3 dir, T attacker, Y ability);
}
