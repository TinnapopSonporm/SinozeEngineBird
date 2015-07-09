using System;
using System.Collections.Generic;
using Sinoze.Engine.Collections;

namespace Sinoze.Engine
{
	[Module]
	public sealed class SiegeServer : AdaptableService, IUpdatable
	{
		private static SiegeServer _instance;

		public SiegeServer() 
		{
			if(_instance != null)
				throw new InvalidOperationException("Logger already instanciated");
			_instance = this;
		}

		Dictionary<int, SiegeGroup> siegeGroups = new Dictionary<int, SiegeGroup>();
		Dictionary<string, int> topicToGroupId = new Dictionary<string, int>();
		
		#region IUpdatable
		public void Update()
		{
			foreach(var group in siegeGroups)
			{
				group.Value.Update();
			}
		}

		/// <summary>
		/// This will process Add/Remove queue
		/// </summary>
		public void LateUpdate()
		{
			foreach(var group in siegeGroups)
			{
				group.Value.LateUpdate();
			}
		}
		#endregion

		public override string ToString ()
		{
			var result = string.Format ("[SiegeServer: ]" + siegeGroups.Count + " topics");
			foreach(var g in siegeGroups)
				result += "\n" + g.ToString ();
			return result;
		}

		#region Internal API Call from Siege instances

		/// <summary>
		/// Enqueues the post.
		/// </summary>
		/// <returns>The post.</returns>
		/// <param name="sender">Sender, can be null if called by anonymous Siege.Post() API</param>
		/// <param name="message">Message.</param>
		/// <param name="optionOption">Option option.</param>
		/// <param name="groupId">Group identifier.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		internal SiegeRef EnqueuePublish(Siege sender, object message, SiegePublishOption publishOption, string topic)
		{
			return GetSiegeGroup(topic).EnqueuePublish(sender, message, publishOption);
		}
		
		internal SiegeRef EnqueueSubscribe(Siege sender, Action<Siege, object> callback, SiegeSubscribeOption subscribeOption, string topic)
		{
			return GetSiegeGroup(topic).EnqueueSubscribe(sender, callback, subscribeOption);
		}
		
		internal void TryEnqueueUnpublish(Siege sender, SiegeRef publishRef)
		{
			GetSiegeGroup(publishRef.groupId).TryEnqueueUnpublish(sender, ref publishRef);
		}

		internal void TryEnqueueUnsubscribe(Siege sender, SiegeRef subscribeRef)
		{
			GetSiegeGroup(subscribeRef.groupId).TryEnqueueUnsubscribe(sender, ref subscribeRef);
		}
		
		int uniqueGroupId;
		private SiegeGroup GetSiegeGroup(string topic)
		{
			int groupId;
			if(!topicToGroupId.TryGetValue(topic, out groupId))
			{
				groupId = uniqueGroupId++;
				topicToGroupId.Add(topic, groupId);
			}
			return GetSiegeGroup(groupId);
		}

		private SiegeGroup GetSiegeGroup(int groupId)
		{
			SiegeGroup group;
			if(!siegeGroups.TryGetValue(groupId, out group))
			{
				group = new SiegeGroup(groupId);
				siegeGroups.Add(groupId, group);
			}
			return group;
		}
		#endregion

		#region Publish and Subscribe Internal Data Structures

		class SiegeGroup
		{
			int groupId;
			public SiegeGroup(int groupId)
			{
				this.groupId = groupId;
			}

			public override string ToString ()
			{
				return string.Format ("[SiegeGroup] posts = " + publishPool.occupiedSlotIndices.Count + " listeners = " + subscribePool.occupiedSlotIndices.Count);
			}

			// items pool
			SiegePublishPool publishPool = new SiegePublishPool();
			SiegeListenPool subscribePool = new SiegeListenPool();
			
			// add queue
			Queue<Args<Siege, object, SiegePublishOption, SiegeRef>> publishToAdd = new Queue<Args<Siege, object, SiegePublishOption, SiegeRef>>();
			Queue<Args<Siege, Action<Siege, object>, SiegeSubscribeOption, SiegeRef>> subscribeToAdd = new Queue<Args<Siege, Action<Siege, object>, SiegeSubscribeOption, SiegeRef>>();
			
			// remove queue
			Queue<int> publishToRemove = new Queue<int>();
			Queue<int> subscribeToRemove = new Queue<int>();
			
			public SiegeRef EnqueuePublish(Siege sender, object message, SiegePublishOption publishOption)
			{
				// immediately allocate a slot but enqueue to add in the next frame
				var publishRef = publishPool.Reserve(sender, groupId);
				publishToAdd.Enqueue(new Args<Siege, object, SiegePublishOption, SiegeRef>(ref sender, ref message, ref publishOption, ref publishRef));
				return publishRef;
			}
			
