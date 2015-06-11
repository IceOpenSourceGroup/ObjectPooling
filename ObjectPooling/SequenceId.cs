using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectPooling
{
	internal class SequenceId
	{
		private static int m_current = 0;
		private static object thisObject = new object();

		public static int NextId
		{
			get
			{
				lock (thisObject)
				{
					return ++m_current;
				}
			}
		}
	}
}