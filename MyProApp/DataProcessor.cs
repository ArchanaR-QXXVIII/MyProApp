using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class DataProcessor
{
	private static readonly Lazy<DataProcessor> _instance = new(() => new DataProcessor());
	public static DataProcessor Instance => _instance.Value;

	private DataProcessor() { }

	public Result[] ProcessData(DataObject[] data)
	{
		Thread.Sleep(1000); // Simulate GPU work

		return data.Select(d => new Result { ProcessedValue = d.Value * 2 }).ToArray();
	}
}
