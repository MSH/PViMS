using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PVIMS.Web.HttpResults
{
    public class ValidationErrorsResult : IHttpActionResult
    {
        private ValidationError[] validationErrors = new ValidationError[0];

        public ValidationErrorsResult(ValidationError[] validationErrors)
        {
            this.validationErrors = validationErrors;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var content = JsonConvert.SerializeObject(validationErrors);
            response.Content = new StringContent(content, Encoding.UTF8, "application/json");
            return Task.FromResult(response);
        }
    }

    public class ValidationError
    {
        public ValidationError(string id, string message)
        {
            Id = id;
            Message = message;
        }
        public string Id { get; private set; }
        public string Message { get; private set; }
    }
}