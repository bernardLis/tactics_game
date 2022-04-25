using System.Threading.Tasks;

public interface ICreatable<Vector3, T>
{
    public Task Initialize(Vector3 pos, T ability);
}
