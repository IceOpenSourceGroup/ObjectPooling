using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectPoolingTest
{
	internal class TestTarget2 : ObjectPooling.IPoolable
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public override string ToString()
		{
			return string.Format("{0};Id={1};Name={2};Description={3}",
				this.GetType().FullName,
				this.Id,
				this.Name,
				this.Description);
		}

		#region IPoolable Members

		public string ClassName { get; set; }

		public string Usage { get; set; }

		public ObjectPooling.IObjectManager Manager { get; set; }

		#endregion IPoolable Members

		#region IDisposable Members

		public void Dispose()
		{
			if (this.Manager != null)
				this.Manager.Recycle(this);
		}

		#endregion IDisposable Members
	}

	internal class TestTarget2Factory : ObjectPooling.IPoolableFactory
	{
		#region IPoolableFactory Members

		public ObjectPooling.IPoolable CreateInstance()
		{
			return Activator.CreateInstance(typeof(TestTarget2)) as ObjectPooling.IPoolable;
		}

		#endregion IPoolableFactory Members
	}
}