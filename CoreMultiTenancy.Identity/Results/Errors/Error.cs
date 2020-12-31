namespace CoreMultiTenancy.Identity.Results.Errors
{
    public class Error
    {
        public Error(string desc, ErrorType e)
        {
            Description = desc;
            ErrorType = e;
        }
        public ErrorType ErrorType { get; private set; }
        public string Description { get; private set; }
    }
}