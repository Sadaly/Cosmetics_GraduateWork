using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Net;
using System.Text;
using WebApi.Middleware;

namespace WebApi.UnitTests.Middleware
{
	public class ExceptionHandlingMiddlewareTests
	{
		private readonly ExceptionHandlingMiddleware _middleware;
		private readonly RequestDelegate _next;
		private readonly HttpContext _context;

		public ExceptionHandlingMiddlewareTests()
		{
			_next = Substitute.For<RequestDelegate>();
			_middleware = new ExceptionHandlingMiddleware(_next);
			_context = new DefaultHttpContext();
			_context.Response.Body = new MemoryStream();
		}

		[Fact]
		public async Task InvokeAsync_ShouldPassThrough_WhenNoException()
		{
			// Arrange
			_next.Invoke(Arg.Any<HttpContext>()).Returns(Task.CompletedTask);

			// Act
			await _middleware.InvokeAsync(_context);

			// Assert
			await _next.Received(1).Invoke(Arg.Any<HttpContext>());
			_context.Response.StatusCode.Should().Be(200);
		}

		[Fact]
		public async Task InvokeAsync_ShouldHandleNullReferenceException()
		{
			// Arrange
			var exception = new NullReferenceException();
			_next.Invoke(Arg.Any<HttpContext>()).Throws(exception);

			// Act
			await _middleware.InvokeAsync(_context);

			// Assert
			_context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
			_context.Response.ContentType.Should().Be("application/json");

			var responseBody = await GetResponseBody();
			responseBody.Should().Contain("NullReference");
			responseBody.Should().Contain("Object reference not set");
		}

		[Fact]
		public async Task InvokeAsync_ShouldHandleArgumentException()
		{
			// Arrange
			var message = "Invalid argument provided";
			var exception = new ArgumentException(message);
			_next.Invoke(Arg.Any<HttpContext>()).Throws(exception);

			// Act
			await _middleware.InvokeAsync(_context);

			// Assert
			_context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
			_context.Response.ContentType.Should().Be("application/json");

			var responseBody = await GetResponseBody();
			responseBody.Should().Contain("InvalidArgument");
			responseBody.Should().Contain(message);
		}

		[Fact]
		public async Task InvokeAsync_ShouldHandleInvalidOperationException()
		{
			// Arrange
			var message = "Invalid operation attempted";
			var exception = new InvalidOperationException(message);
			_next.Invoke(Arg.Any<HttpContext>()).Throws(exception);

			// Act
			await _middleware.InvokeAsync(_context);

			// Assert
			_context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
			_context.Response.ContentType.Should().Be("application/json");

			var responseBody = await GetResponseBody();
			responseBody.Should().Contain("InvalidOperation");
			responseBody.Should().Contain(message);
		}

		[Fact]
		public async Task InvokeAsync_ShouldHandleGenericException()
		{
			// Arrange
			var message = "Something went wrong";
			var exception = new Exception(message);
			_next.Invoke(Arg.Any<HttpContext>()).Throws(exception);

			// Act
			await _middleware.InvokeAsync(_context);

			// Assert
			_context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
			_context.Response.ContentType.Should().Be("application/json");

			var responseBody = await GetResponseBody();
			responseBody.Should().Contain("InvalidException");
			responseBody.Should().Contain(message);
		}

		private async Task<string> GetResponseBody()
		{
			_context.Response.Body.Seek(0, SeekOrigin.Begin);
			using var reader = new StreamReader(_context.Response.Body, Encoding.UTF8);
			return await reader.ReadToEndAsync();
		}
	}
}