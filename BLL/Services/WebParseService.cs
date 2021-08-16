using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using BLL.DTO;
using BLL.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using DAL.Model;
using BLL.DTO.Product;
using BLL.Helpers;
using Microsoft.Extensions.Options;

namespace BLL.Services
{
    public class WebParseService : IWebParseService
    {
        private readonly AppSettings _appSettings;

        public WebParseService(IOptions<AppSettings> options)
        {
            _appSettings = options.Value;
        }

        public List<ProductParse> GetProduct()
        {
            //https://geonode.com/free-proxy-list
            string _path = Path.Combine(Directory.GetCurrentDirectory(), 
                _appSettings.DirectoryForSerializeDeserialize,
                _appSettings.ProxyListFileName);
            List<Proxy> proxyList = Deserialize(_path);           
            
            List<int> idRemoveProxy = new List<int>();
            string html = String.Empty;
            for(int i = 0;  i < proxyList.Count; i++)
            {
                if (!String.IsNullOrEmpty(html))
                    continue;
                try
                {
                    WebRequest WR = WebRequest.Create("https://rozetka.com.ua/notebooks/c80004/");
                    WR.Method = "GET";
                    SetProxy(ref WR, proxyList[i]);
                    WebResponse webResponse = WR.GetResponse();
                    using Stream stream = webResponse.GetResponseStream();
                    using StreamReader sr = new StreamReader(stream);
                    html = sr.ReadToEnd();                    
                }
                catch (System.Net.WebException ex)
                {
                    if(ex.HResult == -2147467259)
                        idRemoveProxy.Add(i);
                }
                catch (Exception ex)
                {

                }
            }
            foreach(int id in idRemoveProxy)
            {
                proxyList.RemoveAt(id);
            }
            Serialize(proxyList, _path);
            

            //var config = Configuration.Default;
            //using var context = BrowsingContext.New(config);
            //using var doc = context.OpenAsync(req => req.Content(html)).GetAwaiter().GetResult();
            return ParserWorcer(html);
        }

        private List<Proxy> Deserialize(string path)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamReader sr = new StreamReader(path))
            using (JsonReader writer = new JsonTextReader(sr))
            {
                return (List<Proxy>)serializer.Deserialize(sr, typeof(List<Proxy>));
            }
        }

        private void Serialize(List<Proxy> proxies, string path)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, proxies);
            }
        }

        private void SetProxy(ref WebRequest WR, Proxy proxy)
        {
            WebProxy myProxy = new WebProxy(proxy.Ip, proxy.Prot);
            myProxy.BypassProxyOnLocal = true;
            WR.Proxy = myProxy;
        }

        private List<ProductParse> ParserWorcer(string html)
        {
            var domParser = new HtmlParser();

            var document = domParser.ParseDocument(html);

            return Parse(document);
        }

        public List<ProductParse> Parse(IHtmlDocument document)
        {
            var list = new List<ProductParse>();
            //var items = document.QuerySelectorAll("a")
            //    .Where(item => item.ClassName != null && item.ClassName
            //    .Contains("goods-tile__heading ng-star-inserted"));
            //foreach (var item in items)
            //{
            //    list.Add(item.TextContent);
            //}
            
            var descriptions = document.QuerySelectorAll("div")
                .Where(item => item.ClassName != null && item.ClassName
                .Contains("goods-tile ng-star-inserted")).ToList();
            var names = document.QuerySelectorAll("a")
                .Where(item => item.ClassName != null && item.ClassName
                .Contains("goods-tile__heading ng-star-inserted")).ToList();
            //var costs = document.QuerySelectorAll("div")
            //    .Where(item => item.ClassName != null && item.ClassName
            //    .Contains("goods-tile__price")).ToList();
            for(int i = 0; i < descriptions.Count; i++)
            {
                ProductParse productParse= new ProductParse();
                productParse.Name = names[i].TextContent;
                //productParse.Cost = costs[i].TextContent;
                productParse.NoParseDescription = descriptions[i].Children[1].TextContent;
                list.Add(productParse);
            }
            return list;
        }
    }
}
