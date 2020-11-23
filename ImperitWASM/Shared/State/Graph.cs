using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.State
{
	[JsonConverter(typeof(Cvt.GraphConverter))]
	public class Graph : IReadOnlyList<IEnumerable<int>>
	{
		readonly ImmutableArray<int> edges, starts;
		public Graph(ImmutableArray<int> edges, ImmutableArray<int> starts)
		{
			this.edges = edges;
			this.starts = starts;
		}
		public bool Passable(int from, int to, int limit, Func<int, int, int> difficulty)
		{
			var stack = new List<(int Pos, int Distance)>() { (from, 0) };
			bool[]? visited = new bool[Count];
			visited[from] = true;
			for (int i = 0; i < stack.Count; ++i)
			{
				if (stack[i].Pos == to)
				{
					return true;
				}
				foreach (int vertex in this[stack[i].Pos].Where(n => !visited[n]))
				{
					if (stack[i].Distance + difficulty(stack[i].Pos, vertex) <= limit)
					{
						stack.Add((vertex, stack[i].Distance + difficulty(stack[i].Pos, vertex)));
						visited[vertex] = true;
					}
				}
			}
			return false;
		}
		public int NeighborCount(int vertex) => starts[vertex + 1] - starts[vertex];
		public IEnumerable<int> this[int vertex] => edges.Take(starts[vertex + 1]).Skip(starts[vertex]);
		public int Count => starts.Length - 1;
		public IEnumerator<IEnumerable<int>> GetEnumerator() => Enumerable.Range(0, Count).Select(i => this[i]).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}