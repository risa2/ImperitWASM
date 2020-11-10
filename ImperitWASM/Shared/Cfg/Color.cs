using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace ImperitWASM.Shared.Cfg
{
	[JsonConverter(typeof(Cvt.ColorConverter))]
	public class Color : IEquatable<Color>
	{
		static string ToHex(byte num) => num.ToString("x2", CultureInfo.InvariantCulture);
		static byte FromHex(string s) => byte.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
		public byte R { get; }
		public byte G { get; }
		public byte B { get; }
		public byte A { get; }
		public Color() : this(0, 0, 0, 0) { }
		public Color(byte R, byte G, byte B, byte A = 255) => (this.R, this.G, this.B, this.A) = (R, G, B, A);
		public override string ToString() => "#" + ToHex(R) + ToHex(G) + ToHex(B) + ToHex(A);
		public static Color Parse(string str) => new Color(FromHex(str[1..3]), FromHex(str[3..5]), FromHex(str[5..7]), str.Length < 9 ? (byte)255 : FromHex(str[7..9]));
		static byte Mix(byte A, byte B, int w1, int w2) => (byte)(((A * w1) + (B * w2)) / (w1 + w2));
		static byte Supl(byte x, byte y) => (byte)(255 - ((255 - x) * (255 - y) / 255));
		public Color Mix(Color color) => new Color(Mix(R, color.R, A, color.A), Mix(G, color.G, A, color.A), Mix(B, color.B, A, color.A), Supl(A, color.A));
		public Color Over(Color color) => new Color(Mix(R, color.R, 255, 255 - A), Mix(G, color.G, 255, 255 - A), Mix(B, color.B, 255, 255 - A), Supl(A, color.A));
		public bool Equals(Color? c2) => (R, G, B, A) == (c2?.R, c2?.G, c2?.B, c2?.A);
		public override bool Equals(object? obj) => Equals(obj as Color);
		public override int GetHashCode() => (R, G, B, A).GetHashCode();
		public static bool operator ==(Color? a, Color? b) => ReferenceEquals(a, b) || (a is Color && a.Equals(b));
		public static bool operator !=(Color? a, Color? b) => !(a == b);
		public Color WithAlpha(byte a) => new Color(R, G, B, a);
		public Color(double h, double s, double v)
		{
			while (h < 0) { h += 360; }
			while (h >= 360) { h -= 360; }
			double r, g, b;
			if (v <= 0)
			{
				r = g = b = 0;
			}
			else if (s <= 0)
			{
				r = g = b = v;
			}
			else
			{
				double hf = h / 60.0;
				int i = (int)Math.Floor(hf);
				double f = hf - i;
				double pv = v * (1 - s);
				double qv = v * (1 - (s * f));
				double tv = v * (1 - (s * (1 - f)));
				switch (i)
				{
					// Red is the dominant color
					case -1:
					case 5:
						r = v;
						g = pv;
						b = qv;
						break;
					case 6:
					case 0:
						r = v;
						g = tv;
						b = pv;
						break;
					// Green is the dominant color
					case 1:
						r = qv;
						g = v;
						b = pv;
						break;
					case 2:
						r = pv;
						g = v;
						b = tv;
						break;
					// Blue is the dominant color
					case 3:
						r = pv;
						g = qv;
						b = v;
						break;
					case 4:
						r = tv;
						g = pv;
						b = v;
						break;
					// The color is not defined, we should throw an error
					default:
						r = g = b = v; // Just pretend its black/white
						break;
				}
			}
			static byte clamp(int i) => i < 0 ? (byte)0 : i > 255 ? (byte)255 : (byte)i;
			R = clamp((int)(r * 255.0));
			G = clamp((int)(g * 255.0));
			B = clamp((int)(b * 255.0));
			A = 255;
		}
	}
}