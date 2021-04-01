using DevIO.Api.Controllers;
using DevIO.Business.Intefaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DevIO.Api.V1.Controllers
{
    [ApiVersion("2.0")]
    [ApiVersion("1.0"), Obsolete()]
    [Route("api/v{version:apiVersion}/version")]
    public class V2Controller : MainController
    {
        public V2Controller(INotificador notificador) : base(notificador)
        {
        }

        [HttpGet]
        public string Get()
        {
            return "V1";
        }
    }
}
