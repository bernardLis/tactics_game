using System.Threading.Tasks;

public interface ICreatable<Vector3, T, Y>
{
    public Task Initialize(Vector3 pos, T ability, string tag);

    public string GetCreatedObjectDescription();
}
