using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KSP_KISS_Mod
{
    public class ModuleComputerCore : PartModule {

		#region VM Definitions

		public static CodeSet ops = new CodeSet();

		public static int DUP  = ops.Add("DUP", core => { core.Push(core.Peek()); });
		public static int DROP = ops.Add("DROP", core => { core.Pop(); });
		public static int SWAP = ops.Add("SWAP", core => {
			int a = core.Pop();
			int b = core.Pop();
			core.Push(a);
			core.Push(b);
		});

		public static int POST_INT = ops.Add("POST_INT", core => { core.Push(core.BCNext()); });

		
		//public static Operation DEF_BYTE  = core => { core.MemDef(core.Pop(), core.Pop()); };
		//public static Operation GET  = core => { core.Push(core.MemGet(core.Pop())); };
		//public static Operation DEL  = core => { core.MemDel((string)core.Pop()); };

		public static int IF   = ops.Add("IF", core => {
			bool condition = core.Pop() == 0 ? false : true;
			int f = (int)core.Pop();
			int t = (int)core.Pop();
			if (condition) core.Goto(t);
			else core.Goto(f);
		});
		public static int GOTO = ops.Add("GOTO", core => { core.Goto(core.Pop()); });
		public static int END  = ops.Add("END", core => { core.Reset(); });

		public static int EQ   = ops.Add("EQ", core => { core.Push(core.Pop() == core.Pop()); });
		public static int GT   = ops.Add("GT", core => { core.Push(core.Pop() > core.Pop()); });
		public static int LT   = ops.Add("LT", core => { core.Push(core.Pop() < core.Pop()); });
		public static int GE   = ops.Add("GE", core => { core.Push(core.Pop() >= core.Pop()); });
		public static int LE   = ops.Add("LE", core => { core.Push(core.Pop() <= core.Pop()); });

		public static int ADD  = ops.Add("ADD", core => { core.Push(core.Pop() + core.Pop()); });
		public static int SUB  = ops.Add("SUB", core => { core.Push(core.Pop() - core.Pop()); });
		public static int MUL  = ops.Add("MUL", core => { core.Push(core.Pop() * core.Pop()); });
		public static int DIV  = ops.Add("DIV", core => { core.Push(core.Pop() / core.Pop()); });
		public static int MOD  = ops.Add("MOD", core => { core.Push(core.Pop() % core.Pop()); });

		//public static Operation POST_ALTITUDE = core => { core.Push(core.part.vessel.altitude); };
		
		public static int LOG  = ops.Add("LOG", core => { print("KISS_MOD: " + core.Peek().ToString()); });
		public static int ECHO = ops.Add("ECHO", core => { ScreenMessages.PostScreenMessage(core.Pop().ToString()); });
		public static int MSG  = ops.Add("MSG", core => {
			int stylecode = core.Pop();
			ScreenMessageStyle style;
			switch (stylecode) {
				case 0: 
					style = ScreenMessageStyle.LOWER_CENTER;
					break;
				case 1:
					style = ScreenMessageStyle.UPPER_CENTER;
					break;
				case 2:
					style = ScreenMessageStyle.UPPER_LEFT;
					break;
				default:
					style = ScreenMessageStyle.UPPER_RIGHT;
					break;
			}
			float dur = core.Pop();
			string msg = core.Pop().ToString();
			GUIStyle guistyle = new ScreenMessages().textStyles[stylecode];
			ScreenMessages.PostScreenMessage(new ScreenMessage(msg, dur, true, style, guistyle));
		});

		/*
		 * copy/paste this to make a new operation
		
		public static int  = ops.Add(core => {  });
		
		 */

		// add 35 + 30, post params for a 2 second centered msg with the result, check result for equality with 65, echo the result, end the program
		List<int> bytecode = new List<int>(){ POST_INT, 35, POST_INT, 30, ADD, DUP, POST_INT, 1, POST_INT, 2, MSG, POST_INT, 65, EQ, ECHO, END };

		// variables used to run the machine
		Stack<int> stack = new Stack<int>(64);
		//Dictionary<string, object> memory = new Dictionary<string, object>();
		int bcIndex = 0;
		bool running = false;



		/// <summary>
		/// Push an int value to the top of the call stack
		/// </summary>
		public void Push(int data) {
			stack.Push(data);
		}

		/// <summary>
		/// Push a bool to the top of the call stack. This simply generates a 1 or a 0 and pushes the result using Push(int)
		/// </summary>
		public void Push(bool data) {
			if (data) Push(1);
			else Push(0);
		}

		/// <summary>
		/// Remove and return the top value from the call stack
		/// </summary>
		public int Pop() {
			return stack.Pop();
		}

		/// <summary>
		/// View a value in the call stack, at an optional depth, without removing it.
		/// Depth is how far down the stack to look. Default depth is 0.
		/// depth = 1 would look at the value immediately below the top.
		/// </summary>
		public int Peek() {
			return stack.Peek();
		}

		/// <summary>
		/// Set the bytecode location to read from
		/// </summary>
		public void Goto(int index) {
			bcIndex = index;
		}

		/// <summary>
		/// Read the next bytecode instruction
		/// </summary>
		public int BCNext() {
			int b = bytecode[bcIndex];
			bcIndex ++;
			return b;
		}

		public void Reset() {
			stack = new Stack<int>();
			//memory = new Dictionary<string, object>();
			bcIndex = 0;
			running = false;
		}

		/*
		public void MemDef(string name, object data) {
			memory.Add(name, data);
		}

		public object MemGet(string name) {
			return memory[name];
		}

		public void MemDel(string name) {
			memory.Remove(name);
		}
		 */

		#endregion

		#region Part Implementation

		private Rect windowPosition = new Rect();
		private GUIStyle windowStyle, labelStyle, buttonStyle;
		private int windowID;

		// called during the loading screen, used for initializing constants and the like
		public override void OnAwake() {
			windowStyle = new GUIStyle(HighLogic.Skin.window);
			windowStyle.fixedWidth = 350f;
			labelStyle = new GUIStyle(HighLogic.Skin.label);
			labelStyle.stretchWidth = true;
			buttonStyle = new GUIStyle(HighLogic.Skin.button);
			buttonStyle.stretchWidth = true;
		}

		// called after the part is loaded into Unity, before anything happens to it
		public override void OnStart(PartModule.StartState state) {
			//Reset();
			RenderingManager.AddToPostDrawQueue(0, OnDraw);
		}

		private void OnDraw() {
			windowID = GUIUtility.GetControlID(FocusType.Keyboard);
			windowPosition = GUILayout.Window(windowID, windowPosition, OnWindow, "KISS Automation Sequence Synthesizer", windowStyle);
		}

		private void OnWindow(int windowID) {
			GUI.skin = HighLogic.Skin;

			GUILayout.BeginHorizontal(GUILayout.Width(400f));

			GUILayout.BeginVertical(GUILayout.Width(100f));
			for (int i = 0; i < ops.Count; i ++) {
				if (GUILayout.Button(ops.getName(i), buttonStyle)) {
					bytecode.Add(i);
				}
			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical(GUILayout.Width(50f));
			foreach (int code in bytecode) {
				GUILayout.Label(code.ToString(), labelStyle);
			}
			GUILayout.EndVertical();

			if (GUILayout.Button("Delete instruction", buttonStyle)) {
				bytecode.RemoveAt(bytecode.Count - 1);
			}

			GUILayout.EndHorizontal();
			GUI.DragWindow();
		}

		public override void OnLoad(ConfigNode node) {
			running = Boolean.Parse(node.GetValue("running"));
			bcIndex = Int32.Parse(node.GetValue("bytecodeIndex"));
			string[] bcarray = node.GetValues("bytecode");
			bytecode = new List<int>(bcarray.Length);
			foreach (string s in bcarray) {
				bytecode.Add(Int32.Parse(s));
			}
			stack = new Stack<int>();
			string[] stackarray = node.GetValues("stack");
			foreach (string s in stackarray) {
				stack.Push(Int32.Parse(s));
			}

		}

		public override void OnSave(ConfigNode node) {
			node.AddValue("running", running.ToString());
			node.AddValue("bytecodeIndex", bcIndex.ToString());
			foreach (int code in bytecode) {
				node.AddValue("bytecode", code);
			}
			foreach (int data in stack) {
				node.AddValue("stack", data);
			}
		}

		public override void OnUpdate() {
			
			if (running) {
				ops[BCNext()](this);
			}
		}

		// called when this part is "activated" (e.g. when staging)
		public override void OnActive() {
			
		}

		// add information to the editor window when hovering over the part
		public override string GetInfo() {
			return "???";
		}
		
		[KSPEvent(guiName = "Run Script", guiActive = true, guiActiveEditor = true)]
		public void RunScript() {
			Reset();
			running = true;
			print("KISS is running!!");
		}

		#endregion

	}
}
