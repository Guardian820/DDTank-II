using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Game.Server.WebLogin
{
    [GeneratedCode("System.ServiceModel", "4.0.0.0"), DebuggerStepThrough]
    public class PassPortSoapClient : ClientBase<PassPortSoap>, PassPortSoap
    {
        public PassPortSoapClient()
        {
        }

        public PassPortSoapClient(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        public PassPortSoapClient(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        public PassPortSoapClient(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
        }

        public PassPortSoapClient(Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        ChenckValidateResponse PassPortSoap.ChenckValidate(ChenckValidateRequest request)
        {
            return base.Channel.ChenckValidate(request);
        }

        public string ChenckValidate(string applicationname, string username, string password)
        {
            ChenckValidateResponse retVal = ((PassPortSoap)this).ChenckValidate(new ChenckValidateRequest
            {
                Body = new ChenckValidateRequestBody
                {
                    applicationname = applicationname,
                    username = username,
                    password = password
                }
            });
            return retVal.Body.ChenckValidateResult;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        Get_UserSexResponse PassPortSoap.Get_UserSex(Get_UserSexRequest request)
        {
            return base.Channel.Get_UserSex(request);
        }

        public bool? Get_UserSex(string applicationname, string username)
        {
            Get_UserSexResponse retVal = ((PassPortSoap)this).Get_UserSex(new Get_UserSexRequest
            {
                Body = new Get_UserSexRequestBody
                {
                    applicationname = applicationname,
                    username = username
                }
            });
            return retVal.Body.Get_UserSexResult;
        }
    }
}