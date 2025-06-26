public abstract class BaseResponse
{
    public bool Success { get; set; }
    
    public List<string> ErrorMessages { get; set; }
}