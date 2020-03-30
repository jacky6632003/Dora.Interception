﻿using Dora.Interception;

namespace WebApi_Dora_AOP.Common
{
    public class LogAttribute : InterceptorAttribute
    {
        public override void Use(IInterceptorChainBuilder builder)
        {
            builder.Use<LogInterceptor>(Order);
        }
    }
}
