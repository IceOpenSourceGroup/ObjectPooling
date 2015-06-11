using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ObjectPooling
{
	internal class ObjectManager : IObjectManager
	{
		#region [Singleton]

		private static ObjectManager m_instance = null;
		private static object thisObject = new object();

		public static ObjectManager Instance
		{
			get
			{
				if (m_instance == null)
				{
					lock (thisObject)
					{
						if (m_instance == null)
						{
							m_instance = new ObjectManager();
						}
					}
				}
				return m_instance;
			}
		}

		#endregion [Singleton]

		#region [Data]

		private Dictionary<string, IPoolableFactory> m_data;
		internal IObjectPool m_idlePool;
		internal IObjectPool m_busyPool;
		private Timer m_timer;
		#endregion [Data]

		#region [Constructor]

		public ObjectManager()
		{
			this.m_data = new Dictionary<string, IPoolableFactory>();
			this.RegistClassType("Self.ObjectSet", new ObjectSetFactory());

			var p = new ObjectPool(this);
			this.m_idlePool = p;

			p = new ObjectPool(this, true);
			p.CreateAdminNode();
			this.m_busyPool = p;
			this.m_timer = new Timer(new TimerCallback(this.Cleanup));
			this.m_timer.Change(1000, 0);
		}

		#endregion [Constructor]

		#region [Callback]

		private void Cleanup(object state)
		{
			this.m_idlePool.Cleanup();
		}

		#endregion [Callback]

		#region IObjectManager Members

		public void RegistClassType(string className, IPoolableFactory factory)
		{
			if (this.m_data.ContainsKey(className))
				throw new ClassExistsException(className);
			this.m_data.Add(className, factory);
		}

		public void UnregistClassType(string className)
		{
			if (this.m_data.ContainsKey(className))
				this.m_data.Remove(className);
		}

		public IPoolable Apply(string className, string usage = null)
		{
			if (!this.m_data.ContainsKey(className))
				throw new ClassNotDefinedException(className);

			//get the obj
			IPoolable obj = this.m_idlePool.Get(className);
			if (obj == null)
			{
				obj = this.m_data[className].CreateInstance();
				obj.ClassName = className;
				obj.Manager = this;
			}

			//put the obj to busy
			if (string.IsNullOrEmpty(usage))
				usage = "System";
			obj.Usage = usage;
			this.m_busyPool.Set(obj, usage);

			return obj;
		}

		public void Recycle(IPoolable obj)
		{
			string usage = obj.Usage;
			this.m_busyPool.Remove(obj, usage);
			this.m_idlePool.Set(obj, obj.ClassName);
		}

		public void Recycle(string usage)
		{
			IPoolable r = null;
			while ((r = this.m_busyPool.Get(usage)) != null)
			{
				this.m_idlePool.Set(r, r.ClassName);
			}
		}

		#endregion IObjectManager Members
	}
}