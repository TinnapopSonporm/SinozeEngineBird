using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sinoze.Engine
{
	/// <summary>
	/// Siege Client
	/// 
	/// A standalone object. User can create as many Siege clients as they want on the fly at any time by calling Siege<T>.Create()
	/// Think of it as a telecommunication device where you can send and receive messages through it 
	/// but with more advanced features e.g. send to a specific recipient, or the sent message is persisted 
	/// through a couple game engine frames.
	/// 
	/// Note : When a Siege client is disposed, messages created by that client still live.
	/// 
	/// For more info about Siege
	/// See : https://docs.google.com/a/sinozegames.com/document/d/1uzRSE6I2TttUczxbC0LrN3N0cp53ziu40eQpLhP64Lk/edit?usp=sharing
	/// </summary>
	public sealed class Siege : IDisposable
	{
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the tags.
		/// </summary>
		/// <value>The tags.</value>
		public ReadOnlyCollection<string> Tags { get; private set; }

		/// <summary>
		/// The creator stack trace.
		/// </summary>
		private System.Diagnostics.StackTrace creatorStackTrace;

		private SiegeServer server;

		/// <summary>
		/// Initializes a new instance of Siege client
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="tags">Tags.</param>
		public Siege(string name = null, params string[] tags)
		{
			this.server = Module.Find<SiegeServer>();
			this.Name = name;
			this.Tags = new ReadOnlyCollection<string>(tags);
			creatorStackTrace = Diagnostic.GetStackTrace(1);
		}
	
		#region Publish
		
		List<Sinoze.Engine.SiegeServer.SiegeRef> publishRefs = new List<Sinoze.Engine.SiegeServer.SiegeRef>(); // collect all unique publish ids for later disposal
		int publishCount;
		int persistentPublishCount; // Have to seperate the count because we need to use this in the destructor

		/// <summary>
		/// Post an anonymous message to server (which forward to every Siege instances including itself)
		/// NOTE : the posting message will be sent to the other Siege instances in the next frame
		/// </summary>
		/// <param name="message">Message.</param>
		public void Publish(object message, SiegePublishOption postOption = SiegePublishOption.Broadcast, string topic = "")
		{
			if(_isDisposed)
				return;

			Assert.True(topic != null, "Siege topic can't be null"); 
			var publishRef = server.EnqueuePublish(this, message, postOption, topic);
			publishRefs.Add(publishRef);
			publishCount++;
			if(postOption == SiegePublishOption.Persist)
				persistentPublishCount++;
		}

		/// <summary>
		/// Shortcut to anonymously publish with Broadcast option
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="topic">Topic.</param>
		public static void Broadcast(object message, string topic = "")
		{
			Module.Find<SiegeServer>().EnqueuePublish(null, message, SiegePublishOption.Broadcast, topic);
		}

		/// <summary>
		/// Unpublishs all.
		/// </summary>
		public void UnpublishAll()
		{
			if(_isDisposed)
			{
				Logger.LogWarning("This Siege was already disposed while calling UnsubscribeAll()");
				return;
			}
			
			foreach(var publishRef in publishRefs) 
			{
				server.TryEnqueueUnpublish(this, publishRef);
			}
			publishRefs.Clear ();
			publishCount = 0;
			persistentPublishCount = 0;
		}

		#endregion

		#region Subscribe

		List<Sinoze.Engine.SiegeServer.SiegeRef> subscriptionRefs = new List<Sinoze.Engine.SiegeServer.SiegeRef>(); // collect all unique subscription ids for later disposal
		int subscriptionCount; // Have to seperate the count because we need to use this in the destructor

		/// <summary>
		/// Subscribe to a topic
		/// NOTE : all subscription callbacks will be unsubscribed a frame after Dispose() is called
		/// </summary>
		/// <param name="callback">Callback.</param>
		/// <param name="subscribeOption">Subscribe option, default is SingleConsume</param>
		/// <param name="topic">Topic, will use anonymous topic if not specified</param>
		public void Subscribe(Action<Siege, object> callback, SiegeSubscribeOption subscribeOption = SiegeSubscribeOption.SingleComsume, string topic = "")
		{
			if(_isDisposed)
			{
				Logger.LogWarning("This Siege was already disposed while calling Subscribe()");
				return;
			}
			
			Assert.True(topic != null); // topic can't be null, default is string.Empty

			var subscribeRef = server.EnqueueSubscribe(this, callback, subscribeOption, topic);
			subscriptionRefs.Add(subscribeRef);
			subscriptionCount++;
		}

		/// <summary>
		/// Unsubscribes all.
		/// </summary>
		public void UnsubscribeAll()
		{
			if(_isDisposed)
			{
				Logger.LogWarning("This Siege was already disposed while calling UnsubscribeAll()");
				return;
			}

			foreach(var subscribeRef in subscriptionRefs) 
			{
				server.TryEnqueueUnsubscribe(this, subscribeRef);
			}
			subscriptionRefs.Clear ();
			subscriptionCount = 0;
		}

		#endregion
		
		#region IDisposable implementation
		
		private bool _isDisposed;
		
		/// <summary>
		/// Creator of this object should call Dispose if it's no longer in use.
		/// NOTE : the actual unpublish and unsubscription will be done in the next frame
		/// </summary>
		public void Dispose ()
		{
			if(_isDisposed)
				return;

			//if(SiegeServer.Instance != null) // check existance this way to prevent Unity error when creating gameObject while the game is stopping
			{
				UnpublishAll();
				UnsubscribeAll();
			}

			_isDisposed = true;
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the Siege instance
		/// is reclaimed by garbage collection.
		/// 
		/// this will check whether the user have called Dispose()
		/// a warning message will be raised if this Siege instance used to subscribe
		/// </summary>
		~Siege()
		{
			if(!_isDisposed)
			{
				if(subscriptionCount > 0)
				{
					// can't use logger here since we can't predict when the destructor is called
					UnityEngine.Debug.LogError("Possible Logic Error : a Siege client with subscription was not properly disposed. (Did you forget to dispose of your siege instance?)\n" 
					                           + Diagnostic.GetTraceString(creatorStackTrace));
				}
				if(persistentPublishCount > 0)
				{
					// can't use logger here since we can't predict when the destructor is called
					UnityEngine.Debug.LogError("Possible Logic Error : a Siege client with persistent publish was not properly disposed. (Did you forget to dispose of your siege instance?)\n" 
					                           + Diagnostic.GetTraceString(creatorStackTrace));
				}
			}
		}
		
		#endregion
	}
}