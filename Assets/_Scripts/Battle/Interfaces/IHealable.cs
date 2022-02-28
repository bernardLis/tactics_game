public interface IHealable<T>
{
    public void GainHealth(int healthGain, T ability);
}
