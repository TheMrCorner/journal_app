using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

namespace InnerQuest
{
    public class RequestManager : MonoBehaviour
    {
        public Authentication authentication;
        public Login login;

        public URLHolder loginURLs;

        private static string contentType = "application/json";

        public void StartRequestManager(Login login)
        {
            this.login = login;
        } 

        public void SendGetRequest(string url, Action<RequestResponse> callback)
        {
            StartCoroutine(GetRequest(url, callback));
        } // SendGetRequest

        public void SendPostRequest(string url, string body, Action<RequestResponse> callback)
        {
            StartCoroutine(PostRequest(url, body, callback));
        } // SendPostRequest

        IEnumerator RefreshTokenRequest()
        {
            if (authentication.token == "")
            {
                yield return Authenticate();
            } // if
            else
            {
                RefreshToken refreshToken = new RefreshToken();
                refreshToken.token = authentication.token;
                string jsonData = JsonUtility.ToJson(refreshToken);
                UnityWebRequest request = UnityWebRequest.Post(loginURLs.findEntryByKey("refresh").value, jsonData, contentType);

                yield return request.SendWebRequest();
                switch (request.result)
                {
                    case UnityWebRequest.Result.ConnectionError |
                         UnityWebRequest.Result.DataProcessingError |
                         UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("Error processing request: " + request.error);
                        // If there was an error refreshing, reauthenticate
                        yield return Authenticate();
                        break;
                    case UnityWebRequest.Result.Success:
                        string response = request.downloadHandler.text;
                        authentication = JsonUtility.FromJson<Authentication>(response);
                        break;
                } // switch
            }
        }

        IEnumerator Authenticate()
        {
            string jsonData = JsonUtility.ToJson(login);
            UnityWebRequest request = UnityWebRequest.Post(loginURLs.findEntryByKey("login").value, jsonData, contentType);

            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError | 
                     UnityWebRequest.Result.DataProcessingError |
                     UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("Error authenticating: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                    string response = request.downloadHandler.text;
                    authentication = JsonUtility.FromJson<Authentication>(response);
                    break;
            } // switch
        } // Authenticate

        /// <summary>
        /// 
        /// GetRequest method. This function creates a get request and sends it to the server,
        /// then returns the result. If there is any problem it shall return the failure.
        /// 
        /// </summary>
        /// <param name="url"> (string) URL to which send the request </param>
        /// <param name="callback"> (Action) Function to which callback with result </param>
        /// <returns> Coroutine </returns>
        IEnumerator GetRequest(string url, Action<RequestResponse> callback = null)
        {
            yield return RefreshTokenRequest();
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", "Bearer " + authentication.accessToken);

            Debug.Log(request.ToString());

            yield return request.SendWebRequest();

            RequestResponse response = new RequestResponse();
            response.status = request.responseCode;
            response.result = request.downloadHandler.text;
            response.error = request.error;
            callback(response);
        } // GetRequest

        /// <summary>
        /// 
        /// PostRequest method to generate a POST request and send it to the server. Then returns
        /// the result.
        /// 
        /// </summary>
        /// <param name="url"> (string) URL to which send the request </param>
        /// <param name="body"> (string) Json object to put in the body </param>
        /// <param name="callback"> (Action) Callback to send the response </param>
        /// <returns> Coroutine </returns>
        IEnumerator PostRequest(string url, string body, Action<RequestResponse> callback = null)
        {
            yield return RefreshTokenRequest();
            UnityWebRequest request = UnityWebRequest.Post(url, body, contentType);
            request.SetRequestHeader("Authorization", "Bearer " + authentication.accessToken);

            yield return request.SendWebRequest();

            RequestResponse response = new RequestResponse();
            response.status = request.responseCode;
            response.result = request.downloadHandler.text;
            response.error = request.error;
            callback(response);
        } // PostRequest
    } // RequestManager
} // namespace
