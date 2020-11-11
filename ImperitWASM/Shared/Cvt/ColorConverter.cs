﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ImperitWASM.Shared.State;

namespace ImperitWASM.Shared.Cvt
{
	public class ColorConverter : JsonConverter<Color>
	{
		public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Color.Parse(reader.GetString().Must());
		public override void Write(Utf8JsonWriter writer, Color col, JsonSerializerOptions options) => writer.WriteStringValue(col.ToString());
	}
}