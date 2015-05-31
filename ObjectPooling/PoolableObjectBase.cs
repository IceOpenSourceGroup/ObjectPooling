using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectPooling
{
	public class PoolableObjectBase : IPoolable
	{
		#region IPoolable Members

		public string ClassName { get; set; }

		public string Usage { get; set; }

		public IObjectManager Manager { get; set; }

		#endregion IPoolable Members

		#region IDisposable Members

		public void Dispose()
		{
			if (this.Manager != null)
				this.Manager.Recycle(this);
		}

		#endregion IDisposable Members
	}
}