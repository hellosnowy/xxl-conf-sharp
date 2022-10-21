using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Microsoft.Extensions.Options;
using xxl_conf_core.domain;

namespace xxl_conf_core.core
{
    /// <summary>
    /// 本地镜像缓存配置
    /// </summary>
    public class XxlConfMirrorConf
    {
        private readonly string mirrorfile = null;
        private readonly XxlConfFactory _options;
        public XxlConfMirrorConf(IOptions<XxlConfFactory> optionsAccessor)
        {
            this._options = optionsAccessor?.Value;
            if (this._options != null)
            {
                // valid
                if (string.IsNullOrWhiteSpace(this._options.Mirrorfile))
                {
                    throw new Exception("xxl-conf mirrorfileParam can not be empty");
                }
                mirrorfile = this._options.Mirrorfile;
            }
        }

        public  Dictionary<string, string> readConfMirror()
        {
            string? mirrorProp = Read();
            if (string.IsNullOrWhiteSpace(mirrorProp))
            {
                return null;
            }
            var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            return deserializer.Deserialize<Dictionary<string, string>>(mirrorProp);
        }


        public  void writeConfMirror(Dictionary<string, string> mirrorConfDataParam)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var yaml = serializer.Serialize(mirrorConfDataParam);
            // write mirror file
            Write(yaml);
        }

        /// <summary>
        /// 写文件到本地
        /// </summary>
        /// <param name="fileName"></param>
        private  void Write(string configData)
        {
            try
            {
                var path = Path.GetDirectoryName(mirrorfile);
                if (!Directory.Exists(path))//验证路径是否存在
                {
                    Directory.CreateDirectory(path);
                }
                File.WriteAllText(mirrorfile, configData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }


        /// <summary>
        /// 写文件到本地
        /// </summary>
        /// <param name="fileName"></param>
        private  string? Read()
        {
            try
            {
                var path = Path.GetDirectoryName(mirrorfile);
                if (!Directory.Exists(path))//验证路径是否存在
                {
                    Directory.CreateDirectory(path);
                }
                if (!File.Exists(mirrorfile)) return null;
                return File.ReadAllText(mirrorfile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }
    }
}
