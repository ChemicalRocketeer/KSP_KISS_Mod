using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_KISS_Mod {
	class StackMachine {

		Stack<byte> stack = new Stack<byte>();

		public StackMachine(byte[] stackdata) {
			stack = new Stack<byte>(stackdata.Length * 2);
			foreach (byte b in stackdata) {
				stack.Push(b);
			}
		}

		public void Push(params byte[] values) {
			foreach (byte value in values) stack.Push(value);
		}

		public void PushBool(bool value) {
			Push(BitConverter.GetBytes(value));
		}

		public void PushInt(int value) {
			Push(BitConverter.GetBytes(value));
		}

		public void PushFloat(float value) {
			Push(BitConverter.GetBytes(value));
		}

		public void PushString(string value) {
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			Push(bytes);
			PushInt(bytes.Length);
		}

		public byte Peek() {
			return stack.Peek();
		}

		public bool PeekBool() {
			return BitConverter.ToBoolean(new byte[] {Peek()}, 0);
		}

		public int PeekInt() {
			byte[] arr = new byte[sizeof(int)];
			Stack<byte>.Enumerator en = stack.GetEnumerator();
			for (int i = arr.Length - 1; i >= 0; i--) {
				arr[i] = en.Current;
				en.MoveNext();
			}
			en.Dispose();
			return BitConverter.ToInt32(arr, 0);
		}

		public float PeekFloat() {
			byte[] arr = new byte[sizeof(float)];
			Stack<byte>.Enumerator en = stack.GetEnumerator();
			for (int i = arr.Length - 1; i >= 0; i--) {
				arr[i] = en.Current;
				en.MoveNext();
			}
			en.Dispose();
			return BitConverter.ToSingle(arr, 0);
		}

		public string PeekString() {
			int size = PeekInt();
			byte[] arr = new byte[size];
			Stack<byte>.Enumerator en = stack.GetEnumerator();
			for (int i = arr.Length - 1; i >= 0; i--) {
				arr[i] = en.Current;
				en.MoveNext();
			}
			en.Dispose();
			return Encoding.UTF8.GetString(arr);
		}

		public byte Pop() {
			return stack.Pop();
		}

		public bool PopBool() {
			return BitConverter.ToBoolean(new byte[] {Pop()}, 0);
		}

		public int PopInt() {
			byte[] arr = new byte[sizeof(int)];
			for (int i = arr.Length - 1; i >= 0; i--) {
				arr[i] = stack.Pop();
			}
			return BitConverter.ToInt32(arr, 0);
		}

		public float PopFloat() {
			byte[] arr = new byte[sizeof(float)];
			for (int i = arr.Length - 1; i >= 0; i--) {
				arr[i] = stack.Pop();
			}
			return BitConverter.ToSingle(arr, 0);
		}

		public string PopString() {
			int size = PeekInt();
			byte[] arr = new byte[size];
			for (int i = arr.Length - 1; i >= 0; i--) {
				arr[i] = stack.Pop();
			}
			return Encoding.UTF8.GetString(arr);
		}
	}
}
