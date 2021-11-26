using System.Threading.Tasks;

public interface IAttackable<T>
{
    public Task TakeDamage(int damage, T attacker);
}
