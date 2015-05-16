using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_KISS_Mod {
	public class CodeSet {

		private struct OperationDetails {
			public Operation op;
			public string name;
			public string description;

			public OperationDetails(Operation op, string name, string description) {
				this.op = op;
				this.name = name;
				this.description = description;
			}
		}

		public delegate void Operation(ModuleComputerCore c);

		private List<OperationDetails> ops = new List<OperationDetails>();
		
		public int Count { get { return ops.Count; } }

		public Operation this[int index] {
			get { return ops[index].op; }
		}

		public string getName(int index) {
			return ops[index].name;
		}

		/// <summary>
		/// Adds the operation to the global operation list, and returns an index that can be used to reference this operation in the future.
		/// </summary>
		/// <param name="op">The operation to add</param>
		/// <returns>An index that can be used to access the operation</returns>
		public int Add(string name, Operation op) {
			OperationDetails deets = new OperationDetails(op, name, "No description");
			int index = ops.Count;
			ops.Add(deets);
			return index;
		}

	}
}
