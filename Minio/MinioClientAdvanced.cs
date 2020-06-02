/*
 * MinIO .NET Library for Amazon S3 Compatible Cloud Storage, (C) 2017 MinIO, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Minio.DataModel;

namespace Minio
{
    public class MinioClientAdvanced
    {
        private readonly MinioClient client;

        internal MinioClientAdvanced(MinioClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Start a new multi-part upload request
        /// </summary>
        /// <param name="bucketName">Bucket Name</param>
        /// <param name="objectName">Object Name</param>
        /// <param name="metaData"></param>
        /// <param name="sseHeaders"> Server-side encryption options</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation</param>
        /// <returns>Upload Id</returns>
        public Task<string> NewMultipartUploadAsync(
            string bucketName, string objectName, Dictionary<string, string> metaData, Dictionary<string, string> sseHeaders = null, CancellationToken cancellationToken = default(CancellationToken))
            => this.client.NewMultipartUploadAsync(bucketName, objectName, metaData, sseHeaders ?? new Dictionary<string, string>(), cancellationToken);

        /// <summary>
        /// Presigned Put Part url -returns a presigned url to upload an object part without credentials.URL can have a maximum expiry of
        /// upto 7 days or a minimum of 1 second.
        /// </summary>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Key of object to retrieve</param>
        /// <param name="partNumber"></param>
        /// <param name="expiresInt">Expiration time in seconds</param>
        /// <param name="uploadId"></param>
        /// <param name="reqDate"> Optional request date and time in UTC</param>
        /// <returns></returns>
        public Task<string> PresignedUploadPartAsync(string bucketName, string objectName, string uploadId, int partNumber, int expiresInt, DateTime? reqDate = null)
            => this.client.PresignedUploadPartAsync(bucketName, objectName, uploadId, partNumber, expiresInt, reqDate);

        /// <summary>
        /// Returns an async observable of parts corresponding to a uploadId for a specific bucket and objectName
        /// </summary>
        /// <param name="bucketName">Bucket Name</param>
        /// <param name="objectName">Object Name</param>
        /// <param name="uploadId"></param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation</param>
        /// <returns>async observable of parts</returns>
        public IObservable<Part> ListParts(
            string bucketName, string objectName, string uploadId, CancellationToken cancellationToken = default(CancellationToken))
            => this.client.ListParts(bucketName, objectName, uploadId, cancellationToken);

        /// <summary>
        /// Remove object with matching uploadId from bucket
        /// </summary>
        /// <param name="bucketName">Bucket Name</param>
        /// <param name="objectName"></param>
        /// <param name="uploadId"></param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation</param>
        /// <returns></returns>
        public Task RemoveUploadAsync(string bucketName, string objectName, string uploadId, CancellationToken cancellationToken)
            => this.client.RemoveUploadAsync(bucketName, objectName, uploadId, cancellationToken);

        /// <summary>
        /// Internal method to complete multi part upload of object to server.
        /// </summary>
        /// <param name="bucketName">Bucket Name</param>
        /// <param name="objectName">Object to be uploaded</param>
        /// <param name="uploadId">Upload Id</param>
        /// <param name="etags">Etags</param>
        /// <param name="cancellationToken">Optional cancellation token to cancel the operation</param>
        public Task CompleteMultipartUploadAsync(
            string bucketName, string objectName, string uploadId, Dictionary<int, string> etags, CancellationToken cancellationToken = default(CancellationToken))
            => this.client.CompleteMultipartUploadAsync(bucketName, objectName, uploadId, etags, cancellationToken);

        /// <summary>
        /// Presigned Put url -returns a presigned url to upload an object without credentials. URL can have a maximum expiry of
        /// up to 7 days or a minimum of 1 second.
        /// </summary>
        /// <param name="bucketName">Bucket to retrieve object from</param>
        /// <param name="objectName">Key of object to retrieve</param>
        /// <param name="headerMap">headerMap. can be <c>null</c>.</param>
        /// <param name="contentType">Content Type. will be <c>application/octet-stream</c> if <c>null</c>.</param>
        /// <param name="expiresInt">Expiration time in seconds</param>
        /// <param name="reqDate"> Optional request date and time in UTC</param>
        /// <returns></returns>
        public Task<string> PresignedPutObjectAsync(string bucketName, string objectName, Dictionary<string, string> headerMap, string contentType, int expiresInt, DateTime? reqDate = null)
            => this.client.PresignedPutObjectAsync(bucketName, objectName, headerMap, contentType, expiresInt, reqDate);
    }
}