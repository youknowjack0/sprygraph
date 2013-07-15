namespace Alastri.SpryGraph
{
    public interface IHeuristicVertex<in TVertex>
    {
        double Heuristic(TVertex destination);
    }
}