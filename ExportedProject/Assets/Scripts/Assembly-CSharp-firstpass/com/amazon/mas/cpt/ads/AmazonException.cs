using System;
using System.Runtime.Serialization;

namespace com.amazon.mas.cpt.ads
{
	public class AmazonException : ApplicationException
	{
		public AmazonException()
		{
		}

		public AmazonException(string message)
			: base(message)
		{
		}

		public AmazonException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected AmazonException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
