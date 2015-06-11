using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectPooling;

namespace ObjectPoolingTest
{
	[TestClass]
	public class basicFunction
	{
		private Random rnd = new Random((int)DateTime.Now.Ticks);

		private string[] CFirstName = "一.二.三.四.五.六.七.八".Split('.');
		private string[] CLastName = "赵.钱.孙.李.周.吴.郑.王".Split('.');
		private string[] EFirstName = "First.Second.Third.Forth.Fifth.Sixth.Seventh.Enghth".Split('.');
		private string[] ELastName = "Zhao.Qian.Sun.Li.Zhou.Wu.Zheng.Wang".Split('.');

		private TestTarget1 InitlizeTestTarget1()
		{
			int i0 = rnd.Next() % 100;
			int i1 = rnd.Next() % 8;
			int i2 = rnd.Next() % 8;
			var t = new TestTarget1();
			t.Id = i0;
			t.Name = string.Format("{0} {1}", EFirstName[i1], ELastName[i2]);
			t.Description = string.Format("{1}{0}", CFirstName[i1], CLastName[i2]);
			return t;
		}

		private TestTarget2 InitlizeTestTarget2()
		{
			int i0 = rnd.Next() % 100;
			int i1 = rnd.Next() % 8;
			int i2 = rnd.Next() % 8;
			var t = new TestTarget2();
			t.Id = i0;
			t.Name = string.Format("{0} {1}", EFirstName[i1], ELastName[i2]);
			t.Description = string.Format("{1}{0}", CFirstName[i1], CLastName[i2]);
			return t;
		}

		[TestMethod]
		public void TestObjectSet()
		{
			IPoolable r = null;
			var tt11 = this.InitlizeTestTarget1();
			var tt12 = this.InitlizeTestTarget1();
			var tt13 = this.InitlizeTestTarget1();

			///
			/// new
			///
			ObjectSet set = new ObjectSet(); Assert.AreEqual(set.Count, 0);

			///
			/// set
			///
			set.Set(tt11);
			Assert.AreEqual(set.Count, 1);

			set.Set(tt12);
			Assert.AreEqual(set.Count, 2);

			set.Set(tt12);
			Assert.AreEqual(set.Count, 2);

			///
			/// Get
			///
			r = set.Get();
			Assert.AreEqual(r.ToString(), tt11.ToString());
			Assert.AreEqual(set.Count, 1);
			Assert.AreSame(r, tt11);
			r = set.Get();
			Assert.AreEqual(r.ToString(), tt12.ToString());
			Assert.AreEqual(set.Count, 0);
			Assert.AreSame(r, tt12);

			r = set.Get();
			Assert.IsNull(r);
			Assert.AreEqual(set.Count, 0);

			///
			/// Remove1
			///
			set.Set(tt11);
			Assert.AreEqual(set.Count, 1);

			set.Set(tt12);
			Assert.AreEqual(set.Count, 2);

			set.Remove(tt12);
			Assert.AreEqual(set.Count, 1);

			r = set.Get();
			Assert.AreEqual(r.ToString(), tt11.ToString());
			Assert.AreEqual(set.Count, 0);
			Assert.AreSame(r, tt11);

			///
			/// Remove2
			///
			set.Set(tt12);
			Assert.AreEqual(set.Count, 1);

			set.Remove(tt13);
			Assert.AreEqual(set.Count, 1);

			set.Remove(tt12);
			Assert.AreEqual(set.Count, 0);

			set.Remove(tt11);
			Assert.AreEqual(set.Count, 0);

			///
			/// Clear
			///
			set.Set(tt12);
			Assert.AreEqual(set.Count, 1);

			set.Set(tt13);
			Assert.AreEqual(set.Count, 2);

			set.Clear();
			Assert.AreEqual(set.Count, 0);
		}

