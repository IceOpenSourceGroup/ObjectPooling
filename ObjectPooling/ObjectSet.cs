using System;
using System.Collections.Generic;

namespace ObjectPooling
{
	internal class ObjectSet : IObjectSet, IPoolable
	{
		#region [Data]

		private int m_id;
		internal HashSet<IPoolable> m_data;

		#endregion [Data]

		#region [Statistics ]

		private int m_setCount;
		private int m_getCount;

		#endregion [Statistics ]

		#region [Constructor]

		public ObjectSet()
		{
			this.m_data = new HashSet<IPoolable>();
			this.m_id = SequenceId.NextId;
			this.m_setCount = 0;
			this.m_getCount = 0;
		}

		#endregion [Constructor]

		#region IObjectSet Members

		public IPoolable Get()
		{
			lock (this)
			{
				this.m_getCount++;
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
				this.m_setCount++;
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

		public void Cleanup()
		{
			if (this.m_getCount < this.Count)
			{
				int r = (this.Count - this.m_getCount) / 10;
				for (int i = 0; i < r; i++)
					this.Get();
			}
			this.m_getCount = 0;
			this.m_setCount = 0;
		}

		#endregion IObjectSet Members

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

		#region [object]

		public override string ToString()
		{
			return string.Format("{0},id={1}", this.GetType().Name, this.m_id);
		}

		#endregion [object]

	}

	internal class ObjectSetFactory : IPoolableFactory
	{
		#region IPoolableFactory Members

		public IPoolable CreateInstance()
		{
			return new ObjectSet();
		}

		#endregion IPoolableFactory Members
	}
}