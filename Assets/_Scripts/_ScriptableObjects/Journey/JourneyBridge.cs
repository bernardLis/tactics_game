public class JourneyBridge
{
    public JourneyNode FromNode;
    public JourneyNode ToNode;

    public void Initialize(JourneyNode from, JourneyNode to)
    {
        FromNode = from;
        ToNode = to;
    }
}
