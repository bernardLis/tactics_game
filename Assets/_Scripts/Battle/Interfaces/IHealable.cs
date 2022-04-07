public interface IHealable<T, Y>
{
    public void GainHealth(int healthGain, T attacker, Y ability);
}
