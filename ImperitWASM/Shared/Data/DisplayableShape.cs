using ImperitWASM.Shared.State;
using System;

namespace ImperitWASM.Shared.Data
{
	public class DisplayableShape
	{
		public Point[] Border { get; set; } = Array.Empty<Point>();
		public Point Center { get; set; }
		public Color Fill { get; set; }
		public Color Stroke { get; set; }
		public int StrokeWidth { get; set; }
		public bool IsStart { get; set; }
		public DisplayableShape() { }
		public DisplayableShape(Point[] border, Point center, Color fill, Color stroke, int strokeWidth, bool isStart)
		{
			Border = border;
			Center = center;
			Fill = fill;
			Stroke = stroke;
			StrokeWidth = strokeWidth;
			IsStart = isStart;
		}
	}
}
