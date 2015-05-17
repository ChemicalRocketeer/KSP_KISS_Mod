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
			Type[] typesProvided;
			List<ScriptNode> subnodes;
			byte[] bytecode;
			string name;

			public void writeBytecode(Bytecode existingCode, byte index) {

			}
		}

		public class Button {
			string text;
			ScriptNode node;
		}

		public List<Button> buttonControls = new List<Button>();

		
		private Rect windowPosition = new Rect();
		private GUIStyle windowStyle, labelStyle, buttonStyle;
		private int windowID;

		// called during the loading screen, used for initializing constants and the like
		public void Awake() {
			windowStyle = new GUIStyle(HighLogic.Skin.window);
			windowStyle.fixedWidth = 350f;
			labelStyle = new GUIStyle(HighLogic.Skin.label);
			labelStyle.stretchWidth = true;
			buttonStyle = new GUIStyle(HighLogic.Skin.button);
			buttonStyle.stretchWidth = true;
		}

		// called after the part is loaded into Unity, before anything happens to it
		public void Start() {
			//Reset();
			RenderingManager.AddToPostDrawQueue(0, OnDraw);
		}

		private void OnDraw() {
			windowID = GUIUtility.GetControlID(FocusType.Keyboard);
			windowPosition = GUILayout.Window(windowID, windowPosition, OnWindow, "KISS Automation Sequence Synthesizer", windowStyle);
		}

		private void OnWindow(int windowID) {
			GUI.skin = HighLogic.Skin;

			GUI.DragWindow();
		}
	}
}
