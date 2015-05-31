using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectPoolingTest
{
	internal class TestTarget1 : ObjectPooling.PoolableObjectBase
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
	}

	internal class TestTarget1Factory : ObjectPooling.IPoolableFactory
	{
		#region IPoolableFactory Members

		public ObjectPooling.IPoolable CreateInstance()
		{
			return new TestTarget1();
		}

		#endregion IPoolableFactory Members
	}
}