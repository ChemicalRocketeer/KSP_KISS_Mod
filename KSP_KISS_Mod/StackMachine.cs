using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_KISS_Mod {
	public class StackMachine : Stack<byte> {

		public StackMachine() : base() {
		}

		public StackMachine(byte[] stackdata) : base(stackdata.Length * 2) {
			Push(stackdata);
		}

		public void OnSave(ConfigNode node) {
			foreach (byte b in this) {
				node.AddValue("stack", b.ToString());
			}
		}

		public void OnLoad(ConfigNode node) {
			Clear();
			string[] stackarray = node.GetValues("stack");
			for (int i = stackarray.Length - 1; i >= 0; i--) {
				Push(Byte.Parse(stackarray[i]));
			}
		}

		#region push

		// push a sequence of bytes onto the stack. The last value will be on top.
		public override void Push(params byte[] values) {
			foreach (byte value in values) base.Push(value);
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

		public void PushAddress(Address address) {
			PushInt(address.Index);
		}

		#endregion

		#region peek

		public bool PeekBool() {
			return BitConverter.ToBoolean(new byte[] {Peek()}, 0);
		}

		public int PeekInt() {
			byte[] arr = new byte[sizeof(int)];
			Stack<byte>.Enumerator en = GetEnumerator();
			en.MoveNext();
			for (int i = arr.Length - 1; i >= 0; i--) {
				arr[i] = en.Current;
				en.MoveNext();
			}
			en.Dispose();
			return BitConverter.ToInt32(arr, 0);
		}

		public float PeekFloat() {
			byte[] arr = new byte[sizeof(float)];
			Stack<byte>.Enumerator en = GetEnumerator();
			en.MoveNext();
			for (int i = arr.Length - 1; i >= 0; i--) {
				arr[i] = en.Current;
				en.MoveNext();
			}
			en.Dispose();
			return BitConverter.ToSingle(arr, 0);
		}

		public string PeekString() {
			// get the string size
			int size = PeekInt();
			byte[] arr = new byte[size];
			Stack<byte>.Enumerator en = GetEnumerator();
			en.MoveNext(); // a stack enumerator starts before the first element for some reason, and you have to call this to get it to work.
			// move the enumerator past the int representing string size
			for (int i = 0; i < sizeof(int); i++) {
				en.MoveNext();
			}
			// read the actual string bytes
			for (int i = arr.Length - 1; i >= 0; i--) {
				arr[i] = en.Current;
				en.MoveNext();
			}
			en.Dispose();
			return Encoding.UTF8.GetString(arr);
		}

		public Address PeekAddress() {
			return new Address(PeekInt());
		}

		#endregion

		#region pop

		public bool PopBool() {
			return BitConverter.ToBoolean(new byte[] {Pop()}, 0);
		}

		public int PopInt() {
			byte[] arr = new byte[sizeof(int)];
			for (int i = arr.Length - 1; i >= 0; i--) {
				arr[i] = Pop();
			}
			return BitConverter.ToInt32(arr, 0);
		}

		public float PopFloat() {
			byte[] arr = new byte[sizeof(float)];
			for (int i = arr.Length - 1; i >= 0; i--) {
				arr[i] = Pop();
			}
			return BitConverter.ToSingle(arr, 0);
		}

		public string PopString() {
			int size = PopInt();
			byte[] arr = new byte[size];
			for (int i = arr.Length - 1; i >= 0; i--) {
				arr[i] = Pop();
			}
			return Encoding.UTF8.GetString(arr);
		}

		public Address PopAddress() {
			return new Address(PopInt());
		}

		#endregion
	}
}
