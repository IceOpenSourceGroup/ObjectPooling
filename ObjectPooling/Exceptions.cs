using System;

namespace ObjectPooling
{
	public class ClassExistsException : Exception
	{
		public ClassExistsException()
			: base()
		{
		}

		public ClassExistsException(string msg)
			: base(msg)
		{
		}

		public ClassExistsException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}

		public ClassExistsException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
	}

	public class ClassNotDefinedException : Exception
	{
		public ClassNotDefinedException()
			: base()
		{
		}

		public ClassNotDefinedException(string msg)
			: base(msg)
		{
		}

		public ClassNotDefinedException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}

		public ClassNotDefinedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
	}
}