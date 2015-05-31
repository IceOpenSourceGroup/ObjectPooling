using System;

namespace ObjectPooling
{
	/// <summary>
	/// the object that can be pooled
	/// </summary>
	public interface IPoolable : IDisposable
	{
		/// <summary>
		/// the name of class, which is registed to the manager
		/// </summary>
		string ClassName { get; set; }

		/// <summary>
		/// the usage of the object,such as page Id, when the object is in use
		/// </summary>
		string Usage { get; set; }

		/// <summary>
		/// the manager object
		/// </summary>
		IObjectManager Manager { get; set; }
	}

	/// <summary>
	/// the factory,whom can create the object
	/// </summary>
	public interface IPoolableFactory
	{
		/// <summary>
		/// to create a object of the class type
		/// </summary>
		/// <returns>the object created</returns>
		IPoolable CreateInstance();
	}

	/// <summary>
	/// the Manager
	/// </summary>
	public interface IObjectManager
	{
		/// <summary>
		/// to declare a class type
		/// </summary>
		/// <param name="className">name of the class</param>
		/// <param name="factory">the factory of the class</param>
		void RegistClassType(string className, IPoolableFactory factory);

		/// <summary>
		/// to undeckare a class
		/// </summary>
		/// <param name="className">name of the class</param>
		void UnregistClassType(string className);

		/// <summary>
		/// to apply an object
		/// if exist one in IdlePool: move an object from idlepool to busypool
		/// if not:create an object ,and put it to busypool
		/// </summary>
		/// <param name="className">the name of the class</param>
		/// <returns>the class</returns>
		IPoolable Apply(string className, string usage = null);

		/// <summary>
		/// to recycle a obj,move it from busypool to idlepool
		/// </summary>
		/// <param name="obj">the object</param>
		void Recycle(IPoolable obj);

		/// <summary>
		/// to recycle a group of object
		/// </summary>
		/// <param name="usage">the usage of the objects</param>
		void Recycle(string usage);
	}

	/// <summary>
	/// a pool which syore object by branch
	/// </summary>
	public interface IObjectPool
	{
		/// <summary>
		/// get an object from a naming branch
		/// </summary>
		/// <param name="name">the name of the branch</param>
		/// <returns>the object</returns>
		IPoolable Get(string name = null);

		/// <summary>
		/// store to a naming branch
		/// </summary>
		/// <param name="obj">the object that will be stored</param>
		/// <param name="name">the name of the branch</param>
		void Set(IPoolable obj, string name = null);

		/// <summary>
		/// remove a object from the naming branch
		/// </summary>
		/// <param name="obj">the object that will be removed</param>
		/// <param name="name">the name of the branch</param>
		void Remove(IPoolable obj, string name = null);

		/// <summary>
		/// remove the whole set which is called name
		/// </summary>
		/// <param name="name">the name of the set</param>
		void Remove(string name = null);

		/// <summary>
		/// count a set
		/// </summary>
		/// <param name="name">name of the set</param>
		/// <returns>the count value</returns>
		int CountSet(string name = null);

		/// <summary>
		/// count sets
		/// </summary>
		int Count { get; }
	}

	/// <summary>
	/// the set,wich stores objects
	/// </summary>
	internal interface IObjectSet
	{
		/// <summary>
		/// get an object from the set
		/// </summary>
		/// <returns>the object that returns,or null if no object in the set </returns>
		IPoolable Get();

		/// <summary>
		/// store an object to the set
		/// </summary>
		/// <param name="obj">the object </param>
		void Set(IPoolable obj);

		/// <summary>
		/// remove an object from the set
		/// </summary>
		/// <param name="obj"></param>
		void Remove(IPoolable obj);

		/// <summary>
		/// number of objects in the set
		/// </summary>
		int Count { get; }

		/// <summary>
		/// clear all the data in the set
		/// </summary>
		void Clear();
	}
}