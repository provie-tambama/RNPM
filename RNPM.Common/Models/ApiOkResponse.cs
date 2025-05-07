namespace RNPM.Common.Models;

public class ApiOkResponse: ApiResponse
{
    public object Result { get; }
    public ApiOkResponse(object result, string message = null)
        : base(200, message)
    {
        Result = result;
    }
}