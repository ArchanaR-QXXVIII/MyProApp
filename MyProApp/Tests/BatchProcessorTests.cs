using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

public class BatchProcessorTests
{
	[Fact]
	public async Task SubmitDataAsync_ReturnsResultWithinTwoSeconds()
	{
		var processor = new BatchProcessorProcess();

		var tasks = new List<Task<Result>>();

		var sw = Stopwatch.StartNew();

		for (int i = 0; i < 4; i++)
		{
			var data = new DataObject { Value = i + 1 };
			tasks.Add(processor.SubmitDataAsync(data));
		}

		var results = await Task.WhenAll(tasks);
		//Console.WriteLine(results.ToString());
		sw.Stop();

		Assert.True(sw.Elapsed.TotalSeconds <= 2.0 + 0.5); // Allow buffer for test delay
		for (int i = 0; i < 4; i++)
		{
			Assert.Equal((i + 1) * 2, results[i].ProcessedValue);
		}
	}
}