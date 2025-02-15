﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GG.Portafolio.Site.Test
{
    internal class NetworkHandler : HttpMessageHandler
    {
        private enum Behavior
        {
            Throw,
            ReturnError,
            ReturnDocument
        }

        private readonly Exception _exception;
        private readonly Behavior _behavior;
        private readonly HttpStatusCode _statusCode;
        private readonly string _reason;
        private readonly string _document;
        private readonly Func<HttpRequestMessage, string> _selector;
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _action;

        public HttpRequestMessage Request { get; private set; }
        public string Body { get; private set; }

        public NetworkHandler(Exception exception)
        {
            _exception = exception;
            _behavior = Behavior.Throw;
        }

        public NetworkHandler(HttpStatusCode statusCode, string reason)
        {
            _statusCode = statusCode;
            _reason = reason;

            _behavior = Behavior.ReturnError;
        }

        public NetworkHandler(string document, HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
            _document = document;
            _behavior = Behavior.ReturnDocument;
        }

        public NetworkHandler(Func<HttpRequestMessage, string> documentSelector, HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
            _selector = documentSelector;
            _behavior = Behavior.ReturnDocument;
        }

        public NetworkHandler(Func<HttpRequestMessage, HttpResponseMessage> action)
        {
            _action = action;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            Body = await SafeReadContentFrom(request);

            if (_action != null)
            {
                return _action(request);
            }

            if (_behavior == Behavior.Throw) throw _exception;

            var response = new HttpResponseMessage(_statusCode);

            if (_behavior == Behavior.ReturnError)
            {
                response.ReasonPhrase = _reason;
            }

            if (_behavior == Behavior.ReturnDocument)
            {
                if (_selector != null)
                {
                    response.Content = new StringContent(_selector(request));
                }
                else
                {
                    response.Content = new StringContent(_document);
                }
            }

            return response;
        }

        private async Task<string> SafeReadContentFrom(HttpRequestMessage request)
        {
            if (request.Content == null) return null;

            return await request.Content.ReadAsStringAsync();
        }
    }
}
