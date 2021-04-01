using DevIO.Api.Controllers;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.V2.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/version")]
    public class V2Controller : MainController
    {
        public V2Controller(INotificador notificador) : base(notificador)
        {
        }

        [HttpGet]
        public string Get()
        {
            return "V2";
        }
    }
}
