namespace Sinoze.Engine
{
	public enum SiegeSubscribeOption
	{
		/// <summary>
		/// The callback is removed as soon as it's called
		/// (or unsubscribed)
		/// </summary>
		SingleComsume,

		/// <summary>
		/// The callback will persist until unsubscribed
		/// </summary>
		Persist,
	}
}