			public SiegeRef EnqueueSubscribe(Siege sender, Action<Siege, object> callback, SiegeSubscribeOption subscribeOption)
			{
				// immediately allocate a slot but enqueue to add in the next frame
				var subscribeRef = subscribePool.Reserve(sender, groupId);
				subscribeToAdd.Enqueue(new Args<Siege, Action<Siege, object>, SiegeSubscribeOption, SiegeRef>(ref sender, ref callback, ref subscribeOption, ref subscribeRef));
				return subscribeRef;
			}

			public bool TryEnqueueUnpublish(Siege sender, ref SiegeRef publishRef)
			{
				var publishSlotIndex = publishRef.slotIndex;
				
				if(publishPool.items[publishSlotIndex].disabled_removing)
					return false;
				
				if(publishPool.items[publishSlotIndex].publishId == publishRef.entityId)
				{
					Assert.True(publishPool.items[publishSlotIndex].sender == sender);
					
					// mark the slot to be disabled so it won't get call anymore in Update
					publishPool.items[publishSlotIndex].disabled_removing = true;
					publishToRemove.Enqueue(publishSlotIndex);
					
					return true;
				}
				
				return false;
			}


			/// <summary>
			/// Unsubscribe when a Siege is being disposed
			/// </summary>
			/// <param name="sender">Sender.</param>
			/// <param name="subscribeRef">Subscribe reference.</param>
			public bool TryEnqueueUnsubscribe(Siege sender, ref SiegeRef subscribeRef)
			{
				var subscribeSlotIndex = subscribeRef.slotIndex;
				
				if(subscribePool.items[subscribeSlotIndex].disabled_removing)
					return false;
				
				if(subscribePool.items[subscribeSlotIndex].subscribeId == subscribeRef.entityId)
				{
					Assert.True(subscribePool.items[subscribeSlotIndex].sender == sender);

					// mark the slot to be disabled so it won't get call anymore in Update
					subscribePool.items[subscribeSlotIndex].disabled_removing = true;
					subscribeToRemove.Enqueue(subscribeSlotIndex);

					return true;
				}

				return false;
			}

			/// <summary>
			/// send posts to subscribers
			/// </summary>
			public void Update()
			{
				for(int i=0;i<publishPool.occupiedSlotIndices.Count;i++)
				{
					var publishSlotIndex = publishPool.occupiedSlotIndices[i];

					// this publish is still in queue, so skip
					if(!publishPool.items[publishSlotIndex].enabled_added)
						continue;

					for(int j=0;j<subscribePool.occupiedSlotIndices.Count;j++)
					{
						var subscribeSlotIndex = subscribePool.occupiedSlotIndices[j];

						// this subscription is still in queue, so skip
						if(!subscribePool.items[subscribeSlotIndex].enabled_added)
							continue;

						// this subscription is about to be removed, so skip
						if(subscribePool.items[subscribeSlotIndex].disabled_removing)
							continue;

						// message will be sent to receiver if
						// 1. this message is just arrived (1st frame) or
						// 2. this message is persisted and the listener just subscribe (1st frame)
						if(publishPool.items[publishSlotIndex].frameCount == 0 || 
						   (publishPool.items[publishSlotIndex].publishOption == SiegePublishOption.Persist && subscribePool.items[subscribeSlotIndex].frameCount == 0))
						{
							// send message to listener
							subscribePool.items[subscribeSlotIndex].callCount++;
							subscribePool.items[subscribeSlotIndex].callback(publishPool.items[publishSlotIndex].sender, publishPool.items[publishSlotIndex].message);
							
							if(subscribePool.items[subscribeSlotIndex].subscribeOption == SiegeSubscribeOption.SingleComsume)
							{
								// for SingleComsume listen option
								// immediately remove it as soon as it's received a message
								subscribePool.items[subscribeSlotIndex].disabled_removing = true;
								subscribeToRemove.Enqueue(subscribeSlotIndex);
							}
							
							if(publishPool.items[publishSlotIndex].publishOption == SiegePublishOption.SingleComsume)
							{
								// for SingleComsume post option
								// immediately remove it as soon as it's sent to a listener
								publishPool.items[publishSlotIndex].disabled_removing = true;
								publishToRemove.Enqueue(publishSlotIndex);
								break;
							}
						}
					}
					
					if(publishPool.items[publishSlotIndex].publishOption == SiegePublishOption.Broadcast)
					{
						// for SingleFrame post option
						// after send this post to all listeners, remove it
						publishPool.items[publishSlotIndex].disabled_removing = true;
						publishToRemove.Enqueue(publishSlotIndex);
					}
					
					// count frame
					publishPool.items[publishSlotIndex].frameCount++;
				}
				
				for(int j=0;j<subscribePool.occupiedSlotIndices.Count;j++)
				{
					var subscribeSlotIndex = subscribePool.occupiedSlotIndices[j];

					// this subscription is still in queue, so skip
					if(!subscribePool.items[subscribeSlotIndex].enabled_added)
						continue;

					// just count, eventhough it might be removed in the next LateUpdate
					subscribePool.items[subscribeSlotIndex].frameCount++;
				}
			}

