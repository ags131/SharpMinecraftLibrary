﻿using System;
using System.IO;
using System.Text;
using LibNbt.Exceptions;
using LibNbt.Queries;

namespace LibNbt.Tags
{
	public class NbtLong : NbtTag, INbtTagValue<long>
	{
		public long Value { get; set; }

		public NbtLong() : this("") { }
		public NbtLong(string tagName) : this(tagName, 0) { }
		[Obsolete("This constructor will be removed in favor of using NbtLong(string tagName, long value)")]
		public NbtLong(long value) : this("", value) { }
		public NbtLong(string tagName, long value)
		{
			Name = tagName;
			Value = value;
		}

		internal override void ReadTag(Stream readStream) { ReadTag(readStream, true); }
		internal override void ReadTag(Stream readStream, bool readName)
		{
			if (readName)
			{
				var name = new NbtString();
				name.ReadTag(readStream, false);

				Name = name.Value;
			}


			var buffer = new byte[8];
			int totalRead = 0;
			while ((totalRead += readStream.Read(buffer, totalRead, 8)) < 8)
			{ }
			if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
			Value = BitConverter.ToInt64(buffer, 0);
		}

		internal override void WriteTag(Stream writeStream) { WriteTag(writeStream, true); }
		internal override void WriteTag(Stream writeStream, bool writeName)
		{
			writeStream.WriteByte((byte)NbtTagType.TAG_Long);
			if (writeName)
			{
				var name = new NbtString("", Name);
				name.WriteData(writeStream);
			}

			WriteData(writeStream);
		}

		internal override void WriteData(Stream writeStream)
		{
			byte[] data = BitConverter.GetBytes(Value);
			if (BitConverter.IsLittleEndian) Array.Reverse(data);
			writeStream.Write(data, 0, data.Length);
		}

		internal override NbtTagType GetTagType()
		{
			return NbtTagType.TAG_Long;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append("TAG_Long");
			if (Name.Length > 0)
			{
				sb.AppendFormat("(\"{0}\")", Name);
			}
			sb.AppendFormat(": {0}", Value);
			return sb.ToString();
		}
	}
}
