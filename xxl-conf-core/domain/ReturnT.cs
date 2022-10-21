namespace xxl_conf_core.domain
{
    public class ReturnT<T>
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public T Data { get; set; }
        public static readonly ReturnT<string> SUCCESS = new ReturnT<string>(null);
        public static readonly ReturnT<string> FAIL = new ReturnT<string>(Const.FAIL_CODE, null);

        public ReturnT()
        {
        }
        public ReturnT(int code, string msg)
        {
            Code = code;
            Msg = msg;
        }
        public ReturnT(T data)
        {
            Code = Const.SUCCESS_CODE;
            Data = data;
        }
    }
}
