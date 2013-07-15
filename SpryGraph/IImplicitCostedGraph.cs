namespace Alastri.SpryGraph
{
    public interface IImplicitCostedGraph<TVertex, TEdge> : IImplicitGraph<TVertex,TEdge> 
        where TEdge : ICostedEdge<TVertex>         
    {        
        
    }
}