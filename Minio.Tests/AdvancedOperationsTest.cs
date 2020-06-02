using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;

namespace Minio.Tests
{
    [TestClass]
    public class AdvancedOperationsTest
    {
        private DateTime _requestDate = new DateTime(2020, 05, 01, 15, 45, 33, DateTimeKind.Utc);

        private static bool IsLocationRequest(IRestRequest rr)
        {
            return rr.Resource.Contains("?") == false &&
                   rr.Parameters.Contains(new Parameter("location", "", ParameterType.QueryString));
        }

        private static Mock<IRestClient> MockRestClient(Uri initialBaseUrl)
        {
            string location = "mock-location";
            Uri baseUrl = initialBaseUrl; // captured state
            var restClient = new Mock<IRestClient>(MockBehavior.Strict);
            restClient.SetupSet(rc => rc.BaseUrl = It.IsAny<Uri>()).Callback((Uri value) => baseUrl = value);
            restClient.SetupGet(rc => rc.BaseUrl).Returns(() => baseUrl);
            restClient.Setup(rc =>
                    rc.ExecuteTaskAsync(It.Is((IRestRequest rr) => IsLocationRequest(rr)), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IRestRequest rr, CancellationToken ct) => new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = $"<?xml version=\"1.0\" encoding=\"UTF-8\"?><GetBucketLocationOutput><LocationConstraint>{location}</LocationConstraint></GetBucketLocationOutput>"
                });
            restClient.Setup(rc => rc.BuildUri(It.IsAny<IRestRequest>()))
                .Returns((IRestRequest rr) => new RestClient {BaseUrl = baseUrl }.UseUrlEncoder(HttpUtility.UrlEncode).BuildUri(rr));
            restClient.SetupProperty(rc => rc.Authenticator);
            return restClient;
        }

        [TestMethod]
        public async Task PresignedUploadPart()
        {
            var client = new MinioClient(endpoint:"localhost:9001", "my-access-key", "my-secret-key");

            Mock<IRestClient> restClient = MockRestClient(client.restClient.BaseUrl);
            client.restClient = restClient.Object;

            var signedUrl = await client.Advanced.PresignedUploadPartAsync("bucket", "object-name", "upload-id", 1, 3600, _requestDate);

            Assert.AreEqual(
                "http://localhost:9001/bucket/object-name?uploadId=upload-id&partNumber=1&X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=my-access-key%2F20200501%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20200501T154533Z&X-Amz-Expires=3600&X-Amz-SignedHeaders=host&X-Amz-Signature=e0a9693f6c8cd893d75fda3c2e6df8cb8f2899376cf953b49f43471a8d3c87db",
                signedUrl);
        }

        [TestMethod]
        public async Task PresignedPutObject()
        {
            var client = new MinioClient(endpoint:"localhost:9001", "my-access-key", "my-secret-key");

            Mock<IRestClient> restClient = MockRestClient(client.restClient.BaseUrl);
            client.restClient = restClient.Object;

            var headerMap = new Dictionary<string, string> {{"X-Special", "special"}, {"Content-Language", "en"}};
            var signedUrl = await client.Advanced.PresignedPutObjectAsync("bucket", "object-name", headerMap, "text/xml", 3600, _requestDate);

            Assert.AreEqual(
                "http://localhost:9001/bucket/object-name?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=my-access-key%2F20200501%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20200501T154533Z&X-Amz-Expires=3600&X-Amz-SignedHeaders=host&content-language=en&x-special=special&X-Amz-Signature=c3c22200130e875228c967e85dbf9f892c15ec2d65486e0ed57beab42ff1b8de",
                signedUrl);
        }
    }
}