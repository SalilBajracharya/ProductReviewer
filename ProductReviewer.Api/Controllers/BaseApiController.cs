﻿using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ProductReviewer.Api.Controllers
{
    public class BaseApiController : ControllerBase
    {
        private IMediator? _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
    }
}
