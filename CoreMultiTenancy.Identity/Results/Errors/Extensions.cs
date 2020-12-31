using Microsoft.AspNetCore.Mvc;

namespace CoreMultiTenancy.Identity.Results.Errors
{
    public static class Extensions
    {
        /// <summary>
        /// Transforms an Error enum into an object result with an end-user formatted message. 
        /// If Error is <typeparamref name="Unspecified"/> then returns a 
        /// <typeparamref name="BadRequestObjectResult"/> with an empty string. 
        /// </summary>
        /// <param name="description">An end-user formatted message describing the error.</param>
        public static ObjectResult ToObjectResult(this Error e)
        {
            switch (e.ErrorType)
            {
                case ErrorType.NotFound : return new NotFoundObjectResult(e.Description);
                case ErrorType.DomainLogic :
                case ErrorType.BadRequest :
                case ErrorType.KeyExists : return new BadRequestObjectResult(e.Description);
                default : return new BadRequestObjectResult(string.Empty);
            }
        }
    }
}
