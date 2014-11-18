using System.Threading;
using Tools;

namespace EventProcessingService {

	/// <summary>
	/// Puts items on the <see cref="Queue{T}"/>.
	/// </summary>
	public class Putter {

		private readonly Queue<ItemToPut> _queue;
		private readonly int _numberOfMessages;

		public Putter(Queue<ItemToPut> queue, int numberOfMessages) {
			this._numberOfMessages = numberOfMessages;
			this._queue = queue;
		}

		public void Run() {
			Logger.Info(() => "Running");
			for (int i = 1; i <= this._numberOfMessages; i++) {
				try {
					ItemToPut item = new ItemToPut { Id = i };
					this._queue.Enqueue(item);
					Logger.Debug(this, () => "Published message " + item.Id);
				} catch (ThreadAbortException) {
					Logger.Warn(() => "Aborted");
				}
			}
		}

	}

}