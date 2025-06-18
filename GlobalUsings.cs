// Common system namespaces
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// Common Microsoft extensions
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Options;

// For API project specifically
#if API_PROJECT
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Identity.Web;
global using AutoMapper;
#endif

// For test projects specifically
#if TEST_PROJECT
global using Xunit;
global using Moq;
#endif

// GlassLewis domain-specific namespaces
global using GlassLewis.Domain.Entities;
global using GlassLewis.Domain.Interfaces;
global using GlassLewis.Domain.Models;