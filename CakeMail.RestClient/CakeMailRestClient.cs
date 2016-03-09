﻿using CakeMail.RestClient.Exceptions;
using CakeMail.RestClient.Models;
using CakeMail.RestClient.Resources;
using CakeMail.RestClient.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace CakeMail.RestClient
{
	/// <summary>
	/// Core class for using the CakeMail Api
	/// </summary>
	public class CakeMailRestClient
	{
		#region Fields

		private static readonly string _version = GetVersion();
		private readonly IRestClient _client;

		#endregion

		#region Properties

		/// <summary>
		/// The API key provided by CakeMail
		/// </summary>
		public string ApiKey { get; private set; }

		/// <summary>
		/// The web proxy
		/// </summary>
		public IWebProxy Proxy
		{
			get { return _client.Proxy; }
		}

		/// <summary>
		/// The user agent
		/// </summary>
		public string UserAgent
		{
			get { return _client.UserAgent; }
		}

		/// <summary>
		/// The timeout
		/// </summary>
		public int Timeout
		{
			get { return _client.Timeout; }
		}

		/// <summary>
		/// The URL where all API requests are sent
		/// </summary>
		public Uri BaseUrl
		{
			get { return _client.BaseUrl; }
		}

		public Campaigns Campaigns { get; private set; }
		public Clients Clients { get; private set; }
		public Countries Countries { get; private set; }
		public Permissions Permissions { get; private set; }
		public Lists Lists { get; private set; }
		public Timezones Timezones { get; private set; }
		public Mailings Mailings { get; private set; }
		public Relays Relays { get; private set; }
		public Segments Segments { get; private set; }

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes a new instance of the <see cref="CakeMailRestClient"/> class.
		/// </summary>
		/// <param name="apiKey">The API Key received from CakeMail</param>
		/// <param name="restClient">The rest client</param>
		public CakeMailRestClient(string apiKey, IRestClient restClient)
		{
			this.ApiKey = apiKey;
			_client = restClient;

			InitializeResources();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CakeMailRestClient"/> class.
		/// </summary>
		/// <param name="apiKey">The API Key received from CakeMail</param>
		/// <param name="host">The host where the API is hosted. The default is api.wbsrvc.com</param>
		/// <param name="timeout">Timeout in milliseconds for connection to web service. The default is 5000.</param>
		/// <param name="webProxy">The web proxy</param>
		public CakeMailRestClient(string apiKey, string host = "api.wbsrvc.com", int timeout = 5000, IWebProxy webProxy = null)
		{
			this.ApiKey = apiKey;

			_client = new RestSharp.RestClient("https://" + host)
			{
				Timeout = timeout,
				UserAgent = string.Format("CakeMail .NET REST Client {0}", _version),
				Proxy = webProxy
			};

			InitializeResources();
		}

		#endregion

		#region Methods related to SUPPRESSION LISTS

		/// <summary>
		/// Add email addresses to the suppression list
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="emailAddresses">The email addresses to add to the suppression list</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>An enumeration of <see cref="SuppressEmailResult">results</see>. Each item in this enumeration indicates the result of adding an email address to the suppression list.</returns>
		public IEnumerable<SuppressEmailResult> AddEmailAddressesToSuppressionList(string userKey, IEnumerable<string> emailAddresses, long? clientId = null)
		{
			string path = "/SuppressionList/ImportEmails/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey)
			};
			if (emailAddresses != null)
			{
				foreach (var item in emailAddresses.Select((email, i) => new { Index = i, Email = email }))
				{
					parameters.Add(new KeyValuePair<string, object>(string.Format("email[{0}]", item.Index), item.Email));
				}
			}
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<SuppressEmailResult>(path, parameters, null);
		}

		/// <summary>
		/// Add domains to the suppression list
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="domains">The domains to add to the suppression list</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>An enumeration of <see cref="SuppressDomainResult">results</see>. Each item in this enumeration indicates the result of adding a domain to the suppression list.</returns>
		public IEnumerable<SuppressDomainResult> AddDomainsToSuppressionList(string userKey, IEnumerable<string> domains, long? clientId = null)
		{
			string path = "/SuppressionList/ImportDomains/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey)
			};
			if (domains != null)
			{
				foreach (var item in domains.Select((domain, i) => new { Index = i, Domain = domain }))
				{
					parameters.Add(new KeyValuePair<string, object>(string.Format("domain[{0}]", item.Index), item.Domain));
				}
			}
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<SuppressDomainResult>(path, parameters, null);
		}

		/// <summary>
		/// Add localparts to the suppression list
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="localParts">The localparts to add to the suppression list</param>
		/// <param name="clientId">Client ID of the client.</param>
		/// <returns>An enumeration of <see cref="SuppressLocalPartResult">results</see>. Each item in this enumeration indicates the result of adding a localpart to the suppression list.</returns>
		public IEnumerable<SuppressLocalPartResult> AddLocalPartsToSuppressionList(string userKey, IEnumerable<string> localParts, long? clientId = null)
		{
			string path = "/SuppressionList/ImportLocalparts/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey)
			};
			if (localParts != null)
			{
				foreach (var item in localParts.Select((localPart, i) => new { Index = i, LocalPart = localPart }))
				{
					parameters.Add(new KeyValuePair<string, object>(string.Format("localpart[{0}]", item.Index), item.LocalPart));
				}
			}
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<SuppressLocalPartResult>(path, parameters, "localparts");
		}

		/// <summary>
		/// Remove email addresses from the suppression list
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="emailAddresses">The email addresses to remove from the suppression list</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>An enumeration of <see cref="SuppressEmailResult">results</see>. Each item in this enumeration indicates the result of removing an email address from the suppression list.</returns>
		public IEnumerable<SuppressEmailResult> RemoveEmailAddressesFromSuppressionList(string userKey, IEnumerable<string> emailAddresses, long? clientId = null)
		{
			string path = "/SuppressionList/DeleteEmails/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey)
			};
			if (emailAddresses != null)
			{
				foreach (var item in emailAddresses.Select((email, i) => new { Index = i, Email = email }))
				{
					parameters.Add(new KeyValuePair<string, object>(string.Format("email[{0}]", item.Index), item.Email));
				}
			}
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<SuppressEmailResult>(path, parameters, null);
		}

		/// <summary>
		/// Remove domains from the suppression list
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="domains">The domains to remove from the suppression list</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>An enumeration of <see cref="SuppressDomainResult">results</see>. Each item in this enumeration indicates the result of removing a domain from the suppression list.</returns>
		public IEnumerable<SuppressDomainResult> RemoveDomainsFromSuppressionList(string userKey, IEnumerable<string> domains, long? clientId = null)
		{
			string path = "/SuppressionList/DeleteDomains/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey)
			};
			if (domains != null)
			{
				foreach (var item in domains.Select((domain, i) => new { Index = i, Domain = domain }))
				{
					parameters.Add(new KeyValuePair<string, object>(string.Format("domain[{0}]", item.Index), item.Domain));
				}
			}
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<SuppressDomainResult>(path, parameters, null);
		}

		/// <summary>
		/// Remove localparts from the suppression list
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="localParts">The localparts to remove from the suppression list</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>An enumeration of <see cref="SuppressLocalPartResult">results</see>. Each item in this enumeration indicates the result of removing a localpart from the suppression list.</returns>
		public IEnumerable<SuppressLocalPartResult> RemoveLocalPartsFromSuppressionList(string userKey, IEnumerable<string> localParts, long? clientId = null)
		{
			string path = "/SuppressionList/DeleteLocalparts/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey)
			};
			if (localParts != null)
			{
				foreach (var item in localParts.Select((localPart, i) => new { Index = i, LocalPart = localPart }))
				{
					parameters.Add(new KeyValuePair<string, object>(string.Format("localpart[{0}]", item.Index), item.LocalPart));
				}
			}
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<SuppressLocalPartResult>(path, parameters, "localparts");
		}

		/// <summary>
		/// Retrieve the email addresses on the suppression list
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="limit">Limit the number of resulting email addresses.</param>
		/// <param name="offset">Offset the beginning of resulting email addresses.</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>An enumeration of <see cref="SuppressedEmail">addresses</see>. The result also indicates how each email address ended up on the suppression list.</returns>
		public IEnumerable<SuppressedEmail> GetSuppressedEmailAddresses(string userKey, int? limit = 0, int? offset = 0, long? clientId = null)
		{
			var path = "/SuppressionList/ExportEmails/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "false")
			};
			if (limit > 0) parameters.Add(new KeyValuePair<string, object>("limit", limit));
			if (offset > 0) parameters.Add(new KeyValuePair<string, object>("offset", offset));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<SuppressedEmail>(path, parameters, "emails");
		}

		/// <summary>
		/// Retrieve the domains on the suppression list
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="limit">Limit the number of resulting domains.</param>
		/// <param name="offset">Offset the beginning of resulting domains.</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>An enumeration of domains.</returns>
		public IEnumerable<string> GetSuppressedDomains(string userKey, int? limit = 0, int? offset = 0, long? clientId = null)
		{
			var path = "/SuppressionList/ExportDomains/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "false")
			};
			if (limit > 0) parameters.Add(new KeyValuePair<string, object>("limit", limit));
			if (offset > 0) parameters.Add(new KeyValuePair<string, object>("offset", offset));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			var result = ExecuteArrayRequest<ExpandoObject>(path, parameters, "domains");

			var domains = (from r in result select r.Single(p => p.Key == "domain").Value.ToString()).ToArray();
			return domains;
		}

		/// <summary>
		/// Retrieve the localparts on the suppression list
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="limit">Limit the number of resulting localparts.</param>
		/// <param name="offset">Offset the beginning of resulting localparts.</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>An enumeration of localparts.</returns>
		public IEnumerable<string> GetSuppressedLocalParts(string userKey, int? limit = 0, int? offset = 0, long? clientId = null)
		{
			var path = "/SuppressionList/ExportLocalparts/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "false")
			};
			if (limit > 0) parameters.Add(new KeyValuePair<string, object>("limit", limit));
			if (offset > 0) parameters.Add(new KeyValuePair<string, object>("offset", offset));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			var result = ExecuteArrayRequest<ExpandoObject>(path, parameters, "localparts");

			var localParts = (from r in result select r.Single(p => p.Key == "localpart").Value.ToString()).ToArray();
			return localParts;
		}

		/// <summary>
		/// Get a count of email addresses on the suppression list
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>The number of email addresses on the suppresssion list</returns>
		public long GetSuppressedEmailAddressesCount(string userKey, long? clientId = null)
		{
			var path = "/SuppressionList/ExportEmails/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "true")
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteCountRequest(path, parameters);
		}

		/// <summary>
		/// Get a count of domains on the suppression list
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>The number of domains on the suppresssion list</returns>
		public long GetSuppressedDomainsCount(string userKey, long? clientId = null)
		{
			var path = "/SuppressionList/ExportDomains/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "true")
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteCountRequest(path, parameters);
		}

		/// <summary>
		/// Get a count of localparts on the suppression list
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>The number of localparts on the suppresssion list</returns>
		public long GetSuppressedLocalPartsCount(string userKey, long? clientId = null)
		{
			var path = "/SuppressionList/ExportLocalparts/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "true")
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteCountRequest(path, parameters);
		}

		#endregion

		#region Methods related to TRIGGERS

		/// <summary>
		/// Create a trigger
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="name">Name of the trigger.</param>
		/// <param name="listId">ID of the list you want to associate the trigger with.</param>
		/// <param name="campaignId">ID of the campaign you want to associate the trigger with.</param>
		/// <param name="encoding">Encoding to be used for the trigger. Possible values: 'utf-8', 'iso-8859-x'</param>
		/// <param name="transferEncoding">Transfer encoding to be used for the trigger. Possible values: 'quoted-printable', 'base64'</param>
		/// <param name="clientId">Client ID of the client in which the mailing is created.</param>
		/// <returns>ID of the new trigger</returns>
		public long CreateTrigger(string userKey, string name, long listId, long? campaignId = null, MessageEncoding? encoding = null, TransferEncoding? transferEncoding = null, long? clientId = null)
		{
			string path = "/Trigger/Create/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("name", name),
				new KeyValuePair<string, object>("list_id", listId)
			};
			if (encoding.HasValue) parameters.Add(new KeyValuePair<string, object>("encoding", encoding.Value.GetEnumMemberValue()));
			if (transferEncoding.HasValue) parameters.Add(new KeyValuePair<string, object>("transfer_encoding", transferEncoding.Value.GetEnumMemberValue()));
			if (campaignId.HasValue) parameters.Add(new KeyValuePair<string, object>("campaign_id", campaignId.Value));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<long>(path, parameters);
		}

		//public bool DeleteTrigger(string userKey, long triggerId, long? clientId = null)
		//{
		//	string path = "/Trigger/Delete/";

		//	var parameters = new List<KeyValuePair<string, object>>()
		//	{
		//		new KeyValuePair<string, object>("user_key", userKey),
		//		new KeyValuePair<string, object>("trigger_id", triggerId),
		//	};
		//	if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

		//	return ExecuteRequest<bool>(path, parameters);
		//}

		/// <summary>
		/// Retrieve a trigger
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>The <see cref="Trigger">trigger</see></returns>
		public Trigger GetTrigger(string userKey, long triggerId, long? clientId = null)
		{
			var path = "/Trigger/GetInfo/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId)
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<Trigger>(path, parameters);
		}

		/// <summary>
		/// Update a trigger
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger</param>
		/// <param name="campaignId">ID of the campaign you want to associate the trigger with.</param>
		/// <param name="name">Name of the trigger</param>
		/// <param name="action">Action of the trigger. Possible values: 'opt-in', 'douopt-in', 'opt-out', 'specific', 'annual'</param>
		/// <param name="encoding">Encoding to be used for the trigger. Possible values: 'utf-8', 'iso-8859-x'</param>
		/// <param name="transferEncoding">Transfer encoding to be used for the trigger. Possible values: 'quoted-printable', 'base64'</param>
		/// <param name="subject">Subject of the trigger.</param>
		/// <param name="senderEmail">Email address of the sender of the trigger.</param>
		/// <param name="senderName">Name of the sender of the trigger.</param>
		/// <param name="replyTo">Email address of the reply-to of the trigger.</param>
		/// <param name="htmlContent">HTML content of the trigger.</param>
		/// <param name="textContent">Text content of the trigger.</param>
		/// <param name="trackOpens">Track the opens in the HTML version.</param>
		/// <param name="trackClicksInHtml">Track the clicks in the HTML version.</param>
		/// <param name="trackClicksInText">Track the clicks in the text version.</param>
		/// <param name="trackingParameters">Additional tracking parameters for links.</param>
		/// <param name="delay">Delay (in seconds) to be used when the trigger is unleashed.</param>
		/// <param name="status">Status of the trigger. Possible values: 'active', 'inactive'</param>
		/// <param name="date">DateTime to be used for trigger with action 'specific' or 'annual'.</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>True if the trigger was updated</returns>
		public bool UpdateTrigger(string userKey, long triggerId, long? campaignId = null, string name = null, TriggerAction? action = null, MessageEncoding? encoding = null, TransferEncoding? transferEncoding = null, string subject = null, string senderEmail = null, string senderName = null, string replyTo = null, string htmlContent = null, string textContent = null, bool? trackOpens = null, bool? trackClicksInHtml = null, bool? trackClicksInText = null, string trackingParameters = null, int? delay = null, TriggerStatus? status = null, DateTime? date = null, long? clientId = null)
		{
			var path = "/Trigger/SetInfo/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId)
			};
			if (campaignId.HasValue) parameters.Add(new KeyValuePair<string, object>("campaign_id", campaignId.Value));
			if (name != null) parameters.Add(new KeyValuePair<string, object>("name", name));
			if (action.HasValue) parameters.Add(new KeyValuePair<string, object>("action", action.Value.GetEnumMemberValue()));
			if (encoding.HasValue) parameters.Add(new KeyValuePair<string, object>("encoding", encoding.Value.GetEnumMemberValue()));
			if (transferEncoding.HasValue) parameters.Add(new KeyValuePair<string, object>("transfer_encoding", transferEncoding.Value.GetEnumMemberValue()));
			if (subject != null) parameters.Add(new KeyValuePair<string, object>("subject", subject));
			if (senderEmail != null) parameters.Add(new KeyValuePair<string, object>("sender_email", senderEmail));
			if (senderName != null) parameters.Add(new KeyValuePair<string, object>("sender_name", senderName));
			if (replyTo != null) parameters.Add(new KeyValuePair<string, object>("reply_to", replyTo));
			if (htmlContent != null) parameters.Add(new KeyValuePair<string, object>("html_message", htmlContent));
			if (textContent != null) parameters.Add(new KeyValuePair<string, object>("text_message", textContent));
			if (trackOpens.HasValue) parameters.Add(new KeyValuePair<string, object>("opening_stats", trackOpens.Value ? "true" : "false"));
			if (trackClicksInHtml.HasValue) parameters.Add(new KeyValuePair<string, object>("clickthru_html", trackClicksInHtml.Value ? "true" : "false"));
			if (trackClicksInText.HasValue) parameters.Add(new KeyValuePair<string, object>("clickthru_text", trackClicksInText.Value ? "true" : "false"));
			if (trackingParameters != null) parameters.Add(new KeyValuePair<string, object>("tracking_params", trackingParameters));
			if (delay != null) parameters.Add(new KeyValuePair<string, object>("delay", delay));
			if (status.HasValue) parameters.Add(new KeyValuePair<string, object>("status", status.Value.GetEnumMemberValue()));
			if (date.HasValue) parameters.Add(new KeyValuePair<string, object>("date_field", date.Value.ToCakeMailString()));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<bool>(path, parameters);
		}

		/// <summary>
		/// Retrieve the triggers matching the filtering criteria
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="status">Filter using the trigger status. Possible values: 'active', 'inactive'</param>
		/// <param name="action">Filter using the trigger action. Possible values: 'opt-in', 'douopt-in', 'opt-out', 'specific', 'annual'</param>
		/// <param name="listId">Filter using the ID of the trigger list.</param>
		/// <param name="campaignId">Filter using the ID of the trigger campaign.</param>
		/// <param name="limit">Limit the number of resulting triggers.</param>
		/// <param name="offset">Offset the beginning of resulting triggers.</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>An enumeration of <see cref="Trigger">triggers</see> matching the filter criteria</returns>
		public IEnumerable<Trigger> GetTriggers(string userKey, TriggerStatus? status = null, TriggerAction? action = null, long? listId = null, long? campaignId = null, int? limit = 0, int? offset = 0, long? clientId = null)
		{
			var path = "/Trigger/GetList/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "false")
			};
			if (status.HasValue) parameters.Add(new KeyValuePair<string, object>("status", status.Value.GetEnumMemberValue()));
			if (action.HasValue) parameters.Add(new KeyValuePair<string, object>("action", action.Value.GetEnumMemberValue()));
			if (listId.HasValue) parameters.Add(new KeyValuePair<string, object>("list_id", listId.Value));
			if (campaignId.HasValue) parameters.Add(new KeyValuePair<string, object>("campaign_id", campaignId.Value));
			if (limit > 0) parameters.Add(new KeyValuePair<string, object>("limit", limit));
			if (offset > 0) parameters.Add(new KeyValuePair<string, object>("offset", offset));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<Trigger>(path, parameters, "triggers");
		}

		/// <summary>
		/// Get a count of triggers matching the filtering criteria.
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="status">Filter using the trigger status. Possible values: 'active', 'inactive'</param>
		/// <param name="action">Filter using the trigger action. Possible values: 'opt-in', 'douopt-in', 'opt-out', 'specific', 'annual'</param>
		/// <param name="listId">Filter using the ID of the trigger list.</param>
		/// <param name="campaignId">Filter using the ID of the trigger campaign.</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>The count of triggers matching the filtering criteria</returns>
		public long GetTriggersCount(string userKey, TriggerStatus? status = null, TriggerAction? action = null, long? listId = null, long? campaignId = null, long? clientId = null)
		{
			var path = "/Trigger/GetList/";
			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "true")
			};
			if (status.HasValue) parameters.Add(new KeyValuePair<string, object>("status", status.Value.GetEnumMemberValue()));
			if (action.HasValue) parameters.Add(new KeyValuePair<string, object>("action", action.Value.GetEnumMemberValue()));
			if (listId.HasValue) parameters.Add(new KeyValuePair<string, object>("list_id", listId.Value));
			if (campaignId.HasValue) parameters.Add(new KeyValuePair<string, object>("campaign_id", campaignId.Value));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteCountRequest(path, parameters);
		}

		/// <summary>
		/// Send a test of a trigger.
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger</param>
		/// <param name="recipientEmail">Email address where the test will be sent.</param>
		/// <param name="separated">True if you want the HTML and the text to be sent seperatly, false if you want to combine the HTML and the text in a multi-part email.</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>True if the test email was sent</returns>
		public bool SendTriggerTestEmail(string userKey, long triggerId, string recipientEmail, bool separated = false, long? clientId = null)
		{
			var path = "/Trigger/SendTestEmail/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId),
				new KeyValuePair<string, object>("test_email", recipientEmail),
				new KeyValuePair<string, object>("test_type", separated ? "separated" : "merged")
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<bool>(path, parameters);
		}

		/// <summary>
		/// Get the multi-part version of a trigger
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>The <see cref="RawEmailMessage">multi-part message</see></returns>
		public RawEmailMessage GetTriggerRawEmailMessage(string userKey, long triggerId, long? clientId = null)
		{
			var path = "/Trigger/GetEmailMessage/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId)
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<RawEmailMessage>(path, parameters);
		}

		/// <summary>
		/// Get the rendered HTML version of a trigger.
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>The rendered HTML</returns>
		public string GetTriggerRawHtml(string userKey, long triggerId, long? clientId = null)
		{
			var path = "/Trigger/GetHtmlMessage/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId)
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<string>(path, parameters);
		}

		/// <summary>
		/// Get the rendered text version of a mailing.
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>The rendered text</returns>
		public string GetTriggerRawText(string userKey, long triggerId, long? clientId = null)
		{
			var path = "/Trigger/GetTextMessage/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId)
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<string>(path, parameters);
		}

		/// <summary>
		/// Unleash a trigger
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger</param>
		/// <param name="listMemberId">ID of the member to unleash the trigger to.</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>True is the trigger is unleashed</returns>
		public bool UnleashTrigger(string userKey, long triggerId, long listMemberId, long? clientId = null)
		{
			var path = "/Trigger/Unleash/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId),
				new KeyValuePair<string, object>("record_id", listMemberId)
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<bool>(path, parameters);
		}

		/// <summary>
		/// Retrieve the log items for a given trigger
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger.</param>
		/// <param name="logType">Filter using the log action.</param>
		/// <param name="listMemberId">Filter using the ID of the member.</param>
		/// <param name="uniques">Return unique log item per member.</param>
		/// <param name="totals">Return all the log items.</param>
		/// <param name="start">Filter using a start date</param>
		/// <param name="end">Filter using an end date</param>
		/// <param name="limit">Limit the number of resulting log items.</param>
		/// <param name="offset">Offset the beginning of resulting log items.</param>
		/// <param name="clientId">Client ID of the client in which the mailing is located.</param>
		/// <returns>An enumeration of <see cref="LogItem">log items</see> matching the filter criteria</returns>
		public IEnumerable<LogItem> GetTriggerLogs(string userKey, long triggerId, LogType? logType = null, long? listMemberId = null, bool uniques = false, bool totals = false, DateTime? start = null, DateTime? end = null, int? limit = 0, int? offset = 0, long? clientId = null)
		{
			string path = "/Trigger/GetLog/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId),
				new KeyValuePair<string, object>("totals", totals ? "true" : "false"),
				new KeyValuePair<string, object>("uniques", uniques ? "true" : "false"),
				new KeyValuePair<string, object>("count", "false")
			};
			if (logType.HasValue) parameters.Add(new KeyValuePair<string, object>("action", logType.Value.GetEnumMemberValue()));
			if (start.HasValue) parameters.Add(new KeyValuePair<string, object>("start_time", start.Value.ToCakeMailString()));
			if (end.HasValue) parameters.Add(new KeyValuePair<string, object>("end_time", end.Value.ToCakeMailString()));
			if (listMemberId.HasValue) parameters.Add(new KeyValuePair<string, object>("record_id", listMemberId.Value));
			if (limit > 0) parameters.Add(new KeyValuePair<string, object>("limit", limit));
			if (offset > 0) parameters.Add(new KeyValuePair<string, object>("offset", offset));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<LogItem>(path, parameters, "logs");
		}

		/// <summary>
		/// Get a count of log items matching the filter criteria
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger.</param>
		/// <param name="logType">Filter using the log action.</param>
		/// <param name="listMemberId">Filter using the ID of the member.</param>
		/// <param name="uniques">Return unique log item per member.</param>
		/// <param name="totals">Return all the log items.</param>
		/// <param name="start">Filter using a start date</param>
		/// <param name="end">Filter using an end date</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>The number of log items matching the filtering criteria</returns>
		public long GetTriggerLogsCount(string userKey, long triggerId, LogType? logType = null, long? listMemberId = null, bool uniques = false, bool totals = false, DateTime? start = null, DateTime? end = null, long? clientId = null)
		{
			string path = "/Trigger/GetLog/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId),
				new KeyValuePair<string, object>("totals", totals ? "true" : "false"),
				new KeyValuePair<string, object>("uniques", uniques ? "true" : "false"),
				new KeyValuePair<string, object>("count", "true")
			};
			if (logType.HasValue) parameters.Add(new KeyValuePair<string, object>("action", logType.Value.GetEnumMemberValue()));
			if (start.HasValue) parameters.Add(new KeyValuePair<string, object>("start_time", start.Value.ToCakeMailString()));
			if (end.HasValue) parameters.Add(new KeyValuePair<string, object>("end_time", end.Value.ToCakeMailString()));
			if (listMemberId.HasValue) parameters.Add(new KeyValuePair<string, object>("record_id", listMemberId.Value));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteCountRequest(path, parameters);
		}

		/// <summary>
		/// Retrieve the links for a given trigger
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger.</param>
		/// <param name="limit">Limit the number of resulting log items.</param>
		/// <param name="offset">Offset the beginning of resulting log items.</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>An enumeration of <see cref="Link">links</see> matching the filter criteria</returns>
		public IEnumerable<Link> GetTriggerLinks(string userKey, long triggerId, int? limit = 0, int? offset = 0, long? clientId = null)
		{
			string path = "/Trigger/GetLinks/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId),
				new KeyValuePair<string, object>("count", "false")
			};
			if (limit > 0) parameters.Add(new KeyValuePair<string, object>("limit", limit));
			if (offset > 0) parameters.Add(new KeyValuePair<string, object>("offset", offset));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<Link>(path, parameters, "links");
		}

		/// <summary>
		/// Get a count of links matching the filter criteria
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger.</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>The number of links matching the filtering criteria</returns>
		public long GetTriggerLinksCount(string userKey, long triggerId, long? clientId = null)
		{
			string path = "/Trigger/GetLinks/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId),
				new KeyValuePair<string, object>("count", "true")
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteCountRequest(path, parameters);
		}

		/// <summary>
		/// Retrieve a link
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="linkId">ID of the link.</param>
		/// <param name="clientId">Client ID of the client in which the link is located.</param>
		/// <returns>The <see cref="Link">link</see></returns>
		/// <remarks>
		/// This method is documented on CakeMail's web site (http://dev.cakemail.com/api/Trigger/GetLinkInfo) but unfortunately, it's not implemented.
		/// Invoking this method will result in an exception with the following error message: "Invalid Method: GetLinkInfo".
		/// </remarks>
		public Link GetTriggerLink(string userKey, long linkId, long? clientId = null)
		{
			string path = "/Trigger/GetLinkInfo/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("link_id", linkId)
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<Link>(path, parameters);
		}

		/// <summary>
		/// Retrieve the links (with their statistics) for a given trigger
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger.</param>
		/// <param name="start">Filter using a start date</param>
		/// <param name="end">Filter using an end date</param>
		/// <param name="limit">Limit the number of resulting log items.</param>
		/// <param name="offset">Offset the beginning of resulting log items.</param>
		/// <param name="clientId">Client ID of the client in which the mailing is located.</param>
		/// <returns>An enumeration of <see cref="LinkStats">links with their statistics</see> matching the filter criteria</returns>
		public IEnumerable<LinkStats> GetTriggerLinksWithStats(string userKey, long triggerId, DateTime? start = null, DateTime? end = null, int? limit = 0, int? offset = 0, long? clientId = null)
		{
			string path = "/Trigger/GetLinksLog/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId),
				new KeyValuePair<string, object>("count", "false")
			};
			if (start.HasValue) parameters.Add(new KeyValuePair<string, object>("start_time", start.Value.ToCakeMailString()));
			if (end.HasValue) parameters.Add(new KeyValuePair<string, object>("end_time", end.Value.ToCakeMailString()));
			if (limit > 0) parameters.Add(new KeyValuePair<string, object>("limit", limit));
			if (offset > 0) parameters.Add(new KeyValuePair<string, object>("offset", offset));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<LinkStats>(path, parameters, "links");
		}

		/// <summary>
		/// Get a count of links (with their statistics) for a given trigger
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="triggerId">ID of the trigger.</param>
		/// <param name="start">Filter using a start date</param>
		/// <param name="end">Filter using an end date</param>
		/// <param name="clientId">Client ID of the client in which the trigger is located.</param>
		/// <returns>The number of links matching the filter criteria</returns>
		public long GetTriggerLinksWithStatsCount(string userKey, long triggerId, DateTime? start = null, DateTime? end = null, long? clientId = null)
		{
			string path = "/Trigger/GetLinksLog/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("trigger_id", triggerId),
				new KeyValuePair<string, object>("count", "true")
			};
			if (start.HasValue) parameters.Add(new KeyValuePair<string, object>("start_time", start.Value.ToCakeMailString()));
			if (end.HasValue) parameters.Add(new KeyValuePair<string, object>("end_time", end.Value.ToCakeMailString()));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteCountRequest(path, parameters);
		}

		#endregion

		#region Methods related to TEMPLATES

		/// <summary>
		/// Create a template category
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="labels">Name of the category.</param>
		/// <param name="isVisibleByDefault">Is the category visible by default.</param>
		/// <param name="templatesCanBeCopied">Are the templates in the category copyable.</param>
		/// <param name="clientId">Client ID of the client in which the category is created.</param>
		/// <returns>ID of the new template category</returns>
		public long CreateTemplateCategory(string userKey, IDictionary<string, string> labels, bool isVisibleByDefault = true, bool templatesCanBeCopied = true, long? clientId = null)
		{
			string path = "/TemplateV2/CreateCategory/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("default", isVisibleByDefault ? "1" : "0"),
				new KeyValuePair<string, object>("templates_copyable", templatesCanBeCopied ? "1" : "0")
			};
			if (labels != null)
			{
				foreach (var item in labels.Select((label, i) => new { Index = i, Language = label.Key, Name = label.Value }))
				{
					parameters.Add(new KeyValuePair<string, object>(string.Format("label[{0}][language]", item.Index), item.Language));
					parameters.Add(new KeyValuePair<string, object>(string.Format("label[{0}][name]", item.Index), item.Name));
				}
			}
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			// The data returned when creating a new category is a little bit unusual
			// Instead of simply returning the unique identifier of the new record like all the other 'Create' methods, for example: {"status":"success","data":"4593766"}
			// this method return an object with a single property called 'id' containing the unique identifier of the new record, like this: {"status":"success","data":{"id":"14052"}}
			return ExecuteRequest<long>(path, parameters, "id");
		}

		/// <summary>
		/// Delete a template category
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="categoryId">ID of the template category</param>
		/// <param name="clientId">Client ID of the client in which the template category is located.</param>
		/// <returns>True if the template category is deleted</returns>
		public bool DeleteTemplateCategory(string userKey, long categoryId, long? clientId = null)
		{
			string path = "/TemplateV2/DeleteCategory/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("category_id", categoryId),
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			// The data returned when deleting a category is a little bit unusual
			// Instead of returning a boolean value to indicate success, it returns an empty array!!!
			// For example:  {"status":"success","data":[]}
			ExecuteRequest(path, parameters);
			return true;
		}

		/// <summary>
		/// Retrieve a template category
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="categoryId">ID of the category</param>
		/// <param name="clientId">Client ID of the client in which the category is located.</param>
		/// <returns>The <see cref="TemplateCategory">category</see></returns>
		public TemplateCategory GetTemplateCategory(string userKey, long categoryId, long? clientId = null)
		{
			var path = "/TemplateV2/GetCategory/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("category_id", categoryId)
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<TemplateCategory>(path, parameters);
		}

		/// <summary>
		/// Retrieve the template categories matching the filtering criteria.
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="limit">Limit the number of resulting categories.</param>
		/// <param name="offset">Offset the beginning of resulting categories.</param>
		/// <param name="clientId">Client ID of the client in which the categories are located.</param>
		/// <returns>Enumeration of <see cref="TemplateCategory">categories</see> matching the filtering criteria</returns>
		public IEnumerable<TemplateCategory> GetTemplateCategories(string userKey, int? limit = 0, int? offset = 0, long? clientId = null)
		{
			var path = "/TemplateV2/GetCategories/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "false")
			};
			if (limit > 0) parameters.Add(new KeyValuePair<string, object>("limit", limit));
			if (offset > 0) parameters.Add(new KeyValuePair<string, object>("offset", offset));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<TemplateCategory>(path, parameters, "categories");
		}

		/// <summary>
		/// Get a count of template categories matching the filtering criteria.
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="clientId">Client ID of the client in which the categories are located.</param>
		/// <returns>The count of categories matching the filtering criteria</returns>
		public long GetTemplateCategoriesCount(string userKey, long? clientId = null)
		{
			var path = "/TemplateV2/GetCategories/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "true")
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteCountRequest(path, parameters);
		}

		/// <summary>
		/// Update a template category
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="categoryId">ID of the category</param>
		/// <param name="labels">Name of the category.</param>
		/// <param name="isVisibleByDefault">Is the category visible by default.</param>
		/// <param name="templatesCanBeCopied">Are the templates in the category copyable.</param>
		/// <param name="clientId">Client ID of the client in which the category is located.</param>
		/// <returns>True if the category was updated</returns>
		public bool UpdateTemplateCategory(string userKey, long categoryId, IDictionary<string, string> labels, bool isVisibleByDefault = true, bool templatesCanBeCopied = true, long? clientId = null)
		{
			string path = "/TemplateV2/SetCategory/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("category_id", categoryId),
				new KeyValuePair<string, object>("default", isVisibleByDefault ? "1" : "0"),
				new KeyValuePair<string, object>("templates_copyable", templatesCanBeCopied ? "1" : "0"),
			};
			if (labels != null)
			{
				foreach (var label in labels)
				{
					parameters.Add(new KeyValuePair<string, object>(string.Format("label[{0}]", label.Key), label.Value));
				}
			}
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<bool>(path, parameters);
		}

		/// <summary>
		/// Retrieve the list of permissions for a category
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="categoryId">ID of the category</param>
		/// <param name="limit">Limit the number of resulting permissions.</param>
		/// <param name="offset">Offset the beginning of resulting permissions.</param>
		/// <param name="clientId">ID of the client in which the category is located.</param>
		/// <returns>An enumeration of permissions</returns>
		public IEnumerable<TemplateCategoryVisibility> GetTemplateCategoryVisibility(string userKey, long categoryId, int? limit = 0, int? offset = 0, long? clientId = null)
		{
			var path = "/TemplateV2/GetCategoryVisibility/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("category_id", categoryId),
				new KeyValuePair<string, object>("count", "false")
			};
			if (limit > 0) parameters.Add(new KeyValuePair<string, object>("limit", limit));
			if (offset > 0) parameters.Add(new KeyValuePair<string, object>("offset", offset));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<TemplateCategoryVisibility>(path, parameters, "clients");
		}

		/// <summary>
		/// Get a count of permissions for a given template category.
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="categoryId">ID of the category</param>
		/// <param name="clientId">ID of the client</param>
		/// <returns>The count of permissions matching the filtering criteria</returns>
		public long GetTemplateCategoryVisibilityCount(string userKey, long categoryId, long? clientId = null)
		{
			var path = "/TemplateV2/GetCategoryVisibility/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("category_id", categoryId),
				new KeyValuePair<string, object>("count", "true")
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteCountRequest(path, parameters);
		}

		/// <summary>
		/// Set the permissions for a category
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="categoryId">ID of the category</param>
		/// <param name="clientVisibility">The list of clients and their associated boolean that indicates if they have access to the category</param>
		/// <param name="clientId">ID of the client in which the category is located.</param>
		/// <returns>True if the permissions are successfully updated</returns>
		public bool SetTemplateCategoryVisibility(string userKey, long categoryId, IDictionary<long, bool> clientVisibility, long? clientId = null)
		{
			var path = "/TemplateV2/SetCategoryVisibility/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("category_id", categoryId),
			};
			if (clientVisibility != null)
			{
				foreach (var item in clientVisibility.Select((visibility, i) => new { Index = i, ClientId = visibility.Key, Visible = visibility.Value }))
				{
					parameters.Add(new KeyValuePair<string, object>(string.Format("client[{0}][client_id]", item.Index), item.ClientId));
					parameters.Add(new KeyValuePair<string, object>(string.Format("client[{0}][visible]", item.Index), item.Visible ? "true" : "false"));
				}
			}
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<bool>(path, parameters);
		}

		/// <summary>
		/// Create a template
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="labels">Name of the template.</param>
		/// <param name="content">Content of the template.</param>
		/// <param name="categoryId">ID of the category.</param>
		/// <param name="clientId">Client ID of the client in which the template is created.</param>
		/// <returns>ID of the new template</returns>
		public long CreateTemplate(string userKey, IDictionary<string, string> labels, string content, long categoryId, long? clientId = null)
		{
			string path = "/TemplateV2/CreateTemplate/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("content", content),
				new KeyValuePair<string, object>("category_id", categoryId)
			};
			if (labels != null)
			{
				foreach (var item in labels.Select((label, i) => new { Index = i, Language = label.Key, Name = label.Value }))
				{
					parameters.Add(new KeyValuePair<string, object>(string.Format("label[{0}][language]", item.Index), item.Language));
					parameters.Add(new KeyValuePair<string, object>(string.Format("label[{0}][name]", item.Index), item.Name));
				}
			}
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			// The data returned when creating a new template is a little bit unusual
			// Instead of simply returning the unique identifier of the new record like all the other 'Create' methods, for example: {"status":"success","data":"4593766"}
			// this method return an object with a single property called 'id' containing the unique identifier of the new record, like this: {"status":"success","data":{"id":"14052"}}
			return ExecuteRequest<long>(path, parameters, "id");
		}

		/// <summary>
		/// Delete a template
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="templateId">ID of the template</param>
		/// <param name="clientId">Client ID of the client in which the template is located.</param>
		/// <returns>True if the template is deleted</returns>
		public bool DeleteTemplate(string userKey, long templateId, long? clientId = null)
		{
			string path = "/TemplateV2/DeleteTemplate/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("template_id", templateId),
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			// The data returned when deleting a template is a little bit unusual
			// Instead of returning a boolean value to indicate success, it returns an empty array!!!
			// For example:  {"status":"success","data":[]}
			ExecuteRequest(path, parameters);
			return true;
		}

		/// <summary>
		/// Retrieve a template
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="templateId">ID of the template</param>
		/// <param name="clientId">Client ID of the client in which the template is located.</param>
		/// <returns>The <see cref="Template">template</see></returns>
		public Template GetTemplate(string userKey, long templateId, long? clientId = null)
		{
			var path = "/TemplateV2/GetTemplate/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("template_id", templateId)
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<Template>(path, parameters);
		}

		/// <summary>
		/// Retrieve the templates matching the filtering criteria.
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="categoryId">ID of the category</param>
		/// <param name="limit">Limit the number of resulting templates.</param>
		/// <param name="offset">Offset the beginning of resulting templates.</param>
		/// <param name="clientId">Client ID of the client in which the templates are located.</param>
		/// <returns>Enumeration of <see cref="Template">templates</see> matching the filtering criteria</returns>
		public IEnumerable<Template> GetTemplates(string userKey, long? categoryId = null, int? limit = 0, int? offset = 0, long? clientId = null)
		{
			var path = "/TemplateV2/GetTemplates/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "false")
			};
			if (categoryId.HasValue) parameters.Add(new KeyValuePair<string, object>("category_id", categoryId.Value));
			if (limit > 0) parameters.Add(new KeyValuePair<string, object>("limit", limit));
			if (offset > 0) parameters.Add(new KeyValuePair<string, object>("offset", offset));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<Template>(path, parameters, "templates");
		}

		/// <summary>
		/// Get a count of templates matching the filtering criteria.
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="categoryId">ID of the category</param>
		/// <param name="clientId">Client ID of the client in which the templates are located.</param>
		/// <returns>The count of templates matching the filtering criteria</returns>
		public long GetTemplatesCount(string userKey, long? categoryId = null, long? clientId = null)
		{
			var path = "/TemplateV2/GetTemplates/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "true")
			};
			if (categoryId.HasValue) parameters.Add(new KeyValuePair<string, object>("category_id", categoryId.Value));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteCountRequest(path, parameters);
		}

		/// <summary>
		/// Update a template
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="templateId">ID of the template</param>
		/// <param name="labels">Name of the template.</param>
		/// <param name="content">Content of the template.</param>
		/// <param name="categoryId">ID of the category.</param>
		/// <param name="clientId">Client ID of the client in which the template is located.</param>
		/// <returns>True if the category was updated</returns>
		public bool UpdateTemplate(string userKey, long templateId, IDictionary<string, string> labels, string content = null, long? categoryId = null, long? clientId = null)
		{
			string path = "/TemplateV2/SetTemplate/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("template_id", templateId)
			};
			if (labels != null)
			{
				foreach (var label in labels)
				{
					parameters.Add(new KeyValuePair<string, object>(string.Format("label[{0}]", label.Key), label.Value));
				}
			}
			if (content != null) parameters.Add(new KeyValuePair<string, object>("content", content));
			if (categoryId.HasValue) parameters.Add(new KeyValuePair<string, object>("category_id", categoryId.Value));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<bool>(path, parameters);
		}

		#endregion

		#region Methods related to USERS

		/// <summary>
		/// Create a user
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="email">Email address of the user.</param>
		/// <param name="password">Password of the user.</param>
		/// <param name="firstName">First name of the user.</param>
		/// <param name="lastName">Last name of the user.</param>
		/// <param name="title">Title of the user.</param>
		/// <param name="officePhone">Office phone number of the user.</param>
		/// <param name="mobilePhone">Mobile phone number of the user.</param>
		/// <param name="language">Language of the user. For example: 'en_US' for English (US)</param>
		/// <param name="timezoneId">ID of the timezone of the user. UTC (id 542) is the default value</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>ID of the new user</returns>
		public long CreateUser(string userKey, string email, string password, string firstName = null, string lastName = null, string title = null, string officePhone = null, string mobilePhone = null, string language = null, long timezoneId = 542, long? clientId = null)
		{
			string path = "/User/Create/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("email", email),
				new KeyValuePair<string, object>("password", password),
				new KeyValuePair<string, object>("password_confirmation", password),
				new KeyValuePair<string, object>("timezone_id", timezoneId)
			};
			if (firstName != null) parameters.Add(new KeyValuePair<string, object>("first_name", firstName));
			if (lastName != null) parameters.Add(new KeyValuePair<string, object>("last_name", lastName));
			if (title != null) parameters.Add(new KeyValuePair<string, object>("title", title));
			if (officePhone != null) parameters.Add(new KeyValuePair<string, object>("office_phone", officePhone));
			if (mobilePhone != null) parameters.Add(new KeyValuePair<string, object>("mobile_phone", mobilePhone));
			if (language != null) parameters.Add(new KeyValuePair<string, object>("language", language));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			// When a new user is created, the payload contains a json object with two properties: 'id' and 'key'. We only care about the ID.
			return ExecuteRequest<long>(path, parameters, "id");
		}

		/// <summary>
		/// Suspend a user
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="userId">ID of the user.</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>True if the user is suspended</returns>
		public bool DeactivateUser(string userKey, long userId, long? clientId = null)
		{
			return UpdateUser(userKey, userId, status: UserStatus.Suspended, clientId: clientId);
		}

		/// <summary>
		/// Delete a user
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="userId">ID of the user.</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>True if the user is deleted</returns>
		public bool DeleteUser(string userKey, long userId, long? clientId = null)
		{
			return UpdateUser(userKey, userId, status: UserStatus.Deleted, clientId: clientId);
		}

		/// <summary>
		/// Retrieve a user
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="userId">ID of the user</param>
		/// <param name="clientId">ID of the client</param>
		/// <returns>The <see cref="User">user</see></returns>
		public User GetUser(string userKey, long userId, long? clientId = null)
		{
			var path = "/User/GetInfo/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("user_id", userId)
			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<User>(path, parameters);
		}

		/// <summary>
		/// Retrieve the users matching the filtering criteria.
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="status">Filter using the user status. Possible values: 'active', 'suspended'</param>
		/// <param name="limit">Limit the number of resulting users.</param>
		/// <param name="offset">Offset the beginning of resulting users.</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>Enumeration of <see cref="User">users</see> matching the filtering criteria</returns>
		public IEnumerable<User> GetUsers(string userKey, UserStatus? status = null, int? limit = 0, int? offset = 0, long? clientId = null)
		{
			var path = "/User/GetList/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "false")
			};
			if (status.HasValue) parameters.Add(new KeyValuePair<string, object>("status", status.Value.GetEnumMemberValue()));
			if (limit > 0) parameters.Add(new KeyValuePair<string, object>("limit", limit));
			if (offset > 0) parameters.Add(new KeyValuePair<string, object>("offset", offset));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteArrayRequest<User>(path, parameters, "users");
		}

		/// <summary>
		/// Get a count of users matching the filtering criteria.
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="status">Filter using the user status. Possible values: 'active', 'suspended'</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>The count of users matching the filtering criteria</returns>
		public long GetUsersCount(string userKey, UserStatus? status = null, long? clientId = null)
		{
			var path = "/User/GetList/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("count", "true")
			};
			if (status.HasValue) parameters.Add(new KeyValuePair<string, object>("status", status.Value.GetEnumMemberValue()));
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteCountRequest(path, parameters);
		}

		/// <summary>
		/// Update a user
		/// </summary>
		/// <param name="userKey">User Key of the user who initiates the call.</param>
		/// <param name="userId">ID of the user.</param>
		/// <param name="email">Email address of the user.</param>
		/// <param name="password">Password of the user.</param>
		/// <param name="firstName">First name of the user.</param>
		/// <param name="lastName">Last name of the user.</param>
		/// <param name="title">Title of the user.</param>
		/// <param name="officePhone">Office phone number of the user.</param>
		/// <param name="mobilePhone">Mobile phone number of the user.</param>
		/// <param name="language">Language of the user. For example: 'en_US' for English (US)</param>
		/// <param name="timezoneId">ID of the timezone of the user.</param>
		/// <param name="status">Status of the user. Possible values: 'active', 'suspended'</param>
		/// <param name="clientId">ID of the client.</param>
		/// <returns>True if the user was updated</returns>
		public bool UpdateUser(string userKey, long userId, string email = null, string password = null, string firstName = null, string lastName = null, string title = null, string officePhone = null, string mobilePhone = null, string language = null, long? timezoneId = null, UserStatus? status = null, long? clientId = null)
		{
			string path = "/User/SetInfo/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("user_key", userKey),
				new KeyValuePair<string, object>("user_id", userId)
			};
			if (status.HasValue) parameters.Add(new KeyValuePair<string, object>("status", status.Value.GetEnumMemberValue()));
			if (email != null) parameters.Add(new KeyValuePair<string, object>("email", email));
			if (firstName != null) parameters.Add(new KeyValuePair<string, object>("first_name", firstName));
			if (lastName != null) parameters.Add(new KeyValuePair<string, object>("last_name", lastName));
			if (title != null) parameters.Add(new KeyValuePair<string, object>("title", title));
			if (officePhone != null) parameters.Add(new KeyValuePair<string, object>("office_phone", officePhone));
			if (mobilePhone != null) parameters.Add(new KeyValuePair<string, object>("mobile_phone", mobilePhone));
			if (language != null) parameters.Add(new KeyValuePair<string, object>("language", language));
			if (timezoneId != null) parameters.Add(new KeyValuePair<string, object>("timezone_id", timezoneId));
			if (password != null)
			{
				parameters.Add(new KeyValuePair<string, object>("password", password));
				parameters.Add(new KeyValuePair<string, object>("password_confirmation", password));
			}
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<bool>(path, parameters);
		}

		/// <summary>
		/// Validate user name and password
		/// </summary>
		/// <param name="email">Email address of the user.</param>
		/// <param name="password">Password of the user.</param>
		/// <param name="clientId">ID of the client</param>
		/// <returns>The <see cref="LoginInfo">login information</see> for the user</returns>
		public LoginInfo Login(string email, string password, long? clientId = null)
		{
			var path = "/User/Login/";

			var parameters = new List<KeyValuePair<string, object>>()
			{
				new KeyValuePair<string, object>("email", email),
				new KeyValuePair<string, object>("password", password)

			};
			if (clientId.HasValue) parameters.Add(new KeyValuePair<string, object>("client_id", clientId.Value));

			return ExecuteRequest<LoginInfo>(path, parameters);
		}

		#endregion

		#region Internal Methods

		internal long ExecuteCountRequest(string urlPath, IEnumerable<KeyValuePair<string, object>> parameters)
		{
			return ExecuteRequest<long>(urlPath, parameters, "count");
		}

		internal T[] ExecuteArrayRequest<T>(string urlPath, IEnumerable<KeyValuePair<string, object>> parameters, string propertyName = null)
		{
			return ExecuteRequest<T[]>(urlPath, parameters, propertyName);
		}

		internal T ExecuteRequest<T>(string urlPath, IEnumerable<KeyValuePair<string, object>> parameters, string propertyName = null)
		{
			// Execute the API call
			var response = ExecuteRequest(urlPath, parameters);

			// Parse the response
			var data = ParseCakeMailResponse(response);

			// Check if the response is a well-known object type (JArray, of JValue)
			if (data is JArray) return (data as JArray).ToObject<T>();
			else if (data is JValue) return (data as JValue).ToObject<T>();

			// The response contains a JObject which we can return is a specific property was not requested
			if (string.IsNullOrEmpty(propertyName)) return (data as JObject).ToObject<T>();

			// The response contains a JObject but we only want a specific property. We must ensure the desired property is present
			var properties = (data as JObject).Properties().Where(p => p.Name.Equals(propertyName));
			if (!properties.Any()) throw new CakeMailException(string.Format("Json does not contain property {0}", propertyName));

			// Convert the property to the appropriate object type (JArray, JValue or JObject)
			var property = properties.First();
			if (property.Value is JArray) return (property.Value as JArray).ToObject<T>();
			else if (property.Value is JValue) return (property.Value as JValue).ToObject<T>();
			return (property.Value as JObject).ToObject<T>();
		}

		#endregion

		#region Private Methods

		private void InitializeResources()
		{
			this.Campaigns = new Campaigns(this);
			this.Clients = new Clients(this);
			this.Countries = new Countries(this);
			this.Permissions = new Permissions(this);
			this.Lists = new Lists(this);
			this.Timezones = new Timezones(this);
			this.Mailings = new Mailings(this);
			this.Relays = new Relays(this);
			this.Segments = new Segments(this);
		}

		private static string GetVersion()
		{
			try
			{
				// The following may throw 'System.Security.Permissions.FileIOPermission' under some circumpstances
				//var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

				// Here's an alternative suggested by Phil Haack: http://haacked.com/archive/2010/11/04/assembly-location-and-medium-trust.aspx
				var assemblyVersion = new AssemblyName(typeof(CakeMailRestClient).Assembly.FullName).Version;
				var version = string.Format("{0}.{1}.{2}.{3}", assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build, assemblyVersion.Revision);

				return version;
			}
			catch
			{
				return "0.0.0.0";
			}
		}

		private IRestResponse ExecuteRequest(string urlPath, IEnumerable<KeyValuePair<string, object>> parameters)
		{
			var request = new RestRequest(urlPath, Method.POST) { RequestFormat = DataFormat.Json };

			request.AddHeader("apikey", this.ApiKey);

			if (parameters != null)
			{
				foreach (var parameter in parameters)
				{
					request.AddParameter(parameter.Key, parameter.Value);
				}
			}

			var response = _client.Execute(request);
			var responseUri = response.ResponseUri ?? new Uri(string.Format("{0}/{1}", _client.BaseUrl.ToString().TrimEnd('/'), request.Resource.TrimStart('/')));

			if (response.ResponseStatus == ResponseStatus.Error)
			{
				var errorMessage = string.Format("Error received while making request: {0}", response.ErrorMessage);
				throw new HttpException(errorMessage, response.StatusCode, responseUri);
			}
			else if (response.ResponseStatus == ResponseStatus.TimedOut)
			{
				throw new HttpException("Request timed out", response.StatusCode, responseUri, response.ErrorException);
			}

			var statusCode = (int)response.StatusCode;
			if (statusCode == 200)
			{
				if (string.IsNullOrEmpty(response.Content))
				{
					var missingBodyMessage = string.Format("Received a 200 response from {0} but there was no message body.", request.Resource);
					throw new HttpException(missingBodyMessage, response.StatusCode, responseUri);
				}
				else if (response.ContentType == null || !response.ContentType.Contains("json"))
				{
					var unsupportedContentTypeMessage = string.Format("Received a 200 response from {0} but the content type is not JSON: {1}", request.Resource, response.ContentType ?? "NULL");
					throw new CakeMailException(unsupportedContentTypeMessage);
				}

				#region DEBUGGING
#if DEBUG
				var debugRequestMsg = string.Format("Request sent to CakeMail: {0}/{1}", _client.BaseUrl.ToString().TrimEnd('/'), urlPath.TrimStart('/'));
				var debugHeadersMsg = string.Format("Request headers: {0}", string.Join("&", request.Parameters.Where(p => p.Type == ParameterType.HttpHeader).Select(p => string.Concat(p.Name, "=", p.Value))));
				var debugParametersMsg = string.Format("Request parameters: {0}", string.Join("&", request.Parameters.Where(p => p.Type != ParameterType.HttpHeader).Select(p => string.Concat(p.Name, "=", p.Value))));
				var debugResponseMsg = string.Format("Response received from CakeMail: {0}", response.Content);
				Debug.WriteLine("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{0}", new string('=', 25), debugRequestMsg, debugHeadersMsg, debugParametersMsg, debugResponseMsg);
#endif
				#endregion

				// Request was successful
				return response;
			}
			else if (statusCode >= 400 && statusCode < 500)
			{
				if (string.IsNullOrEmpty(response.Content))
				{
					var missingBodyMessage = string.Format("Received a {0} error from {1} with no body", response.StatusCode, request.Resource);
					throw new HttpException(missingBodyMessage, response.StatusCode, responseUri);
				}

				var errorMessage = string.Format("Received a {0} error from {1} with the following content: {2}", response.StatusCode, request.Resource, response.Content);
				throw new HttpException(errorMessage, response.StatusCode, responseUri);
			}
			else if (statusCode >= 500 && statusCode < 600)
			{
				var errorMessage = string.Format("Received a server ({0}) error from {1}", (int)response.StatusCode, request.Resource);
				throw new HttpException(errorMessage, response.StatusCode, responseUri);
			}
			else if (!string.IsNullOrEmpty(response.ErrorMessage))
			{
				var errorMessage = string.Format("Received an error message from {0} (status code: {1}) (error message: {2})", request.Resource, (int)response.StatusCode, response.ErrorMessage);
				throw new HttpException(errorMessage, response.StatusCode, responseUri);
			}
			else
			{
				var errorMessage = string.Format("Received an unexpected response from {0} (status code: {1})", request.Resource, (int)response.StatusCode);
				throw new HttpException(errorMessage, response.StatusCode, responseUri);
			}
		}

		private JToken ParseCakeMailResponse(IRestResponse response)
		{
			try
			{
				/* A typical response from the CakeMail API looks like this:
				 *	{
				 *		"status" : "success",
				 *		"data" : { ... data for the API call ... }
				 *	}
				 *	
				 * In case of an error, the response looks like this:
				 *	{
				 *		"status" : "failed",
				 *		"data" : "An error has occured"
				 *	}
				 */
				var cakeResponse = JObject.Parse(response.Content);
				var status = cakeResponse["status"].ToString();
				var data = cakeResponse["data"];
				var postData = cakeResponse["post"];

				if (status != "success")
				{
					if (postData != null) throw new CakeMailPostException(data.ToString(), postData.ToString());
					else throw new CakeMailException(data.ToString());
				}

				return data;
			}
			catch (JsonReaderException ex)
			{
				throw new CakeMailException(string.Format("Unable to decode response from CakeMail as JSON: {0}", response.Content), ex);
			}
		}

		#endregion
	}
}