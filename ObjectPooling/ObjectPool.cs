using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectPooling
{
	internal class ObjectPool : IObjectPool
	{
		#region [Data]

		private int m_id;
		private IObjectManager m_manager;
		private Dictionary<string, IObjectSet> m_data;
		private bool m_autoCleanup;

		#endregion [Data]

		#region [Constructor]

		public ObjectPool(IObjectManager manager, bool autoCleanup = false)
		{
			this.m_id = SequenceId.NextId;
			this.m_manager = manager;
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
				{
					var obj0 = this.m_manager == null ? new ObjectSet() : this.m_manager.Apply("Self.ObjectSet", "admin") as ObjectSet;
					this.m_data.Add(name, obj0);
				}
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
					{
						var obj0 = this.m_data[name];
						this.m_data.Remove(name);
						if (this.m_manager != null && obj0 is IPoolable)
						{
							this.m_manager.Recycle(obj0 as IPoolable);
						}
					}
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

		public void CreateAdminNode()
		{
			IObjectSet s = new ObjectSet();
			this.m_data.Add("admin", s);
		}

		public void Cleanup()
		{
			foreach (var o in this.m_data)
			{
				o.Value.Cleanup();
			}
		}

		#endregion IObjectPool Members

		#region [object]

		public override string ToString()
		{
			return string.Format("{0},Id={1}", this.GetType().Name, this.m_id);
		}

		#endregion [object]

	}
}