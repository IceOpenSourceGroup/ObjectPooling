using System;
using System.Collections.Generic;

namespace ObjectPooling
{
	internal class ObjectSet : IObjectSet
	{
		#region [Data]

		internal HashSet<IPoolable> m_data;

		#endregion [Data]

		#region [Constructor]

		public ObjectSet()
		{
			this.m_data = new HashSet<IPoolable>();
		}

		#endregion [Constructor]

		#region IObjectSet Members

		public IPoolable Get()
		{
			lock (this)
			{
				if (this.m_data.Count > 0)
				{
					var e = this.m_data.GetEnumerator();
					e.MoveNext();
					var r = e.Current;
					this.Remove(r);
					return r;
				}
				else
					return null;
			}
		}

		public void Set(IPoolable obj)
		{
			lock (this)
			{
				this.m_data.Add(obj);
			}
		}

		public void Remove(IPoolable obj)
		{
			lock (this)
			{
				this.m_data.Remove(obj);
			}
		}

		public void Clear()
		{
			this.m_data.Clear();
		}

		public int Count
		{
			get { return this.m_data.Count; }
		}

		#endregion IObjectSet Members
	}
}