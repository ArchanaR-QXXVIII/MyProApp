using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


[ApiController]
[Route("[controller]")]
public class ProcessController : ControllerBase
{
	private readonly BatchProcessorProcess _batchProcessor;

	public ProcessController(BatchProcessorProcess batchProcessor)
	{
		_batchProcessor = batchProcessor;
	}

	[HttpPost("postprocess")]
	public async Task<ActionResult<Result>> PostProcess([FromBody] DataObject data)
	{
		var result = await _batchProcessor.SubmitDataAsync(data);
		return Ok(result);
	}
}
