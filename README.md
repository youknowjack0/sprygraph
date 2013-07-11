SpryGraph
=========

.NET graph lib for fast path finding (in C#)

Currently has: 
- Fast A*
- Fast Dijkstra

State: Alpha

If you're looking for a production library, have a look at <a href="http://quickgraph.codeplex.com/">QuickGraph</a>.

==========

Some quick performance stats on random graphs:

<pre>
             |        |           |       Query Time       |
Vertex Count | Degree | Algorithm | SpryGraph | QuickGraph |
------------------------------------------------------------
200,000        2        Dijkstra    145.9ms     903.3ms
1,000,000      4        A*          513.5ms     9625.7ms
</pre>
