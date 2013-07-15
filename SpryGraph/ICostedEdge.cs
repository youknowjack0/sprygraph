namespace Alastri.SpryGraph
{
    public interface ICostedEdge<TVertex> : IEdge<TVertex>
    {
        double GetCost();
    }
}