using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KSP_KISS_Mod
{
    public class ModuleComputerCore : PartModule
    {

		public static CodeSet ops = new CodeSet();

		public static int DUP  = ops.Add(core => { core.Push(core.Peek()); });
		public static int DROP = ops.Add(core => { core.Pop(); });
		public static int SWAP = ops.Add(core => {
			int a = core.Pop();
			int b = core.Pop();
			core.Push(a);
			core.Push(b);
		});

		public static int POST_INT = ops.Add(core => { core.Push(core.BCNext()); });

		
		//public static Operation DEF_BYTE  = core => { core.MemDef(core.Pop(), core.Pop()); };
		//public static Operation GET  = core => { core.Push(core.MemGet(core.Pop())); };
		//public static Operation DEL  = core => { core.MemDel((string)core.Pop()); };

		public static int IF   = ops.Add(core => {
			bool condition = core.Pop() == 0 ? false : true;
			int f = (int)core.Pop();
			int t = (int)core.Pop();
			if (condition) core.Goto(t);
			else core.Goto(f);
		});
		public static int GOTO = ops.Add(core => { core.Goto(core.Pop()); });
		public static int END  = ops.Add(core => { core.Reset(); });

		public static int EQ   = ops.Add(core => { core.Push(core.Pop() == core.Pop()); });
		public static int GT   = ops.Add(core => { core.Push(core.Pop() > core.Pop()); });
		public static int LT   = ops.Add(core => { core.Push(core.Pop() < core.Pop()); });
		public static int GE   = ops.Add(core => { core.Push(core.Pop() >= core.Pop()); });
		public static int LE   = ops.Add(core => { core.Push(core.Pop() <= core.Pop()); });

		public static int ADD  = ops.Add(core => { core.Push(core.Pop() + core.Pop()); });
		public static int SUB  = ops.Add(core => { core.Push(core.Pop() - core.Pop()); });
		public static int MUL  = ops.Add(core => { core.Push(core.Pop() * core.Pop()); });
		public static int DIV  = ops.Add(core => { core.Push(core.Pop() / core.Pop()); });
		public static int MOD  = ops.Add(core => { core.Push(core.Pop() % core.Pop()); });

		//public static Operation POST_ALTITUDE = core => { core.Push(core.part.vessel.altitude); };
		
		public static int LOG  = ops.Add(core => { print("KISS_MOD: " + core.Peek().ToString()); });
		public static int ECHO = ops.Add(core => { ScreenMessages.PostScreenMessage(core.Pop().ToString()); });
		public static int MSG  = ops.Add(core => {
			ScreenMessageStyle style = (ScreenMessageStyle)core.Pop();
			float dur = core.Pop();
			string msg = core.Pop().ToString();
			ScreenMessages.PostScreenMessage(new ScreenMessage(msg, dur, style));
		});

		/*
		 * copy/paste this to make a new operation
		
		public static int  = ops.Add(core => {  });
		
		 */

		// add 35 + 30, post params for a 2 second centered msg with the result, check result for equality with 65, echo the result
		int[] bytecode = { POST_INT, 35, POST_INT, 30, ADD, DUP, POST_INT, 1, POST_INT, 2, MSG, POST_INT, 65, EQ, ECHO, END };

		// variables used to run the machine
		int[] stack = new int[1024];
		//Dictionary<string, object> memory = new Dictionary<string, object>();
		int stackTop = 0;
		int bcIndex = 0;
		bool running = false;

		/// <summary>
		/// Push an int value to the top of the call stack
		/// </summary>
		public void Push(int data) {
			stackTop++;
			if (stack.Length == stackTop) {
				Array.Resize(ref stack, stack.Length * 2);
			}
			stack[stackTop] = data;
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
			int p = stack[stackTop];
			// this will make stackTop -1 if there is nothing on the stack, but that's ok because nothing will be called when it's empty.
			stackTop--;
			return p;
		}

		/// <summary>
		/// View a value in the call stack, at an optional depth, without removing it.
		/// Depth is how far down the stack to look. Default depth is 0.
		/// depth = 1 would look at the value immediately below the top.
		/// </summary>
		public int Peek(int depth = 0) {
			return stack[stackTop - depth];
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

		// called during the loading screen, used for initializing constants and the like
		public override void OnAwake() {
			
		}

		public override void OnLoad(ConfigNode node) {
			
		}

		public override void OnSave(ConfigNode node) {
			
		}

		// called after the part is loaded into Unity, before anything happens to it
		public override void OnStart(PartModule.StartState state) {
			Reset();
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

		public void Reset() {
			stack = new int[1024];
			//memory = new Dictionary<string, object>();
			stackTop = 0;
			bcIndex = 0;
			running = false;
		}
		
		[KSPEvent(guiActive = true, guiName = "Run Script")]
		public void RunScript() {
			Reset();
			running = true;
			print("KISS is running!!");
		}

    }
}
