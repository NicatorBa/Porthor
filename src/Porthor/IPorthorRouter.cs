﻿using Microsoft.AspNetCore.Routing;
using Porthor.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Porthor
{
    public interface IPorthorRouter : IRouter
    {
        Task Initialize(IEnumerable<Resource> resources);
    }
}