namespace Sinoze.Engine
{
	public enum SiegePublishOption 
	{
		// The message lives until it's consumed
		// Warning : order of message receive is not guarantee
		SingleComsume,

		// in the next frame after the message is sent to all recipients, server delete the message
		Broadcast,
		
		// after the message is sent to all recipients, server retain the message
		// so when a new subscription occurs, this message is sent
		Persist,
	}
}