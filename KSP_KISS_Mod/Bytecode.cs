using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_KISS_Mod {
	public class Bytecode {

		private byte[] bytecode;
		private int index;

		public Bytecode(byte[] bytecode) {
			this.bytecode = bytecode;
			this.index = 0;
		}

		public byte readByte() {
			byte code = bytecode[index];
			index++;
			return code;
		}

		public bool readBool() {
			bool result = BitConverter.ToBoolean(bytecode, index);
			index += sizeof(int);
			return result;
		}

		public int readInt() {
			int result = BitConverter.ToInt32(bytecode, index);
			index += sizeof(int);
			return result;
		}

		public float readFloat() {
			float result = BitConverter.ToSingle(bytecode, index);
			index += sizeof(float);
			return result;
		}

		public void Goto(int address) {
			index = address;
		}

		public void Reset() {
			index = 0;
		}
	}
}
