using System.Threading.Tasks;

public interface IAttackable<T, Y>
{
    public Task TakeDamage(int damage, T attacker, Y ability);
}
