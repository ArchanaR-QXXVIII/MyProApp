using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;


	public class BatchProcessorProcess : IDisposable
{
		private class PendingItem
		{
			public DataObject Data { get; set; }
			public TaskCompletionSource<Result> Tcs { get; set; }
		}

		private readonly ConcurrentQueue<PendingItem> _queue = new();
		private readonly SemaphoreSlim _processingLock = new(1, 1);
		private readonly object _batchLock = new();
		private readonly List<PendingItem> _currentBatch = new();
		private Timer _timer;
		private readonly TimeSpan _timeout = TimeSpan.FromSeconds(2);

		public BatchProcessorProcess()
		{
			_timer = new Timer(OnTimerElapsed, null, Timeout.Infinite, Timeout.Infinite);
		}

		public Task<Result> SubmitDataAsync(DataObject data)
		{
			var item = new PendingItem
			{
				Data = data,
				Tcs = new TaskCompletionSource<Result>(TaskCreationOptions.RunContinuationsAsynchronously)
			};

			lock (_batchLock)
			{
				_currentBatch.Add(item);

				if (_currentBatch.Count == 1)
				{
					_timer.Change(_timeout, Timeout.InfiniteTimeSpan); // Start timer
				}

				if (_currentBatch.Count == 4)
				{
					_timer.Change(Timeout.Infinite, Timeout.Infinite); // Cancel timer
					_ = ProcessBatchAsync(); // Fire and forget
				}
			}

			return item.Tcs.Task;
		}

		private async void OnTimerElapsed(object state)
		{
			await ProcessBatchAsync();
		}

		private async Task ProcessBatchAsync()
		{
			List<PendingItem> batch;

			lock (_batchLock)
			{
				if (_currentBatch.Count == 0) return;

				batch = new List<PendingItem>(_currentBatch);
				_currentBatch.Clear();
			}

			await _processingLock.WaitAsync(); // Ensure only one execution at a time

			try
			{

				var processor = DataProcessor.Instance; // Singleton
				var results = processor.ProcessData(batch.Select(x => x.Data).ToArray());

				for (int i = 0; i < batch.Count; i++)
				{
					batch[i].Tcs.SetResult(results[i]);
				}
			}
			catch (Exception ex)
			{
				foreach (var item in batch)
				{
					item.Tcs.SetException(ex);
				}
			}
			finally
			{
				_processingLock.Release();
			}
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
