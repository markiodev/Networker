using ProtoBuf;
using MessagePack;
using System;
using System.IO;
using ZeroFormatter;

namespace Networker.SerialiserTest 
{
	[MessagePackObject]
	[ProtoContract]
	[ZeroFormattable]
	public class TestClass 
	{
		[Key(1)]
		[ProtoMember(1)]
		[Index(1)]
		public string SomeString { get; set; }

		[Key(2)]
		[ProtoMember(2)]
		[Index(2)]
		public int SomeInt { get; set; }
	}

	class Program 
	{

		static void Main(string[] args)
		{
			TestClass testClass = new TestClass 
			{
				SomeString = "Hello",
				SomeInt = 10
			};
			
			// Just works :)
			TestMessagePack(testClass);

			// Requires type so we can make it work :)
			TestProtobuf(testClass);

			// Requires generic so we cannot make it work :(
			// Deprecated in favor of message pack anyways
			TestZeroFormatter(testClass);
		}

		static void TestMessagePack(TestClass testClass) 
		{
			byte[] bytes = MessagePackSerializer.Typeless.Serialize(testClass);

			object obj = MessagePackSerializer.Typeless.Deserialize(bytes);

			Console.WriteLine(obj.GetType());
		}

		static void TestProtobuf(TestClass testClass) 
		{
			object input = testClass;
			byte[] bytes;
			using (var ms = new MemoryStream()) 
			{
				Serializer.Serialize(ms, input);
				bytes = ms.ToArray();
			}
			
			object obj;
			using (var ms = new MemoryStream()) 
			{
				ms.Write(bytes, 0, bytes.Length);
				obj = Serializer.Deserialize(typeof(TestClass), ms);
			}

			Console.WriteLine(obj.GetType());
		}

		static void TestZeroFormatter(TestClass testClass) 
		{
			byte[] bytes = ZeroFormatterSerializer.Serialize(testClass);

			object obj = ZeroFormatterSerializer.Deserialize<TestClass>(bytes);

			Console.WriteLine(obj.GetType());
		}
	}
}