		[TestMethod]
		public void TestObjectPool()
		{
			IPoolable r = null;
			var tt11 = this.InitlizeTestTarget1();
			var tt12 = this.InitlizeTestTarget1();
			var tt13 = this.InitlizeTestTarget1();
			var tt21 = this.InitlizeTestTarget2();
			var tt22 = this.InitlizeTestTarget2();
			var tt23 = this.InitlizeTestTarget2();
			string type1 = typeof(TestTarget1).Name;
			string type2 = typeof(TestTarget2).Name;

			///
			/// new
			///
			ObjectPool pool = new ObjectPool(null);
			r = pool.Get(tt11.GetType().Name);
			Assert.IsNull(r);
			Assert.AreEqual(pool.Count, 0);

			///
			/// Set & Get & Count
			///
			pool.Set(tt11, type1);
			Assert.IsNull(pool.Get(type2));
			Assert.AreEqual(pool.Count, 1);

			pool.Set(tt12, type1);
			Assert.IsNull(pool.Get(type2));
			Assert.AreEqual(pool.Count, 1);

			pool.Set(tt21, type2);
			Assert.AreSame(pool.Get(type2), tt21);
			Assert.AreEqual(pool.Count, 2);

			pool.Set(tt22, type2);
			Assert.AreSame(pool.Get(type1), tt11);
			Assert.AreEqual(pool.Count, 2);

			Assert.IsNull(pool.Get("unknown"));
			Assert.IsNull(pool.Get(null));
			Assert.AreSame(pool.Get(type1), tt12);
			Assert.IsNull(pool.Get(type1));
			Assert.AreSame(pool.Get(type2), tt22);
			Assert.IsNull(pool.Get(type2));

			///
			/// Remove
			///
			pool.Set(tt11, type1);
			Assert.AreEqual(pool.CountSet(type1), 1);
			Assert.AreEqual(pool.CountSet(type2), 0);

			pool.Set(tt12, type1);
			Assert.AreEqual(pool.CountSet(type1), 2);
			Assert.AreEqual(pool.CountSet(type2), 0);

			pool.Set(tt21, type2);
			Assert.AreEqual(pool.CountSet(type1), 2);
			Assert.AreEqual(pool.CountSet(type2), 1);

			pool.Set(tt22, type2);
			Assert.AreEqual(pool.CountSet(type1), 2);
			Assert.AreEqual(pool.CountSet(type2), 2);

			pool.Remove(tt12, type1);
			Assert.AreEqual(pool.CountSet(type1), 1);
			Assert.AreEqual(pool.CountSet(type2), 2);

			pool.Remove(tt12, type1);
			Assert.AreEqual(pool.CountSet(type1), 1);
			Assert.AreEqual(pool.CountSet(type2), 2);

			pool.Remove(tt11, type1);
			Assert.AreEqual(pool.CountSet(type1), 0);
			Assert.AreEqual(pool.CountSet(type2), 2);

			pool.Remove(tt11, type1);
			Assert.AreEqual(pool.CountSet(type1), 0);
			Assert.AreEqual(pool.CountSet(type2), 2);

			pool.Remove(type2);
			Assert.AreEqual(pool.CountSet(type1), 0);
			Assert.AreEqual(pool.CountSet(type2), 0);
		}

