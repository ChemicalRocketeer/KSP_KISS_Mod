using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_KISS_Mod {
	public class CodeSet {

		public delegate void Operation(ModuleComputerCore c);

		private List<Operation> ops = new List<Operation>();
		private List<string> names = new List<string>();
		private List<string> descriptions = new List<string>();
		
		public int Count { get { return ops.Count; } }

		public Operation this[int index] {
			get { return ops[index]; }
		}

		/// <summary>
		/// Adds the operation to the global operation list, and returns an index that can be used to reference this operation in the future.
		/// </summary>
		/// <param name="op">The operation to add</param>
		/// <returns>An index that can be used to access the operation</returns>
		public int Add(Operation op) {
			int index = ops.Count;
			ops.Add(op);
			return index;
		}

	}
}
