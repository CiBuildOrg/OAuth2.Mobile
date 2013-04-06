﻿namespace StudioDonder.OAuth2.Mobile.IntegrationTests
{
    using System;
    using System.Threading;

    using RestSharp;

    using Xunit;

    public class RestClientExtensionsTests
    {
        [Fact]
        public void ExecuteAsyncOnSuccessfulRequestReturnsRestResponseWithContentSet()
        {
            // Arrange
            var restClient = CreateRestClient();

            // Act
            var executeAsyncTask = restClient.ExecuteAsync(CreateRestRequest(), CancellationToken.None);
            executeAsyncTask.Wait();

            // Assert
            Assert.Equal("{\n   \"one\": \"two\",\n   \"key\": \"value\"\n}\n", executeAsyncTask.Result.Content);
        }

        [Fact]
        public void ExecuteAsyncCorrectlyHandlesCancellation()
        {
            // Arrange
            var restClient = CreateRestClient();
            var cancellationTokenSource = new CancellationTokenSource();

            // Act
            var executeAsyncTask = restClient.ExecuteAsync(CreateRestRequest(), cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();

            // Assert
            Assert.True(executeAsyncTask.IsCanceled);
        }

        [Fact]
        public void ExecuteAsyncCorrectlyHandlesErrors()
        {
            // Arrange
            var restClient = CreateInvalidRestClient();

            // Act
            var executeAsyncTask = restClient.ExecuteAsync(CreateRestRequest(), CancellationToken.None);
            executeAsyncTask.ContinueWith(t => t.Result);

            // Assert
            Assert.True(executeAsyncTask.IsFaulted);
        }

        [Fact]
        public void ExecuteAsyncGenericOnSuccessfulRequestReturnsCorrectlyDeserializedResponse()
        {
            // Arrange
            var restClient = CreateRestClient();

            // Act
            var executeAsyncTask = restClient.ExecuteAsync<JsonData>(CreateRestRequest(), CancellationToken.None);
            executeAsyncTask.Wait();

            // Assert
            Assert.Equal("value", executeAsyncTask.Result.Key);
        }

        [Fact]
        public void ExecuteAsyncGenericCorrectlyHandlesCancellation()
        {
            // Arrange
            var restClient = CreateRestClient();
            var cancellationTokenSource = new CancellationTokenSource();

            // Act
            var executeAsyncTask = restClient.ExecuteAsync<JsonData>(CreateRestRequest(), cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();

            // Assert
            Assert.True(executeAsyncTask.IsCanceled);
        }

        private static RestClient CreateInvalidRestClient()
        {
            return new RestClient("http://xxxxxxxxxxxxxxxxxxxx.xx");
        }

        private static RestClient CreateRestClient()
        {
            return new RestClient("http://echo.jsontest.com/key/value/one/two");
        }

        private static RestRequest CreateRestRequest()
        {
            return new RestRequest(new Uri("/", UriKind.Relative));
        }

        private class JsonData
        {
            public string Key { get; set; }
        }
    }
}