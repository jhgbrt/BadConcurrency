using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace EventProcessingService {

	public class Program {

		private static readonly CancellationTokenSource cancellation = new CancellationTokenSource();
		private static readonly ManualResetEvent eventsHandled = new ManualResetEvent(false);

		private static Queue<ItemToPut> queue;
		private static Putter putter;
		private static Getter getter;

		private const int NUMBER_OF_MESSAGES = 1000; //1024*100;

		public static void Main() {

			Logger.Appenders.Add(new ConsoleAppender());
			Logger.Level = Level.Info;
			Logger.Info(() => "Started");

			try {

				queue = new Queue<ItemToPut>(16, "The Queue");
				putter = new Putter(queue, NUMBER_OF_MESSAGES);
				getter = new Getter(queue);
				getter.OnItemHandled += OnEventHandled;
				
				Stopwatch sw = Stopwatch.StartNew();

				Task.Factory.StartNew(putter.Run, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
				Task.Factory.StartNew(getter.Run, cancellation.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

				Logger.Info("Started, waiting for events to be handled...");
				eventsHandled.WaitOne();
				Logger.Info("Events handled!");

				long ms = sw.ElapsedMilliseconds;
				double perMessage = Math.Round((double)ms / NUMBER_OF_MESSAGES, 3);
				Logger.Info(() => "Handled " + NUMBER_OF_MESSAGES + " event(s) in " + ms + "ms (" + perMessage + " ms/event, " + NUMBER_OF_MESSAGES * 1000 / ms + " events/sec)");

			} catch (Exception ex) {
				Logger.Error(ex);
			}

			Console.WriteLine("Press <ENTER> to exit...");
			Console.ReadLine();

		}

		private static void OnEventHandled(ItemToPut item) {
			Logger.Debug("Item nr " + item.Id + " reported handled");
			if (item.Id == NUMBER_OF_MESSAGES) {
				Logger.Info("All events have been handled");
				Task.Factory.StartNew(Stop);
			}
		}

		private static void Stop() {
			Logger.Info("Stopping...");
			cancellation.Cancel();
			Logger.Debug(() => "Stopped");
			eventsHandled.Set();
		}

	}

}

