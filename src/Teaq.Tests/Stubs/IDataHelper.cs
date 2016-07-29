using System;
using System.Data;
using System.Reflection;
using Teaq.FastReflection;

namespace Teaq.Tests.Stubs
{
    public interface IDataHelper
    {
        IDataReader GetReader();
    }
}