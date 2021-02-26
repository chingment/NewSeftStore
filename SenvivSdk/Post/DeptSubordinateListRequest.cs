using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenvivSdk
{
    public class DeptSubordinateListRequest : IApiPostRequest<RequestResult<DeptSubordinateListResult>>
    {
        private object _postData = null;

        private string _token = "";

        public DeptSubordinateListRequest(string token, object postData)
        {
            this._token = token;
            this._postData = postData;
        }

        public string Token
        {
            get
            {
                return _token;
            }
        }

        public object PostData
        {
            get
            {
                return _postData;
            }
        }

        public string ApiUrl
        {

            get
            {
                return "lite/Admin/DeptSubordinateList";
            }
        }
    }
}
