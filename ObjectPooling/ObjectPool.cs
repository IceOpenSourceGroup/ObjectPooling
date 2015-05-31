using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectPooling
{
	internal class ObjectPool : IObjectPool
	{
		#region [Data]

		private Dictionary<string, IObjectSet> m_data;
		private bool m_autoCleanup;

		#endregion [Data]

		#region [Constructor]

		public ObjectPool(bool autoCleanup = false)
		{
			this.m_data = new Dictionary<string, IObjectSet>();
			this.m_autoCleanup = autoCleanup;
		}

		#endregion [Constructor]

		#region IObjectPool Members

		public IPoolable Get(string name = null)
		{
			if (string.IsNullOrEmpty(name)) name = "System";
			lock (this)
			{
				if (this.m_data.ContainsKey(name))
					return this.m_data[name].Get();
				else
					return null;
			}
		}

		public void Set(IPoolable obj, string name = null)
		{
			if (string.IsNullOrEmpty(name)) name = "System";
			lock (this)
			{
				if (!this.m_data.ContainsKey(name))
					this.m_data.Add(name, new ObjectSet());
				this.m_data[name].Set(obj);
			}
		}

		public void Remove(IPoolable obj, string name = null)
		{
			if (string.IsNullOrEmpty(name)) name = "System";
			lock (this)
			{
				if (this.m_data.ContainsKey(name))
				{
					this.m_data[name].Remove(obj);
					if (this.m_data[name].Count == 0)
						this.m_data.Remove(name);
				}
			}
		}

		public void Remove(string name = null)
		{
			if (string.IsNullOrEmpty(name)) name = "System";

			lock (this)
			{
				if (this.m_data.ContainsKey(name))
				{
					this.m_data[name].Clear();
					this.m_data.Remove(name);
				}
			}
		}

		public int CountSet(string name = null)
		{
			if (string.IsNullOrEmpty(name)) name = "System";

			lock (this)
			{
				if (this.m_data.ContainsKey(name))
					return this.m_data[name].Count;
				else
					return 0;
			}
		}

		public int Count
		{
			get { return this.m_data.Count; }
		}

		#endregion IObjectPool Members
	}
}