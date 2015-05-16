using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KSP_KISS_Mod {
	public class ScriptingInterface : MonoBehaviour {

		public enum Type { VOID, BOOL, BYTE, INT, FLOAT, STRING, ADDRESS }
		

		public class ScriptNode {
			Type[] requirements;
			List<ScriptNode> subnodes;
			byte[] bytecode;
			string name;

			public void writeBytecode(Bytecode existingCode, byte index) {

			}
		}

		public class Button {
			Type[] typesProvided;
			string text;
			ScriptNode node;
		}

		public List<Button> buttonControls = new List<Button>();

	}
}
