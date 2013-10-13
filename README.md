SpryGraph
=========

.NET graph lib for fast path finding (in C#)

Currently has: 
- Fast A*
- Fast Dijkstra

Development state: I use this library in production software.

For a brief tutorial see <a href="http://jacklangman.com/2013/07/15/faster-path-finding-in-csharp/">http://jacklangman.com/2013/07/15/faster-path-finding-in-csharp/</a>

==========

Some quick performance stats on random graphs:

<pre>
             |        |           |       Query Time       |
Vertex Count | Degree | Algorithm | SpryGraph | QuickGraph |
------------------------------------------------------------
200,000        2        Dijkstra    145.9ms     903.3ms
1,000,000      4        A*          513.5ms     9625.7ms
</pre>
