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

		public static byte DUP  = ops.Add("DUP", core => { core.stack.Push(core.stack.Peek()); });
		public static byte DROP = ops.Add("DROP", core => { core.stack.Pop(); });
		public static byte SWAP = ops.Add("SWAP", core => {
			byte a = core.stack.Pop();
			byte b = core.stack.Pop();
			core.stack.Push(a);
			core.stack.Push(b);
		});

		public static byte POST_BYTE = ops.Add("POST_BYTE", core => { core.stack.Push(core.bytecode.ReadByte()); });
		public static byte POST_INT  = ops.Add("POST_INT", core => { core.stack.PushInt(core.bytecode.ReadInt()); });

		
		//public static Operation DEF_BYTE  = core => { core.MemDef(core.Pop(), core.Pop()); };
		//public static Operation GET  = core => { core.Push(core.MemGet(core.Pop())); };
		//public static Operation DEL  = core => { core.MemDel((string)core.Pop()); };

		public static byte IF   = ops.Add("IF", core => {
			bool condition = core.stack.Pop() == 0 ? false : true;
			Address f = core.stack.PopAddress();
			Address t = core.stack.PopAddress();
			if (condition) core.bytecode.Goto(t);
			else core.bytecode.Goto(f);
		});
		public static byte GOTO = ops.Add("GOTO", core => { core.bytecode.Goto(core.stack.PopAddress()); });
		public static byte END  = ops.Add("END", core => { core.Reset(); });

		public static byte EQ_BYTE = ops.Add("EQ_BYTE", core => { core.stack.PushBool(core.stack.Pop() == core.stack.Pop()); });

		public static byte ADD_BYTE = ops.Add("ADD", core => { core.stack.Push((byte)(core.stack.Pop() + core.stack.Pop())); });
		public static byte SUB_BYTE = ops.Add("SUB", core => { core.stack.Push((byte)(core.stack.Pop() - core.stack.Pop())); });
		public static byte MUL_BYTE = ops.Add("MUL", core => { core.stack.Push((byte)(core.stack.Pop() * core.stack.Pop())); });
		public static byte DIV_BYTE = ops.Add("DIV", core => { core.stack.Push((byte)(core.stack.Pop() / core.stack.Pop())); });
		public static byte MOD_BYTE = ops.Add("MOD", core => { core.stack.Push((byte)(core.stack.Pop() % core.stack.Pop())); });

		//public static Operation POST_ALTITUDE = core => { core.Push(core.part.vessel.altitude); };
		
		public static byte LOG  = ops.Add("LOG", core => { print("KISS_MOD: " + core.stack.PeekString()); });
		public static byte PRNT = ops.Add("PRNT", core => { ScreenMessages.PostScreenMessage(core.stack.PopString()); });

		/*
		 * copy/paste this to make a new operation
		
		public static int  = ops.Add(core => {  });
		
		 */

		// add 25 + 30, print result, check result for equality with 62, echo the result, end the program
		Bytecode bytecode = new Bytecode(new byte[] { POST_BYTE, 25, POST_BYTE, 30, ADD_BYTE, DUP, PRNT, POST_BYTE, 62, EQ_BYTE, PRNT, END });

		// variables used to run the machine
		StackMachine stack = new StackMachine();
		//Dictionary<string, object> memory = new Dictionary<string, object>();
		bool running = false;


		public void Reset() {
			stack = new StackMachine();
			//memory = new Dictionary<string, object>();
			bytecode.Reset();
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
		
		private GUIStyle buttonStyle;

		// called during the loading screen, used for initializing constants and the like
		public override void OnAwake() {
			buttonStyle = new GUIStyle(HighLogic.Skin.button);
			buttonStyle.stretchWidth = true;
		}

		// called after the part is loaded into Unity, before anything happens to it
		public override void OnStart(PartModule.StartState state) {
			//Reset();
			RenderingManager.AddToPostDrawQueue(0, OnDraw);
		}

		private void OnDraw() {

		}

		public override void OnLoad(ConfigNode node) {
			running = Boolean.Parse(node.GetValue("running"));
			bytecode.OnLoad(node.GetNode("BYTECODE"));
			stack.OnLoad(node.GetNode("STACK"));
		}

		public override void OnSave(ConfigNode node) {
			node.AddValue("running", running.ToString());
			ConfigNode bcNode = new ConfigNode("BYTECODE");
			bytecode.OnSave(bcNode);
			node.AddData(bcNode);
			ConfigNode stackNode = new ConfigNode("STACK");
			stack.OnSave(stackNode);
			node.AddData(stackNode);
		}

		public override void OnUpdate() {
			if (running) {
				ops[bytecode.ReadByte()](this);
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
