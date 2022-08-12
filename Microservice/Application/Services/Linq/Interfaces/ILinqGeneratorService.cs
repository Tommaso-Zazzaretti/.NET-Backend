using System.Linq.Expressions;

namespace Microservice.Application.Services.Linq.Interfaces
{
    public interface ILinqGeneratorService
    {
        public Expression<Func<T,dynamic>> Selector<T>(string PropertyName) where T : class;
        public Expression<Func<T, bool>> StringPredicate<T>(string PropertyName,string MethodName,string Pattern) where T : class;
        public Expression<Func<T, bool>> EqualityPredicate<T, TConst>(string StringPropertyName, TConst Constant) where T : class;
    }
}
