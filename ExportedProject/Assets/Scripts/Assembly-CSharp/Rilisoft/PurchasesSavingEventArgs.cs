using System;
using System.Threading.Tasks;

namespace Rilisoft
{
	public class PurchasesSavingEventArgs : EventArgs
	{
		public System.Threading.Tasks.Task<bool> Future { get; private set; }

		public PurchasesSavingEventArgs(System.Threading.Tasks.Task<bool> future)
		{
			Future = future;
		}
	}
}
