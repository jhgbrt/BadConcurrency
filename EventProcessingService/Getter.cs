using System;
using System.Collections.Generic;
using System.Linq;
	using Tools;

namespace EventProcessingService {

	/// <summary>
	/// Fetches items from the <see cref="Queue{T}"/>
	/// </summary>
	public class Getter {

		private readonly Queue<ItemToPut> _queue;
		private readonly List<int> _eventsReceived = new List<int>();
		private int _lastEventId;

		public Getter(Queue<ItemToPut> queue) {
			this._queue = queue;
		}

		public event OnItemHandledHandler OnItemHandled;

		public void Run() {
			Logger.Info(() => "Running");
			while (true) {
				try {
					Logger.Debug(this, "Waiting for item...");
					ItemToPut item = this._queue.Dequeue();
					this.HandleItem(item);
					if (this.OnItemHandled != null) this.OnItemHandled(item);
					Logger.Debug(this, () => "Handled item " + item + " from ringbuffer");
					// Thread.Sleep(TimeSpan.FromMilliseconds(500));
				} catch (Exception ex) {
					Logger.Error(ex);
					break;
				}
			}
		}

		private void HandleItem(ItemToPut item) {
			if (item.Id <= 0) throw new Exception("event " + item.Id + " does not have a valid id");
			if (_eventsReceived.Any(id => id == item.Id)) throw new Exception("event " + item.Id + " was handled already!");
			int expectedId = _lastEventId + 1;
			if (expectedId != item.Id) throw new Exception("event " + item.Id + " is not in the right order, expected event " + (expectedId));
			this._eventsReceived.Add(item.Id);
			this._lastEventId = item.Id;
		}

	}

}