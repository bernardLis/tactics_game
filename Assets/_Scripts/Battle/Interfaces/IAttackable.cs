using System.Threading.Tasks;

public interface IAttackable<T, Y>
{
    public Task<bool> TakeDamage(int damage, T attacker, Y ability);
}
