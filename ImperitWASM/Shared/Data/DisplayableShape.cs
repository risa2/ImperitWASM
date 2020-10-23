using ImperitWASM.Shared.State;
using System;
using System.Linq;

namespace ImperitWASM.Shared.Data
{
	public class DisplayableShape
	{
		public Point[] B { get; set; } = Array.Empty<Point>();
		public Point C { get; set; }
		public Color F { get; set; }
		public Color S { get; set; }
		public int W { get; set; }
		public bool R { get; set; }
		public string[] T { get; set; } = Array.Empty<string>();
		public DisplayableShape() { }
		public DisplayableShape(Point[] border, Point center, Color fill, Color stroke, int strokeWidth, bool isStart, string[] text)
		{
			B = border;
			C = center;
			F = fill;
			S = stroke;
			W = strokeWidth;
			R = isStart;
			T = text;
		}
		public DisplayableShape UpdateText()
		{
			return new DisplayableShape(B, C, F, S, W, R, R ? T : T.Skip(1).ToArray());
		}
	}
}
