
namespace Sinoze.Engine
{
	/// <summary>
	/// inline Assert.
	/// </summary>
	public static class Assert
	{
		/// <summary>
		/// True the specified shouldBeTrue and message.
		/// </summary>
		/// <param name="shouldBeTrue">If set to <c>true</c> should be true.</param>
		/// <param name="message">Message.</param>
		public static void True(bool shouldBeTrue, string message = null, AssertLevel assertLevel = AssertLevel.Warning, params string[] tags)
		{
			if(!shouldBeTrue)
			{
				if(string.IsNullOrEmpty(message))
					message = "(No Message)";

				var logType = LogType.Warning;
				var fatalException = default(System.Exception);
				if(assertLevel == AssertLevel.Critical)
				{
					logType = LogType.Error;
				}
				else if(assertLevel == AssertLevel.Fatal)
				{
					logType = LogType.Exception;
					fatalException = new System.Exception("Fatal Assert : " + message);
				}

				Logger.LogAssert("Assert Fail : " + message, logType, fatalException, tags);

				// just quit application if fatal assert is found
	//				if(assertLevel == AssertLevel.Fatal)
	//				{
	//					throw fatalException;
	//				}
			}
		}
	}

	public enum AssertLevel
	{
		Fatal,
		Critical, // log error
		Warning, // log warning
	}
}