		[TestMethod]
		public void TestObjectManager()
		{
			IPoolable r = null;
			string type1 = typeof(TestTarget1).Name;
			string type2 = typeof(TestTarget2).Name;

			//			ObjectManager manager = new ObjectManager();
			ObjectManager manager = ObjectManager.Instance;
			Assert.AreEqual(manager.m_idlePool.Count, 0);
			Assert.AreEqual(manager.m_busyPool.Count, 1);

			///
			/// RegistClass
			///
			manager.RegistClassType(type1, new TestTarget1Factory());
			Assert.AreEqual(manager.m_idlePool.Count, 0);
			Assert.AreEqual(manager.m_busyPool.Count, 1);

			manager.RegistClassType(type2, new TestTarget2Factory());
			Assert.AreEqual(manager.m_idlePool.Count, 0);
			Assert.AreEqual(manager.m_busyPool.Count, 1);

			///
			/// Apply
			///
			/*
			 * BusyPool:Create Set(admin)
			 * BusyPool:Create Set(test)
			 *	==>[busypool(admin)==1]
			 */
			var tt11 = manager.Apply(type1, "test");
			(tt11 as TestTarget1).Id = 11;
			Assert.AreEqual(tt11.GetType().Name, type1);
			Assert.AreEqual(manager.m_busyPool.CountSet("test"), 1);
			Assert.AreEqual(manager.m_busyPool.Count, 2);
			Assert.AreEqual(manager.m_busyPool.CountSet("admin"), 1);

			/*
			 * BusyPool:Create Set(System)
			 *	==>[busypool(admin)==2]
			 */
			var tt12 = manager.Apply(type1);
			(tt12 as TestTarget1).Id = 12;
			Assert.AreEqual(tt12.GetType().Name, type1);
			Assert.AreEqual(manager.m_busyPool.CountSet(), 1);
			Assert.AreEqual(manager.m_busyPool.Count, 3);
			Assert.AreEqual(manager.m_busyPool.CountSet("admin"), 2);

			/*
			 * BusyPool:no change
			 */
			var tt21 = manager.Apply(type2, "test");
			(tt21 as TestTarget2).Id = 21;
			Assert.AreEqual(tt21.GetType().Name, type2);
			Assert.AreEqual(manager.m_busyPool.CountSet("test"), 2);
			Assert.AreEqual(manager.m_busyPool.Count, 3);
			Assert.AreEqual(manager.m_busyPool.CountSet("admin"), 2);

			/*
			 * BusyPool:no change
			 */
			var tt22 = manager.Apply(type2);
			(tt22 as TestTarget2).Id = 22;
			Assert.AreEqual(tt22.GetType().Name, type2);
			Assert.AreEqual(manager.m_busyPool.CountSet(), 2);
			Assert.AreEqual(manager.m_busyPool.Count, 3);

			///
			/// Recycle
			///
			/*
			 * IdlePool:create set(TestTarget1)
			 *	==>[busypool(admin)==3]
			 */
			tt11.Dispose();
			Assert.AreEqual(manager.m_idlePool.Count, 1);
			Assert.AreEqual(manager.m_idlePool.CountSet(type1), 1);
			Assert.AreEqual(manager.m_busyPool.Count, 3);
			Assert.AreEqual(manager.m_busyPool.CountSet("admin"), 3);

			/*
			 */
			tt12.Dispose();
			Assert.AreEqual(manager.m_idlePool.Count, 1);
			Assert.AreEqual(manager.m_idlePool.CountSet(type1), 2);
			Assert.AreEqual(manager.m_busyPool.Count, 3);
			Assert.AreEqual(manager.m_busyPool.CountSet("admin"), 3);

			/*
			 * BusyPool:remove set(test)
			 *	==>[busypool(admin)==2]
			 *	==>[idlepool:create(Self.ObjectSet)]
			 *		==>[busypool(admin)==3]
			 *		&&>[idlepool(Self.ObjectSet)==1]
			 * IdlePool:reuse(TestTarget1)
			 *	==>[busypool(admin)==4]
			 *	&&>[idlepool(Self.ObjectSet)==0]
			 */
			tt21.Dispose();
			Assert.AreEqual(manager.m_idlePool.Count, 3);
			Assert.AreEqual(manager.m_idlePool.CountSet(type2), 1);
			Assert.AreEqual(manager.m_busyPool.Count, 2);
			Assert.AreEqual(manager.m_idlePool.CountSet("Self.ObjectSet"), 0);
			Assert.AreEqual(manager.m_busyPool.CountSet("admin"), 4);

			/*
			 * BusyPool:remove set(System)
			 *	==>[busypool(admin)==3]
			 *	&&>[idlepool(Self.ObjectSet)==1]
			 */
			tt22.Dispose();
			Assert.AreEqual(manager.m_idlePool.Count, 3);
			Assert.AreEqual(manager.m_idlePool.CountSet(type2), 2);
			Assert.AreEqual(manager.m_busyPool.Count, 1);

			Assert.AreEqual(manager.m_busyPool.CountSet("test"), 0);
			Assert.AreEqual(manager.m_busyPool.CountSet(""), 0);

			///
			/// ReApply & Recycle
			///
			r = manager.Apply(type1, "test");
			Assert.AreEqual(manager.m_busyPool.CountSet("test"), 1);
			Assert.AreEqual(r.GetType().Name, type1);
			Assert.AreEqual(manager.m_idlePool.CountSet(type1), 1);
			Assert.AreEqual(manager.m_idlePool.CountSet(type2), 2);

			r = manager.Apply(type1, "test");
			Assert.AreEqual(manager.m_busyPool.CountSet("test"), 2);
			Assert.AreEqual(r.GetType().Name, type1);
			Assert.AreEqual(manager.m_idlePool.CountSet(type1), 0);
			Assert.AreEqual(manager.m_idlePool.CountSet(type2), 2);

			r = manager.Apply(type2, "test");
			Assert.AreEqual(manager.m_busyPool.CountSet("test"), 3);
			Assert.AreEqual(r.GetType().Name, type2);
			Assert.AreEqual(manager.m_idlePool.CountSet(type1), 0);
			Assert.AreEqual(manager.m_idlePool.CountSet(type2), 1);

			r = manager.Apply(type2, "test");
			Assert.AreEqual(manager.m_busyPool.CountSet("test"), 4);
			Assert.AreEqual(r.GetType().Name, type2);
			Assert.AreEqual(manager.m_idlePool.CountSet(type1), 0);
			Assert.AreEqual(manager.m_idlePool.CountSet(type2), 0);

			r = manager.Apply(type2, "test");
			Assert.AreEqual(manager.m_busyPool.CountSet("test"), 5);
			Assert.AreEqual(r.GetType().Name, type2);
			Assert.AreEqual(manager.m_idlePool.CountSet(type1), 0);
			Assert.AreEqual(manager.m_idlePool.CountSet(type2), 0);

			manager.Recycle("test");
			Assert.AreEqual(manager.m_busyPool.CountSet("test"), 0);
			Assert.AreEqual(manager.m_idlePool.CountSet(type1), 2);
			Assert.AreEqual(manager.m_idlePool.CountSet(type2), 3);
		}
	}
}