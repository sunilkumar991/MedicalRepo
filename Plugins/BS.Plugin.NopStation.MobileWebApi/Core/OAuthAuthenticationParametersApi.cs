﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Authentication.External;

namespace BS.Plugin.NopStation.MobileWebApi.Core
{
    // not working 4.00
    //public class OAuthAuthenticationParametersApi : OpenAuthenticationParameters
    //{
    //    private readonly string _providerSystemName;
    //    private IList<UserClaims> _claims;

    //    public OAuthAuthenticationParametersApi(string providerSystemName)
    //    {
    //        this._providerSystemName = providerSystemName;
    //    }

    //    public override IList<UserClaims> UserClaims
    //    {
    //        get
    //        {
    //            return _claims;
    //        }
    //    }

    //    public void AddClaim(UserClaims claim)
    //    {
    //        if (_claims == null)
    //            _claims = new List<UserClaims>();

    //        _claims.Add(claim);
    //    }

    //    public override string ProviderSystemName
    //    {
    //        get { return _providerSystemName; }
    //    }
    //}
}
