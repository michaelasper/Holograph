// /********************************************************
// *                                                       *
// *   Copyright (C) Microsoft. All rights reserved.       *
// *                                                       *
// ********************************************************/

namespace Microsoft.MSRC.Portal.Libraries.AzureDocDb
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Azure.Documents;
    using Azure.Documents.Client;
    using CoreUtils;
    using DTO;

    public class DocDbAccessor
    {
        private const string DbName = "EngageDb";
        private readonly DocumentClient client;

        public DocDbAccessor(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            string url = config.GetAppSetting("DocDbUrl");
            string key = config.GetAppSetting("DocDbKey");
            string endpointUrl = config.GetSecret(url);
            string primaryKey = config.GetSecret(key);

            client = new DocumentClient(new Uri(endpointUrl), primaryKey);
        }

        public async void WriteNewEngagement(EngageDocDbWrapper engagement)
        {
            if (engagement == null)
            {
                throw new ArgumentNullException("engagement");
            }

            engagement.SubmissionDateUtc = DateTime.UtcNow;

            if (engagement.Engagement.KeyValuePairs.All(x => x.Key != "PortalUserIdentity"))
            {
                engagement.SubmitterFederatedName = engagement.Engagement.KeyValuePairs.FirstOrDefault(x => x.Key == "ReporterEmailAddress").Value;
            }

            if (string.IsNullOrEmpty(engagement.SubmitterFederatedName))
            {
                engagement.SubmitterFederatedName = "anonymous";
            }

            if (string.IsNullOrEmpty(engagement.Id))
            {
                engagement.Id = Guid.NewGuid().ToString();
                engagement.EngagementSubmissionStatus = EngagementSubmissionStatus.Unsubmitted;
            }
            else
            {
                engagement.EngagementSubmissionStatus = EngagementSubmissionStatus.Submitted;
            }

            engagement.EngagementStatus = EngagementStatus.Open;

            await InitDb();

            await InitCollection("Engagements");

            await CreateEngagementDocumentIfNotExists(engagement);
        }

        public IQueryable<EngageDocDbWrapper> GetUnsentEngagements(string databaseName, string collectionName)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentNullException("databaseName");
            }

            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentNullException("collectionName");
            }

            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                MaxItemCount = 20
            };

            // Here we find the Andersen family via its LastName
            var unsentQuery = client.CreateDocumentQuery<EngageDocDbWrapper>(
                UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                .Where(f => f.EngagementSubmissionStatus == EngagementSubmissionStatus.Unsubmitted);

            return unsentQuery;
        }

        public IEnumerable<EngagePublicDto> GetEngagementsForUser(string submitterName, string pageNumber = null)
        {
            int page = 0;

            if (string.IsNullOrWhiteSpace(submitterName))
            {
                throw new ArgumentNullException("submitterName");
            }

            if (!string.IsNullOrWhiteSpace(pageNumber))
            {
                int.TryParse(pageNumber, out page);
            }

            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                MaxItemCount = -1
            };

            // Here we find the Andersen family via its LastName
            var unsentQuery = client.CreateDocumentQuery<EngageDocDbWrapper>(
                UriFactory.CreateDocumentCollectionUri(DbName, "Engagements"), queryOptions)
                .Where(f => f.SubmitterFederatedName == submitterName)
                .OrderByDescending(x => x.SubmissionDateUtc);            

            var answer = new List<EngagePublicDto>();
            foreach (EngageDocDbWrapper wrapper in unsentQuery) //.Skip(page * 40).Take(40)
            {
                try
                {
                    answer.Add(ConvertPrivateToPublicWrapper(wrapper));
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return answer.Skip(page * 20).Take(20);
        }

        private EngagePublicDto ConvertPrivateToPublicWrapper(EngageDocDbWrapper privateWrapper)
        {
            if (privateWrapper == null)
            {
                throw new ArgumentNullException("privateWrapper");
            }

            EngagePublicDto publicWrapper = new EngagePublicDto();

            if (privateWrapper.EngagementType.ToLower().Contains("cars"))
            {
                publicWrapper.SubmissionType = "CERT";
            }
            else if (privateWrapper.EngagementType.ToLower().Contains("pentest"))
            {
                publicWrapper.SubmissionType = "Pentest";
            }
            else if (privateWrapper.EngagementType.ToLower().Contains("ssi"))
            {
                publicWrapper.SubmissionType = "Service Security";
            }

            publicWrapper.EngagementStatus = privateWrapper.EngagementStatus.ToString();

            if (privateWrapper.EngagementStatus == EngagementStatus.Open)
            {
                publicWrapper.ClosureDate = "N/A";
            }
            else
            {
                publicWrapper.ClosureDate = privateWrapper.SubmissionClosedUtc.ToString("yyyy-MM-dd HH:mm");
            }
            publicWrapper.SubmissionDate = privateWrapper.SubmissionDateUtc.ToString("yyyy-MM-dd HH:mm");

            publicWrapper.IncidentGuid = privateWrapper.Id;

            return publicWrapper;
        }

        public IQueryable GetUnclosedEngagements(string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentNullException("collectionName");
            }

            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                MaxItemCount = -1
            };

            // Here we find the Open Items
            var unclosedQuery = client.CreateDocumentQuery<EngageDocDbWrapper>(
                UriFactory.CreateDocumentCollectionUri(DbName, collectionName), queryOptions)
                .Where(f => f.EngagementStatus == EngagementStatus.Open);

            return unclosedQuery;
        }

        public async void ConvertUnsubmittedToSubmitted(EngageDocDbWrapper engagement, string automationCallbackId)
        {
            if (engagement == null)
            {
                throw new ArgumentNullException("engagement");
            }

            if (string.IsNullOrWhiteSpace(automationCallbackId))
            {
                throw new ArgumentNullException("automationCallbackId");
            }

            string oldGuid = engagement.Id;

            engagement.Id = automationCallbackId;
            engagement.EngagementSubmissionStatus = EngagementSubmissionStatus.Submitted;

            await InitDb();

            await InitCollection(engagement.EngagementType);

            await CreateEngagementDocumentIfNotExists(engagement);

            await DeleteEngagemntDocument(engagement.EngagementType, oldGuid);
        }

        public async void ConvertOpenToClosed(EngageDocDbWrapper engagement)
        {
            if (engagement == null)
            {
                throw new ArgumentNullException("engagement");
            }

            engagement.EngagementStatus = EngagementStatus.Closed;
            engagement.SubmissionClosedUtc = DateTime.UtcNow;
            
            await InitDb();

            await InitCollection(engagement.EngagementType);

            await client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(DbName, engagement.EngagementType, engagement.Id), engagement);
        }

        private async Task DeleteEngagemntDocument(string engagementType, string oldGuid)
        {
            if (string.IsNullOrWhiteSpace(engagementType))
            {
                throw new ArgumentNullException("engagementType");
            }

            if (string.IsNullOrWhiteSpace(oldGuid))
            {
                throw new ArgumentNullException("oldGuid");
            }

            await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DbName, engagementType, oldGuid));
        }

        private async Task CreateEngagementDocumentIfNotExists(EngageDocDbWrapper engagement)
        {
            if (engagement == null)
            {
                throw new ArgumentNullException("engagement");
            }

            try
            {
                await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DbName, "Engagements", engagement.Id));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DbName, "Engagements"), engagement);
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task InitDb()
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(DbName));
            }
            catch (DocumentClientException de)
            {
                // If the database does not exist, create a new database
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = DbName });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task InitCollection(string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentNullException("collectionName");
            }

            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(DbName, collectionName));
            }
            catch (DocumentClientException de)
            {
                // If the document collection does not exist, create a new collection
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = collectionName;

                    // Configure collections for maximum query flexibility including string range queries.
                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

                    // Here we create a collection with 400 RU/s.
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(DbName),
                        collectionInfo,
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}