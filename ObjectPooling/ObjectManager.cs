using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectPooling
{
	internal class ObjectManager : IObjectManager
	{
		#region [Data]

		private Dictionary<string, IPoolableFactory> m_data;
		internal IObjectPool m_idlePool;
		internal IObjectPool m_busyPool;

		#endregion [Data]

		#region [Constructor]

		public ObjectManager()
		{
			this.m_data = new Dictionary<string, IPoolableFactory>();
			this.m_idlePool = new ObjectPool();
			this.m_busyPool = new ObjectPool(true);
		}

		#endregion [Constructor]

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
			this.m_busyPool.Remove(usage);
		}

		#endregion IObjectManager Members
	}
}