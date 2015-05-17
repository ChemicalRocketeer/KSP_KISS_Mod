using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KSP_KISS_Mod;

namespace KISS_Test {
	[TestClass]
	public class UnitTest1 {
		[TestMethod]
		public void TestStackMachine() {
			int pushInt = 42000;
			string pushString = "hello world";
			float pushFloat = 12.756f;
			bool pushBool = false;
			byte pushByte = 128;
			StackMachine stack = new StackMachine();
			
			stack.PushString(pushString);
			Assert.AreEqual(pushString, stack.PeekString(), "String not pushed correctly");
			stack.PushInt(pushInt);
			Assert.AreEqual(pushInt, stack.PeekInt(), "Int not pushed correctly");
			stack.PushFloat(pushFloat);
			Assert.AreEqual(pushFloat, stack.PeekFloat(), "Float not pushed correctly");
			stack.PushBool(pushBool);
			Assert.AreEqual(pushBool, stack.PeekBool(), "Bool not pushed correctly");
			stack.Push(pushByte);
			Assert.AreEqual(pushByte, stack.Peek(), "Byte not pushed correctly");

			
			Assert.AreEqual(pushByte, stack.Pop(), "Byte not popped correctly");
			Assert.AreEqual(pushBool, stack.PopBool(), "Bool not popped correctly");
			Assert.AreEqual(pushFloat, stack.PopFloat(), "Float not popped correctly");
			Assert.AreEqual(pushInt, stack.PopInt(), "Int not popped correctly");
			Assert.AreEqual(pushString, stack.PopString(), "String not popped correctly");

			Assert.AreEqual(0, stack.Count, "Stack not empty");
		}

		[TestMethod]
		public void TestCombineArrays() {
			byte[] a = new byte[0];
			byte[] b = new byte[] { 1, 2, 3 };
			byte[] c = new byte[] { 4, 5, 6, 7, 8, 9 };
			byte[] d = new byte[0];
			a = CombineArrays(a, b, c, d);
			byte[] a1 = new byte[] {1, 2, 3, 4, 5, 6, 7, 8, 9 };
			Assert.AreEqual(a1.Length, a.Length, "Arrays not combined correctly (wrong length)");
			for (int i = 0; i < a.Length; i++) {
				Assert.AreEqual(a1[i], a[i], "Arrays have incorrect values");
			}
			a = new byte[] { 1, 2, 3 };
			b = new byte[] { 4, 5, 6 };
			a = CombineArrays(a, b);
			a1 = new byte[] {1, 2, 3, 4, 5, 6 };
			Assert.AreEqual(a1.Length, a.Length, "Arrays not combined correctly (wrong length)");
			for (int i = 0; i < a.Length; i++) {
				Assert.AreEqual(a1[i], a[i], "Arrays have incorrect values");
			}
		}

		[TestMethod]
		public void TestBytecode() {
			byte[] testint = BitConverter.GetBytes(42000);
			byte[] testbool = BitConverter.GetBytes(true);
			byte[] testfloat = BitConverter.GetBytes(2.56f);
			byte[] testString = Encoding.UTF8.GetBytes("Hello World!");
			byte[] testStringLength = BitConverter.GetBytes(testString.Length);
			byte[] testByte = new byte[] { 42 };
			byte[] bytecode = CombineArrays(testint, testbool, testfloat, testStringLength, testString, testByte);
			Bytecode bc = new Bytecode(bytecode);
			
			Assert.AreEqual(42000, bc.readInt(), "Bytecode not reading int");
			Assert.AreEqual(true, bc.readBool(), "Bytecode not reading bool");
			Assert.AreEqual(2.56f, bc.readFloat(), "Bytecode not reading float");
			Assert.AreEqual("Hello World!", bc.readString(), "Bytecode not reading string");
			Assert.AreEqual(42, bc.readByte(), "Bytecode not reading byte");
		}

		T[] CombineArrays<T>(T[] a, T[] b, params T[][] others) {
			T[] result = new T[a.Length + b.Length];
			// copy a to result
			Array.Copy(a, result, a.Length);
			// copy b to result
			Array.Copy(b, 0, result, a.Length, b.Length);
			// if there are more arrays, copy them
			if (others.Length > 0) {
				a = result;
				b = others[0];
				// make a new array that is the same as others but with the first element removed
				T[][] others2 = new T[others.Length - 1][];
				Array.Copy(others, 1, others2, 0, others2.Length);
				result = CombineArrays(a, b, others2);
			}
			return result;
		}
	}
}
