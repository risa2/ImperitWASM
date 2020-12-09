using System;
using System.Globalization;

namespace ImperitWASM.Shared.State
{
	public record Color(byte R, byte G, byte B, byte A = 255)
	{
		public Color() : this(0, 0, 0, 0) { }
		static string ToHex(byte num) => num.ToString("x2", CultureInfo.InvariantCulture);
		static byte FromHex(string s) => byte.Parse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
		public static Color Parse(string str) => new Color(FromHex(str[1..3]), FromHex(str[3..5]), FromHex(str[5..7]), str.Length < 9 ? (byte)255 : FromHex(str[7..9]));
		public override string ToString() => "#" + ToHex(R) + ToHex(G) + ToHex(B) + ToHex(A);

		static byte Mix(byte a, byte b, int w1, int w2) => (byte)(((a * w1) + (b * w2)) / (w1 + w2));
		static byte Supl(byte x, byte y) => (byte)(255 - ((255 - x) * (255 - y) / 255));
		public Color Mix(Color color) => new Color(Mix(R, color.R, A, color.A), Mix(G, color.G, A, color.A), Mix(B, color.B, A, color.A), Supl(A, color.A));
		public Color Over(Color color) => new Color(Mix(R, color.R, 255, 255 - A), Mix(G, color.G, 255, 255 - A), Mix(B, color.B, 255, 255 - A), Supl(A, color.A));

		public Color WithAlpha(byte alpha) => this with { A = alpha };
		public static Color HSV(double H, double S, double V)
		{
			while (H < 0) { H += 360; }
			while (H >= 360) { H -= 360; }
			double R, G, B;
			if (V <= 0)
			{
				R = G = B = 0;
			}
			else if (S <= 0)
			{
				R = G = B = V;
			}
			else
			{
				double hf = H / 60.0;
				int i = (int)Math.Floor(hf);
				double f = hf - i;
				double pv = V * (1 - S);
				double qv = V * (1 - (S * f));
				double tv = V * (1 - (S * (1 - f)));
				switch (i)
				{
					// Red is the dominant color
					case -1:
					case 5:
						R = V;
						G = pv;
						B = qv;
						break;
					case 6:
					case 0:
						R = V;
						G = tv;
						B = pv;
						break;
					// Green is the dominant color
					case 1:
						R = qv;
						G = V;
						B = pv;
						break;
					case 2:
						R = pv;
						G = V;
						B = tv;
						break;
					// Blue is the dominant color
					case 3:
						R = pv;
						G = qv;
						B = V;
						break;
					case 4:
						R = tv;
						G = pv;
						B = V;
						break;
					// The color is not defined, we should throw an error
					default:
						R = G = B = V; // Just pretend its black/white
						break;
				}
			}
			static byte clamp(double i) => (byte)Math.Clamp((int)i, 0, 255);
			return new Color(clamp(R * 255), clamp(G * 255), clamp(B * 255));
		}
		public static Color Generate(int i, double h_0, double s, double v) => HSV(h_0 + (137.507764050037854 * i), s, v);
	}
}