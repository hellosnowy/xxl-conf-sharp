using HttpTracer;
using HttpTracer.Logger;
using Microsoft.Extensions.Options;
using RestSharp;
using xxl_conf_core.domain;

namespace xxl_conf_core.core
{
    /// <summary>
    /// 远程配置
    /// </summary>
    public class XxlConfRemoteConf: IDisposable
    {
        private readonly XxlConfFactory _options;

        protected static List<RestClient>? configClients = new List<RestClient>();


        public XxlConfRemoteConf(IOptions<XxlConfFactory> optionsAccessor)
        {
            configClients = new List<RestClient>();
            this._options = optionsAccessor?.Value;
            if (this._options != null)
            {
                // valid
                if (string.IsNullOrWhiteSpace(this._options.AdminAddress))
                {
                    throw new Exception("xxl-conf adminAddress can not be empty");
                }
                if (string.IsNullOrWhiteSpace(this._options.Env))
                {
                    throw new Exception("xxl-conf env can not be empty");
                }
                foreach (var url in this._options.AdminAddress.Split(','))
                {
                    var options = new RestClientOptions(url)
                    {
                        ConfigureMessageHandler = handler =>
                            new HttpTracerHandler(handler, new ConsoleLogger(), HttpMessageParts.All)
                    };
                    var restClient = new RestClient(options);
                    configClients.Add(restClient);
                }
            }

        }

        public string? find(string key)
        {
            Dictionary<string, string>? result = find(new List<string>() { key }).Result;
            if (result != null)
            {
                return result[key];
            }
            return null;
        }


        public async Task<Dictionary<string, string>?> find(List<string> keys)
        {
            foreach (var client in configClients)
            {
                XxlConfParamVO paramVO = new XxlConfParamVO();
                paramVO.AccessToken = this._options.AccessToken;
                paramVO.Env = this._options.Env;
                paramVO.Keys = keys;
                var request = new RestRequest("/conf/find", Method.Post).AddJsonBody(paramVO);
                var response =await client.PostAsync<ReturnT<Dictionary<string, string>>>(request);
                if (response != null && response.Code == Const.SUCCESS_CODE)
                {
                    return response.Data;
                }
            }
            return null;
        }

        public async Task<bool> monitor(List<String> keys)
        {
            foreach (var client in configClients)
            {
                XxlConfParamVO paramVO = new XxlConfParamVO();
                paramVO.AccessToken = this._options.AccessToken;
                paramVO.Env = this._options.Env;
                paramVO.Keys = keys;
                // get and valid
                var request = new RestRequest("/conf/monitor", Method.Post).AddJsonBody(paramVO);
                client.Options.MaxTimeout = 60000;
                var response =await client.PostAsync<ReturnT<string>>(request);
                if (response != null && response.Code == Const.SUCCESS_CODE)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        public void Dispose()
        {
            if (configClients.Count > 0) {
                foreach (var _client in configClients)
                {
                    _client?.Dispose();                   
                }
            }
        }
    }
}
