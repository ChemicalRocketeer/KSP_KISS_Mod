using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_KISS_Mod {

	public class Address {
		public int Index { get; private set; }

		public Address(int index) {
			this.Index = index;
		}
	}

	public class Bytecode {

		private byte[] bytecode;
		private int index;

		public Bytecode(byte[] bytecode) {
			this.bytecode = bytecode;
			this.index = 0;
		}

		public byte ReadByte() {
			byte code = bytecode[index];
			index++;
			return code;
		}

		public bool ReadBool() {
			bool result = BitConverter.ToBoolean(bytecode, index);
			index += sizeof(bool);
			return result;
		}

		public int ReadInt() {
			int result = BitConverter.ToInt32(bytecode, index);
			index += sizeof(int);
			return result;
		}

		public float ReadFloat() {
			float result = BitConverter.ToSingle(bytecode, index);
			index += sizeof(float);
			return result;
		}

		public string ReadString() {
			int length = ReadInt();
			string result = Encoding.UTF8.GetString(bytecode, index, length);
			index += length;
			return result;
		}

		public Address ReadAddress() {
			return new Address(ReadInt());
		}

		public void Goto(Address address) {
			index = address.Index;
		}

		public void Reset() {
			index = 0;
		}

		public byte[] getRawBytecode() {
			return bytecode;
		}

		public void OnSave(ConfigNode node) {
			node.AddValue("index", index.ToString());
			foreach (int code in bytecode) {
				node.AddValue("code", code);
			}
		}

		public void OnLoad(ConfigNode node) {
			index = Int32.Parse(node.GetValue("index"));
			string[] bcarray = node.GetValues("code");
			bytecode = new byte[bcarray.Length];
			for (int i = 0; i < bcarray.Length; i++) {
				bytecode[i] = Byte.Parse(bcarray[i]);
			}
		}
	}
}
