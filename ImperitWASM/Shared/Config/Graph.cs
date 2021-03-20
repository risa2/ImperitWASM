using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ImperitWASM.Shared.Config
{
	public sealed record Graph(ImmutableArray<ImmutableArray<int>> Neighbors)
	{
		public int NeighborCount(int vertex) => Neighbors[vertex].Length;
		public ImmutableArray<int> this[int vertex] => Neighbors[vertex];
		public int Count => Neighbors.Length;

		public bool Passable(int from, int to, int limit, Func<int, int, int> difficulty)
		{
			var stack = new List<(int Pos, int Distance)>() { (from, 0) };
			bool[] visited = new bool[Count];
			visited[from] = true;
			for (int i = 0; i < stack.Count; ++i)
			{
				if (stack[i].Pos == to)
				{
					return true;
				}
				var neighbors = Neighbors[stack[i].Pos];
				for (int n = 0; n < neighbors.Length; ++n)
				{
					int vertex = neighbors[n];
					if (!visited[vertex] && stack[i].Distance + difficulty(stack[i].Pos, vertex) <= limit)
					{
						stack.Add((vertex, stack[i].Distance + difficulty(stack[i].Pos, vertex)));
						visited[vertex] = true;
					}
				}
			}
			return false;
		}

		public IEnumerable<List<int>> Split(Func<int, bool> relevant, Func<int, int, bool> passable)
		{
			bool[] visited = new bool[Count];
			for (int from = 0; from < visited.Length; ++from)
			{
				if (!visited[from] && relevant(from))
				{
					visited[from] = true;
					var points = new List<int> { from };
					for (int i = 0; i < points.Count; ++i)
					{
						var neighbors = Neighbors[points[i]];
						for (int n = 0; n < neighbors.Length; ++n)
						{
							int vertex = neighbors[n];
							if (!visited[vertex] && passable(points[i], vertex))
							{
								visited[vertex] = true;
								points.Add(vertex);
							}
						}
					}
					yield return points;
				}
			}
		}
	}
}
