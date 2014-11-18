using System;
using Tools;

namespace EventProcessingService {

	/// <summary>
	/// Maintains a classic queue, thread safe, bounded capacity
	/// </summary>
	public class Queue<T> {

		private readonly System.Collections.Generic.Queue<T> _queue = new System.Collections.Generic.Queue<T>();
		private readonly object _lock = new object();
		private readonly string _name;
		private readonly int _size;

		public Queue(int size, string name) {
			this._size = size;
			_name = name;
		}

		public string Name {
			get { return (this._name); }
		}

		public void Enqueue(T item) {
			while (true) {
				lock (_lock) {
					if (this._queue.Count <= this._size) {
						this._queue.Enqueue(item);
						break;
					}
					Logger.Debug(this, "() => Queue is full, it contains " + this._queue.Count + " item(s)");
				}
			}
		}

		/// <summary>
		/// blocking call
		/// </summary>
		/// <returns></returns>
		public T Dequeue() {
			while (true) {
				try {
					lock (_lock) {
						T item = this._queue.Dequeue();
						return (item);
					}
				} catch (InvalidOperationException) {
					// queue is empty
				}
			}
		}

		public override string ToString() {
			return ("Queue " + Name);
		}

	}

}