			/// <summary>
			/// process the add and remove queue
			/// </summary>
			public void LateUpdate()
			{
				// clean up
				while(publishToRemove.Count > 0)
				{
					var toFree = publishToRemove.Dequeue();
					Assert.True(publishPool.items[toFree].disabled_removing);
					publishPool.Free(toFree);
				}
				while(subscribeToRemove.Count > 0)
				{
					var toFree = subscribeToRemove.Dequeue();
					Assert.True(subscribePool.items[toFree].disabled_removing);
					subscribePool.Free(toFree);
				}
				
				// enable new
				while(publishToAdd.Count > 0)
				{
					var publish = publishToAdd.Dequeue();
					if(!publishPool.items[publish.arg4.slotIndex].reserved)
						continue;

					publishPool.Enable(ref publish);
					Assert.True(publishPool.items[publish.arg4.slotIndex].enabled_added);
				}
				while(subscribeToAdd.Count > 0)
				{
					var subscribe = subscribeToAdd.Dequeue();
					if(!subscribePool.items[subscribe.arg4.slotIndex].reserved)
						continue;
					subscribePool.Enable(ref subscribe);
					Assert.True(subscribePool.items[subscribe.arg4.slotIndex].enabled_added);
				}
			}
		}
		
		public struct SiegeRef
		{
			public int groupId;
			public int slotIndex;
			public int entityId;
			
			public SiegeRef(int groupId, int slotIndex, int entityId)
			{
				this.groupId = groupId;
				this.slotIndex = slotIndex;
				this.entityId = entityId;
			}
		}
			
		#region Publish & Subscribe Meta
		struct SiegePublish
		{
			public bool reserved;
			public bool enabled_added; // will be true when dequeued from adding
			public bool disabled_removing; // will be true when enqueued to free
			public int frameCount; // number of frames passed after this publish is being registered, zero mean first frame
			public Siege sender;
			public object message;
			public SiegePublishOption publishOption;
			public int publishId;
		}
		
		struct SiegeSubscribe
		{
			public bool reserved;
			public bool enabled_added; // will be true when dequeued from adding
			public bool disabled_removing; // will be true when enqueued to free
			public int frameCount; // number of frames passed after this subscription is being registered, zero mean first frame
			public Siege sender;
			public Action<Siege, object> callback;
			public int callCount;
			public SiegeSubscribeOption subscribeOption;
			public int subscribeId;
		}
		
		class SiegePublishPool : RecycleArray<SiegePublish>
		{
			int uniquePublishId;

			public SiegeRef Reserve(Siege sender, int groupId)
			{
				var publishId = uniquePublishId++;
				var slotIndex = base.Alloc();
				base.items[slotIndex].reserved = true;
				base.items[slotIndex].sender = sender;
				base.items[slotIndex].publishId = publishId;
				return new SiegeRef(groupId, slotIndex, publishId);
			}
			
			public void Enable(ref Args<Siege, object, SiegePublishOption, SiegeRef> reservedEntity)
			{
				base.items[reservedEntity.arg4.slotIndex] = new SiegePublish()
				{
					enabled_added = true,
					sender = reservedEntity.arg1,
					message = reservedEntity.arg2,
					publishOption = reservedEntity.arg3,
					publishId = reservedEntity.arg4.entityId,
				};
			}
		}
		
		class SiegeListenPool : RecycleArray<SiegeSubscribe>
		{
			int uniqueSubscribeId;

			public SiegeRef Reserve(Siege sender, int groupId)
			{
				var subscribeId = uniqueSubscribeId++;
				var slotIndex = base.Alloc();
				base.items[slotIndex].reserved = true;
				base.items[slotIndex].sender = sender;
				base.items[slotIndex].subscribeId = subscribeId;
				return new SiegeRef(groupId, slotIndex, subscribeId);
			}
			
			public void Enable(ref Args<Siege, Action<Siege, object>, SiegeSubscribeOption, SiegeRef> reservedEntity)
			{
				base.items[reservedEntity.arg4.slotIndex] = new SiegeSubscribe()
				{
					enabled_added = true,
					sender = reservedEntity.arg1,
					callback = reservedEntity.arg2,
					subscribeOption = reservedEntity.arg3,
					subscribeId = reservedEntity.arg4.entityId,
				};
			}
		}
		#endregion

		#endregion
	}
}