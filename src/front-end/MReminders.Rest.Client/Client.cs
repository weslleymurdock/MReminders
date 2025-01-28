﻿namespace RemoteTickets.Rest.Client;

public partial class MRemindersClient
{
    public string BearerToken { get; set; }
    public string BasicToken { get; set; }
    partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
    {
        if (url.Contains("api/Account/Register"))
        {
            return;
        }
        if (url.Contains("api/Account/Login"))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", BasicToken);
        }
        else
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);
        }

    }
}
